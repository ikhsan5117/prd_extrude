# Script untuk perbaiki Views/SpsMaster/Index.cshtml
# 1. Hapus duplikasi text MIN/ASLI/MAX dari header (badge sudah cukup jelas)
# 2. Format decimal jadi 2 digit
# 3. Rapikan marker classes

$filePath = "D:\extrude\Views\SpsMaster\Index.cshtml"
$content = Get-Content $filePath -Raw -Encoding UTF8

Write-Host "Memperbaiki header columns..." -ForegroundColor Yellow

# Fix headers - hapus duplikasi text, biarkan badge saja + nama field
# Pattern: <span class="badge bg-success">MIN</span>Nipple Min → <span class="badge bg-success">MIN</span> Nipple
$content = $content -replace '(<span class="badge bg-success">MIN</span>)([A-Za-z0-9\s\-\.]+?)\s*Min(?=</th>)', '$1 $2'
$content = $content -replace '(<span class="badge bg-primary">ASLI</span>)([A-Za-z0-9\s\-\.]+?)\s*Asli(?=</th>)', '$1 $2'
$content = $content -replace '(<span class="badge bg-danger">MAX</span>)([A-Za-z0-9\s\-\.]+?)\s*Max(?=</th>)', '$1 $2'

# Specific fixes untuk nama yang tidak ada suffix
$headerFixes = @{
    '>MIN</span>Nipple<' = '>MIN</span> Nipple<'
    '>ASLI</span>Nipple<' = '>ASLI</span> Nipple<'
    '>MAX</span>Nipple<' = '>MAX</span> Nipple<'
    '>MIN</span>Tube Die<' = '>MIN</span> Tube Die<'
    '>ASLI</span>Tube Die<' = '>ASLI</span> Tube Die<'
    '>MAX</span>Tube Die<' = '>MAX</span> Tube Die<'
    '>MIN</span>Middle Die<' = '>MIN</span> Middle Die<'
    '>ASLI</span>Middle Die<' = '>ASLI</span> Middle Die<'
    '>MAX</span>Middle Die<' = '>MAX</span> Middle Die<'
    '>MIN</span>Cover Die<' = '>MIN</span> Cover Die<'
    '>ASLI</span>Cover Die<' = '>ASLI</span> Cover Die<'
    '>MAX</span>Cover Die<' = '>MAX</span> Cover Die<'
    '>MIN</span>Spacer<' = '>MIN</span> Spacer<'
    '>ASLI</span>Spacer<' = '>ASLI</span> Spacer<'
    '>MAX</span>Spacer<' = '>MAX</span> Spacer<'
    '>MIN</span>A Distance<' = '>MIN</span> A Distance<'
    '>ASLI</span>A Distance<' = '>ASLI</span> A Distance<'
    '>MAX</span>A Distance<' = '>MAX</span> A Distance<'
    '>MIN</span>Mesh Dim' = '>MIN</span> Mesh Dim'
    '>ASLI</span>Mesh Dim' = '>ASLI</span> Mesh Dim'
    '>MAX</span>Mesh Dim' = '>MAX</span> Mesh Dim'
    '>MIN</span>Head ' = '>MIN</span> Head '
    '>ASLI</span>Head ' = '>ASLI</span> Head '
    '>MAX</span>Head ' = '>MAX</span> Head '
    '>MIN</span>Cyl ' = '>MIN</span> Cyl '
    '>ASLI</span>Cyl ' = '>ASLI</span> Cyl '
    '>MAX</span>Cyl ' = '>MAX</span> Cyl '
    '>MIN</span>Screw ' = '>MIN</span> Screw '
    '>ASLI</span>Screw ' = '>ASLI</span> Screw '
    '>MAX</span>Screw ' = '>MAX</span> Screw '
    '>MIN</span>Feed ' = '>MIN</span> Feed '
    '>ASLI</span>Feed ' = '>ASLI</span> Feed '
    '>MAX</span>Feed ' = '>MAX</span> Feed '
    '>MIN</span>Screw V' = '>MIN</span> Screw V'
    '>ASLI</span>Screw V' = '>ASLI</span> Screw V'
    '>MAX</span>Screw V' = '>MAX</span> Screw V'
    '>MIN</span>Feed Roll' = '>MIN</span> Feed Roll'
    '>ASLI</span>Feed Roll' = '>ASLI</span> Feed Roll'
    '>MAX</span>Feed Roll' = '>MAX</span> Feed Roll'
    '>MIN</span>Press ' = '>MIN</span> Press '
    '>ASLI</span>Press ' = '>ASLI</span> Press '
    '>MAX</span>Press ' = '>MAX</span> Press '
    '>MIN</span>Sp-Spd<' = '>MIN</span> Spiral Speed<'
    '>ASLI</span>Sp-Spd<' = '>ASLI</span> Spiral Speed<'
    '>MAX</span>Sp-Spd<' = '>MAX</span> Spiral Speed<'
    '>MIN</span>Hose-S<' = '>MIN</span> Hose Speed<'
    '>ASLI</span>Hose-S<' = '>ASLI</span> Hose Speed<'
    '>MAX</span>Hose-S<' = '>MAX</span> Hose Speed<'
    '>MIN</span>Chiller<' = '>MIN</span> Chiller<'
    '>ASLI</span>Chiller<' = '>ASLI</span> Chiller<'
    '>MAX</span>Chiller<' = '>MAX</span> Chiller<'
    '>MIN</span>Cat.Gap<' = '>MIN</span> Caterpillar Gap<'
    '>ASLI</span>Cat.Gap<' = '>ASLI</span> Caterpillar Gap<'
    '>MAX</span>Cat.Gap<' = '>MAX</span> Caterpillar Gap<'
    '>MIN</span>Take-up<' = '>MIN</span> Take-up Speed<'
    '>ASLI</span>Take-up<' = '>ASLI</span> Take-up Speed<'
    '>MAX</span>Take-up<' = '>MAX</span> Take-up Speed<'
    '>MIN</span>Tebal In<' = '>MIN</span> Tebal Inner<'
    '>ASLI</span>Tebal In<' = '>ASLI</span> Tebal Inner<'
    '>MAX</span>Tebal In<' = '>MAX</span> Tebal Inner<'
    '>MIN</span>In+Mid<' = '>MIN</span> Inner+Middle<'
    '>ASLI</span>In+Mid<' = '>ASLI</span> Inner+Middle<'
    '>MAX</span>In+Mid<' = '>MAX</span> Inner+Middle<'
    '>MIN</span>Tebal Out<' = '>MIN</span> Tebal Outer<'
    '>ASLI</span>Tebal Out<' = '>ASLI</span> Tebal Outer<'
    '>MAX</span>Tebal Out<' = '>MAX</span> Tebal Outer<'
    '>MIN</span>Tot Tebal<' = '>MIN</span> Total Thickness<'
    '>ASLI</span>Tot Tebal<' = '>ASLI</span> Total Thickness<'
    '>MAX</span>Tot Tebal<' = '>MAX</span> Total Thickness<'
}

