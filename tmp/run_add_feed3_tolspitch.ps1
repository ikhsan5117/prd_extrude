$serverName = "10.14.149.34"
$databaseName = "prd_extrude_hose"
$username = "usrvelasto"
$password = "H1s@na2025!!"

$connectionString = "Server=$serverName;Database=$databaseName;User Id=$username;Password=$password;TrustServerCertificate=True;Connection Timeout=300;"

$sqlScript = Get-Content -Path "add_feed3_tolspitch.sql" -Raw

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "Connected to database." -ForegroundColor Green
    
    $command = $connection.CreateCommand()
    $command.CommandText = $sqlScript
    $command.CommandTimeout = 300
    
    Write-Host "Adding Feed3 and ToleranceSpiralPitch columns..." -ForegroundColor Yellow
    $command.ExecuteNonQuery() | Out-Null
    
    Write-Host "[SUCCESS] Columns added!" -ForegroundColor Green
    Write-Host "Added Feed3 and ToleranceSpiralPitch (original string columns)" -ForegroundColor Cyan
    
    $connection.Close()
}
catch {
    Write-Host "[ERROR] Failed: $_" -ForegroundColor Red
    exit 1
}
