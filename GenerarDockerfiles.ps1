$basePath = Get-Location
$objetivo = "net8.0"
$csprojFiles = Get-ChildItem -Path $basePath -Recurse -Filter *.csproj
$modificados = 0

Write-Host "`n🔧 Unificando TargetFramework a $objetivo..." -ForegroundColor Cyan

foreach ($archivo in $csprojFiles) {
    [xml]$xml = Get-Content $archivo.FullName

    $tfNode = $xml.Project.PropertyGroup.TargetFramework
    if ($tfNode -and $tfNode -ne $objetivo) {
        Write-Host "✏️  Modificando: $($archivo.FullName)" -ForegroundColor Yellow
        $tfNode.InnerText = $objetivo
        $xml.Save($archivo.FullName)
        $modificados++
    }
}

Write-Host "`n📊 Resultado:" -ForegroundColor Cyan
if ($modificados -eq 0) {
    Write-Host "✅ Todos los proyectos ya estaban en $objetivo." -ForegroundColor Green
} else {
    Write-Host "✅ Se actualizaron $modificados archivo(s) .csproj a $objetivo." -ForegroundColor Green
}