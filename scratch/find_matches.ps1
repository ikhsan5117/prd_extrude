$conn1 = New-Object System.Data.SqlClient.SqlConnection("Server=10.14.149.34;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;")
$conn2 = New-Object System.Data.SqlClient.SqlConnection("Server=10.14.149.34;Database=ELWP_PRD;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;")

try {
    # 1. Get SPS data from both tables
    $querySPS1 = "SELECT HoseType, ItemList FROM MasterlistSpsDoubleLayers"
    $querySPS2 = "SELECT HoseType, ItemList, ProductCode FROM StandardParameterSettings"
    
    $dtSPS = New-Object System.Data.DataTable
    $adapter1 = New-Object System.Data.SqlClient.SqlDataAdapter($querySPS1, $conn1)
    $adapter1.Fill($dtSPS) | Out-Null
    
    $dtSPS2 = New-Object System.Data.DataTable
    $adapter1b = New-Object System.Data.SqlClient.SqlDataAdapter($querySPS2, $conn1)
    $adapter1b.Fill($dtSPS2) | Out-Null

    # 2. Get Planning from ELWP
    $queryPlanning1 = "SELECT DISTINCT KodeItem, PartName FROM produksi.tb_elwp_produksi_plannings"
    $dtElwp = New-Object System.Data.DataTable
    $adapter2 = New-Object System.Data.SqlClient.SqlDataAdapter($queryPlanning1, $conn2)
    $adapter2.Fill($dtElwp) | Out-Null

    # 3. Get Planning from Local
    $queryPlanning2 = "SELECT DISTINCT PartName1, PartName2 FROM PlanningMasters"
    $dtLocal = New-Object System.Data.DataTable
    $adapter3 = New-Object System.Data.SqlClient.SqlDataAdapter($queryPlanning2, $conn1)
    $adapter3.Fill($dtLocal) | Out-Null

    function CheckMatch($kode) {
        if ([string]::IsNullOrWhiteSpace($kode)) { return $null }
        $k = ($kode -replace '\s+', '').ToUpper()
        
        # Check Masterlist
        foreach ($s in $dtSPS.Rows) {
            if (($s.HoseType -replace '\s+', '').ToUpper() -eq $k -or ($s.ItemList -replace '\s+', '').ToUpper() -eq $k) {
                return "MasterlistSpsDoubleLayers"
            }
        }
        # Check StandardParameterSettings
        foreach ($s in $dtSPS2.Rows) {
            if (($s.HoseType -replace '\s+', '').ToUpper() -eq $k -or ($s.ItemList -replace '\s+', '').ToUpper() -eq $k -or ($s.ProductCode -replace '\s+', '').ToUpper() -eq $k) {
                return "StandardParameterSettings"
            }
        }
        return $null
    }

    Write-Host "--- MATCHES IN ELWP PLANNING ---"
    $count = 0
    foreach ($row in $dtElwp.Rows) {
        $source = CheckMatch($row.KodeItem)
        if ($source) {
            Write-Host "ELWP: [$($row.KodeItem)] ($($row.PartName)) -> Found in $source"
            $count++
        }
        if ($count -ge 10) { break }
    }

    Write-Host "`n--- MATCHES IN LOCAL PLANNING MASTERS ---"
    $count = 0
    foreach ($row in $dtLocal.Rows) {
        $source = CheckMatch($row.PartName1)
        if ($source) {
            Write-Host "LOCAL: [$($row.PartName1)] ($($row.PartName2)) -> Found in $source"
            $count++
        }
        if ($count -ge 10) { break }
    }

} catch {
    Write-Host "Error: $($_.Exception.Message)"
} finally {
    $conn1.Close()
    $conn2.Close()
}
