$connString = 'Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;MultipleActiveResultSets=True;Encrypt=False;'
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT Dimensi, TebalInner, ToleranceInner, HoseType, InnerTarget, ThickTarget, TebalInner_Asli, ToleranceInner_Asli FROM SpsNoDocs WHERE DocumentNumber='VI-SOP-PROD-132'"
$r = $cmd.ExecuteReader()
while($r.Read()) {
    Write-Host 'Dimensi:' $r['Dimensi']
    Write-Host 'TebalInner:' $r['TebalInner']
    Write-Host 'ToleranceInner:' $r['ToleranceInner']
    Write-Host 'InnerTarget:' $r['InnerTarget']
    Write-Host 'ThickTarget:' $r['ThickTarget']
    Write-Host 'TebalInner_Asli:' $r['TebalInner_Asli']
    Write-Host 'ToleranceInner_Asli:' $r['ToleranceInner_Asli']
}
$conn.Close()
