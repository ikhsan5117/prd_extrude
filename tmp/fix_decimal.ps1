# Fix decimal formatting di data rows
$filePath = "D:\extrude\Views\SpsMaster\Index.cshtml"
$content = Get-Content $filePath -Raw -Encoding UTF8

Write-Host "Formatting decimal data rows..." -ForegroundColor Yellow

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

$count = 0
foreach ($field in $decimalFields) {
    foreach ($suffix in @('_Min', '_Asli', '_Max')) {
        $old = "@item.$field$suffix"
        $new = "@(item.$field$suffix?.ToString(""F2"") ?? """")"
        
        if ($content -match [regex]::Escape($old)) {
            $content = $content.Replace($old, $new)
            $count++
        }
    }
}

Write-Host "Replaced $count decimal fields" -ForegroundColor Cyan
Write-Host "Saving..." -ForegroundColor Yellow

$content | Set-Content $filePath -Encoding UTF8 -NoNewline

Write-Host "[OK] Selesai! Decimal sudah diformat 2 digit." -ForegroundColor Green
