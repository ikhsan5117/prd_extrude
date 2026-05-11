# Fix yang salah - restore suffix _Min, _Asli, _Max
$filePath = "D:\extrude\Views\SpsMaster\Index.cshtml"
$content = Get-Content $filePath -Raw -Encoding UTF8

Write-Host "Memperbaiki suffix yang hilang..." -ForegroundColor Yellow

$decimalFields = @(
    'Nipple', 'TubeDie', 'CoverDie', 'MiddleDie', 'SpacerDie', 'ADistance',
    'MeshDim1', 'MeshDim2', 'MeshDim3',
    'HeadTemp1', 'HeadTemp2', 'HeadTemp3',
    'Cylinder1_1', 'Cylinder1_2', 'Cylinder1_3',
    'Cylinder2_1', 'Cylinder2_2', 'Cylinder2_3',
    'Cylinder3_1', 'Cylinder3_2', 'Cylinder3_3',
    'ScrewTemp1', 'ScrewTemp2', 'ScrewTemp3',
    'Feed1', 'Feed2', 'Feed3',
    'ScrewSpeed1', 'ScrewSpeed2', 'ScrewSpeed3',
    'Pressure1', 'Pressure2', 'Pressure3',
    'FeedRollRatio1', 'FeedRollRatio2', 'FeedRollRatio3',
    'HoseSpeed', 'SpiralSpeed', 'ChillerWaterTemp',
    'CaterpillarGap', 'TakeUpConveyorSpeed',
    'TebalInner', 'TebalOuter', 'TebalTotal', 'TebalInnerMiddle'
)

# Fix yang salah dulu - kembalikan suffix
foreach ($field in $decimalFields) {
    # Fix Min
    $wrong = "@(item.$field.ToString(""F2"") ?? """")"
    $correct_min = "@(item.${field}_Min?.ToString(""F2"") ?? """")"
    $correct_asli = "@(item.${field}_Asli?.ToString(""F2"") ?? """")"
    $correct_max = "@(item.${field}_Max?.ToString(""F2"") ?? """")"
    
    # Replace wrong patterns dengan yang benar berdasarkan context
    # Ini tricky karena semua sama, jadi kita perlu lebih hati-hati
    # Lebih baik gunakan line-by-line approach
}

# Approach baru: Replace setiap baris dengan benar
$lines = $content -split "`r?`n"
$newLines = New-Object System.Collections.ArrayList

foreach ($line in $lines) {
    $newLine = $line
    
    foreach ($field in $decimalFields) {
        $wrongPattern = "@(item.$field.ToString(""F2"") ?? """")"
        
        if ($line -match [regex]::Escape($wrongPattern)) {
            # Detect context - is this Min, Asli, or Max column based on class
            if ($line -match 'class="text-success"') {
                # MIN column
                $newLine = $line.Replace($wrongPattern, "@(item.${field}_Min?.ToString(""F2"") ?? """")")
            }
            elseif ($line -match 'class="text-primary fw-bold"') {
                # ASLI column
                $newLine = $line.Replace($wrongPattern, "@(item.${field}_Asli?.ToString(""F2"") ?? """")")
            }
            elseif ($line -match 'class="text-danger"') {
                # MAX column
                $newLine = $line.Replace($wrongPattern, "@(item.${field}_Max?.ToString(""F2"") ?? """")")
            }
            break
        }
    }
    
    [void]$newLines.Add($newLine)
}

$content = $newLines -join "`r`n"

Write-Host "Saving corrected file..." -ForegroundColor Yellow
$content | Set-Content $filePath -Encoding UTF8 -NoNewline

Write-Host "[OK] File diperbaiki dengan suffix yang benar!" -ForegroundColor Green