foreach ($key in $headerFixes.Keys) {
    $content = $content -replace [regex]::Escape($key), $headerFixes[$key]
}

Write-Host "Formatting decimal values (2 digits)..." -ForegroundColor Yellow

# Format decimal properties dengan ?.ToString("F2")
# Pattern: @item.Field_Min → @(item.Field_Min?.ToString("F2") ?? "")
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

foreach ($field in $decimalFields) {
    foreach ($suffix in @('_Min', '_Asli', '_Max')) {
        $oldPattern = "@item\.$field$suffix"
        $newPattern = "@(item.$field$suffix?.ToString(""F2"") ?? """")"
        $content = $content -replace [regex]::Escape($oldPattern), $newPattern
    }
}

Write-Host "`nSaving fixed file..." -ForegroundColor Yellow
$content | Set-Content $filePath -Encoding UTF8 -NoNewline

Write-Host "`n[OK] SELESAI! File sudah diperbaiki:" -ForegroundColor Green
Write-Host "  - Header columns dibersihkan (no duplikasi text)" -ForegroundColor Cyan
Write-Host "  - Decimal diformat jadi 2 digit (.ToString('F2'))" -ForegroundColor Cyan
Write-Host "  - File: $filePath" -ForegroundColor Cyan
Write-Host "`nSilakan build dan test!" -ForegroundColor Yellow
