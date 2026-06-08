$dllPath = "c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\Hino.Solar.DatabaseConnector\bin\Debug\MySql.Data.dll"
if (-not (Test-Path $dllPath)) {
    $dllPath = "c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\bin\MySql.Data.dll"
}
Write-Output "Loading DLL from: $dllPath"
[System.Reflection.Assembly]::LoadFrom($dllPath) | Out-Null

$conn = New-Object MySql.Data.MySqlClient.MySqlConnection("Server=localhost;Database=scada;Uid=root;Pwd=101101;")
try {
    $conn.Open()
    Write-Output "Successfully connected to scada database."
    
    # 1. Row counts
    $cmd = New-Object MySql.Data.MySqlClient.MySqlCommand("SELECT COUNT(*) FROM batches", $conn)
    $batchCount = $cmd.ExecuteScalar()
    $cmd.CommandText = "SELECT COUNT(*) FROM runs"
    $runCount = $cmd.ExecuteScalar()
    $cmd.CommandText = "SELECT COUNT(*) FROM run_info"
    $bomCount = $cmd.ExecuteScalar()
    Write-Output "Row counts: Batches: $batchCount | Runs: $runCount | Run Info (BOM): $bomCount"
    
    # 2. Latest 5 Batches
    Write-Output "`n--- Latest 5 Batches ---"
    $cmd.CommandText = "SELECT id, name, status, product_name, target_weight, start_time, end_time FROM batches ORDER BY id DESC LIMIT 5"
    $reader = $cmd.ExecuteReader()
    while ($reader.Read()) {
        Write-Output "Batch ID: $($reader['id']) | Name: $($reader['name']) | Status: $($reader['status']) | Product: $($reader['product_name']) | TargetWeight: $($reader['target_weight']) | StartTime: $($reader['start_time'])"
    }
    $reader.Close()
    
    # 3. Latest 5 Runs
    Write-Output "`n--- Latest 5 Runs ---"
    $cmd.CommandText = "SELECT id, batch_id, run_number, name, status, start_time FROM runs ORDER BY id DESC LIMIT 5"
    $reader = $cmd.ExecuteReader()
    while ($reader.Read()) {
        Write-Output "Run ID: $($reader['id']) | Batch ID: $($reader['batch_id']) | Num: $($reader['run_number']) | Name: $($reader['name']) | Status: $($reader['status'])"
    }
    $reader.Close()

    # 4. Check distinct run_ids in run_info
    Write-Output "`n--- Distinct Run IDs in run_info ---"
    $cmd.CommandText = "SELECT DISTINCT run_id FROM run_info"
    $reader = $cmd.ExecuteReader()
    $runIds = @()
    while ($reader.Read()) {
        $runIds += $reader['run_id'].ToString()
    }
    $reader.Close()
    Write-Output "Distinct run_ids in run_info: $($runIds -join ', ')"

    # 5. Check if those run_ids exist in runs table
    if ($runIds.Count -gt 0) {
        Write-Output "`n--- Checking if run_ids exist in runs table ---"
        foreach ($rid in $runIds) {
            $cmd.CommandText = "SELECT COUNT(*) FROM runs WHERE id = $rid"
            $exists = $cmd.ExecuteScalar()
            $cmd.CommandText = "SELECT name, status FROM runs WHERE id = $rid"
            $r = $cmd.ExecuteReader()
            $info = ""
            if ($r.Read()) {
                $info = " (Name: $($r['name']), Status: $($r['status']))"
            }
            $r.Close()
            Write-Output "Run ID $rid in runs table count: $exists $info"
        }
    }

    # 6. Sample BOM entries
    Write-Output "`n--- Sample BOM entries (first 5) ---"
    $cmd.CommandText = "SELECT id, run_id, code, material_code, quantity, unit, batch_no FROM run_info ORDER BY id ASC LIMIT 5"
    $reader = $cmd.ExecuteReader()
    while ($reader.Read()) {
        Write-Output "BOM ID: $($reader['id']) | Run ID: $($reader['run_id']) | Code: $($reader['code']) | Material: $($reader['material_code']) | Qty: $($reader['quantity']) | Unit: $($reader['unit']) | Lot: $($reader['batch_no'])"
    }
    $reader.Close()
}
catch {
    Write-Error $_.Exception.Message
}
finally {
    $conn.Close()
}
