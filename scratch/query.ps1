$dllPath = "c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\Hino.Solar.DatabaseConnector\bin\Debug\MySql.Data.dll"
[System.Reflection.Assembly]::LoadFrom($dllPath) | Out-Null

$conn = New-Object MySql.Data.MySqlClient.MySqlConnection("Server=localhost;Database=scada;Uid=root;Pwd=101101;")
$conn.Open()

$cmd = New-Object MySql.Data.MySqlClient.MySqlCommand("SELECT id, batch_id, name, status, start_time, end_time FROM runs WHERE batch_id = 1 ORDER BY id ASC LIMIT 1", $conn)
$reader = $cmd.ExecuteReader()
if ($reader.Read()) {
    Write-Output "Resolved: ID: $($reader['id']) | BatchID: $($reader['batch_id']) | Name: $($reader['name']) | Status: $($reader['status'])"
} else {
    Write-Output "No run found"
}
$reader.Close()

$conn.Close()
