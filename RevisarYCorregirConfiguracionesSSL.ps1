$basePath = Get-Location
$errores = 0
$corregidos = 0

Write-Host "`nCorrigiendo configuración SSL en archivos de entorno..."

# 1. Corregir appsettings.json y appsettings.Development.json
$appsettings = Get-ChildItem -Path $basePath -Recurse -Include "appsettings*.json" -File

foreach ($archivo in $appsettings) {
    $contenido = Get-Content $archivo.FullName -Raw
    $original = $contenido

    $contenido = $contenido -replace '"Encrypt"\s*:\s*true', '"Encrypt": false'
    $contenido = $contenido -replace 'Encrypt=True', 'Encrypt=False'
    $contenido = $contenido -replace 'https://(localhost|[a-zA-Z0-9\-]+):\d+', { param($m) $m.Value -replace 'https', 'http' }

    if ($contenido -ne $original) {
        $contenido | Out-File -FilePath $archivo.FullName -Encoding UTF8
        Write-Host "Corregido: $($archivo.FullName)"
        $corregidos++
    }
}

# 2. Corregir launchSettings.json
$launchSettings = Get-ChildItem -Path $basePath -Recurse -Include "launchSettings.json" -File

foreach ($archivo in $launchSettings) {
    $contenido = Get-Content $archivo.FullName -Raw
    $original = $contenido

    $contenido = $contenido -replace 'https://localhost:\d+;?', ''
    $contenido = $contenido -replace 'applicationUrl"\s*:\s*"\s*;?', ''

    if ($contenido -ne $original) {
        $contenido | Out-File -FilePath $archivo.FullName -Encoding UTF8
        Write-Host "launchSettings corregido: $($archivo.FullName)"
        $corregidos++
    }
}

# 3. Agregar configuración Kestrel en Program.cs si no existe
foreach ($servicio in @("Auth", "Booking", "Notification", "Payment", "Pets", "Review")) {
    $programPath = Join-Path $basePath "$servicio\PetCare.$servicio.Api\Program.cs"
    if (Test-Path $programPath) {
        $contenido = Get-Content $programPath -Raw
        if ($contenido -notmatch "ConfigureKestrel") {
            $bloque = @'
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // Solo HTTP
});
'@
            $contenido = $contenido -replace 'var builder = WebApplication.CreateBuilder\(args\);', "var builder = WebApplication.CreateBuilder(args);\n$bloque"
            $contenido | Out-File -FilePath $programPath -Encoding UTF8
            Write-Host "Kestrel configurado para HTTP en: $programPath"
            $corregidos++
        }
    }
}

Write-Host "`nResumen:"
Write-Host "Correcciones aplicadas: $corregidos"
Write-Host "Errores no corregidos: $errores"