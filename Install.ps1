$versionEstable = "8.0.100"
$targetFrameworkEstable = "net8.0"
$solutionPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform"
$globalJsonPath = Join-Path $solutionPath "global.json"

Write-Host "`n🔍 Buscando archivos .csproj con versiones inestables..." -ForegroundColor Cyan
$csprojFiles = Get-ChildItem -Path $solutionPath -Filter *.csproj -Recurse

foreach ($file in $csprojFiles) {
    $contenido = Get-Content $file.FullName
    $nuevoContenido = $contenido -replace "<TargetFramework>net9\.0.*?</TargetFramework>", "<TargetFramework>$targetFrameworkEstable</TargetFramework>"
    if ($contenido -ne $nuevoContenido) {
        Set-Content $file.FullName $nuevoContenido
        Write-Host "🔧 Actualizado: $($file.FullName)" -ForegroundColor Yellow
    }
}

Write-Host "`n✔️ Archivos .csproj verificados." -ForegroundColor Green

$dotnetSdks = & dotnet --list-sdks
if (-not ($dotnetSdks | Where-Object { $_ -like "$versionEstable*" })) {
    Write-Host "`n⚠️ SDK $versionEstable no está instalado. Abriendo página oficial..." -ForegroundColor Red
    Start-Process "https://dotnet.microsoft.com/en-us/download/dotnet/8.0"
    Write-Host "📌 Instala el SDK y luego vuelve a ejecutar este script." -ForegroundColor Magenta
    exit
}

$globalJsonContent = @{
    sdk = @{ version = $versionEstable }
} | ConvertTo-Json -Depth 3

$globalJsonContent | Out-File -FilePath $globalJsonPath -Encoding UTF8
Write-Host "`n📝 Archivo global.json configurado con versión $versionEstable" -ForegroundColor Cyan

Write-Host "`n🧹 Limpiando y restaurando paquetes..." -ForegroundColor DarkCyan
Set-Location $solutionPath
dotnet clean
dotnet restore

Write-Host "`n✅ Entorno estabilizado con .NET $versionEstable. Ahora puedes compilar tu solución." -ForegroundColor Green