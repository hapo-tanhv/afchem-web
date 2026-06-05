$paths = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
)

$msbuildPath = $null
foreach ($path in $paths) {
    if (Test-Path $path) {
        $msbuildPath = $path
        break
    }
}

if ($null -eq $msbuildPath) {
    # Recursive search as a fallback in common folders
    Write-Output "Searching for MSBuild.exe..."
    $found = Get-ChildItem -Path "C:\Program Files\Microsoft Visual Studio" -Filter "MSBuild.exe" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($found) {
        $msbuildPath = $found.FullName
    }
}

if ($msbuildPath) {
    Write-Output "Found MSBuild at: $msbuildPath"
    Write-Output "Building solution LongDucProjectTest.sln..."
    & $msbuildPath "LongDucProjectTest.sln" /t:Build /p:Configuration=Debug /p:Platform="Any CPU"
} else {
    Write-Error "MSBuild.exe not found. Please compile the solution in Visual Studio manually."
}
