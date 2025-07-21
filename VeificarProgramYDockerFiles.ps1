$basePath = Get-Location
$servicio = "Booking"
$domainPath = Join-Path $basePath "$servicio\PetCare.$servicio.Domain"
$archivos = Get-ChildItem -Path $domainPath -Recurse -Filter *.cs
$corregidos = 0

Write-Host ""
Write-Host "Corrigiendo namespaces incorrectos en archivos .cs del microservicio Booking..."
Write-Host ""

foreach ($archivo in $archivos) {
    $contenido = Get-Content $archivo.FullName
    $modificado = $false

    $nuevoContenido = $contenido | ForEach-Object {
        if ($_ -match "namespace\s+PetCare\.BookingEntity") {
            $modificado = $true
            $_ -replace "namespace\s+PetCare\.BookingEntity", "namespace PetCare.Booking"
        } else {
            $_
        }
    }

    if ($modificado) {
        $nuevoContenido | Out-File -FilePath $archivo.FullName -Encoding UTF8
        Write-Host "Corregido: $($archivo.FullName)"
        $corregidos++
    }
}

Write-Host ""
Write-Host "Total de archivos corregidos: $corregidos"