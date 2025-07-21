$basePath = Get-Location
$servicios = @("Auth", "Booking", "Notification", "Payment", "Pets", "Review")
$objetivo = "net8.0"
$errores = 0
$corregidos = 0
$logs = @{}

Write-Host "`nIniciando reparación automática de microservicios..."

foreach ($servicio in $servicios) {
    $log = @()
    $log += "Microservicio: $servicio"
    $csprojPaths = Get-ChildItem -Path "$basePath\$servicio" -Recurse -Filter *.csproj

    foreach ($csproj in $csprojPaths) {
        $log += "`nArchivo: $($csproj.FullName)"
        try {
            [xml]$xml = Get-Content $csproj.FullName
        } catch {
            $log += "ERROR: No se pudo leer el archivo XML"
            $errores++
            continue
        }

        # Verificar y corregir TargetFramework
        $tf = $xml.Project.PropertyGroup.TargetFramework
        if ($tf -and $tf -ne $objetivo) {
            $log += "Corrigiendo TargetFramework: $tf → $objetivo"
            $xml.Project.PropertyGroup.TargetFramework = $objetivo
            $corregidos++
        } elseif (-not $tf) {
            $log += "Agregando TargetFramework faltante: $objetivo"
            $tfNode = $xml.CreateElement("TargetFramework")
            $tfNode.InnerText = $objetivo
            $xml.Project.PropertyGroup.AppendChild($tfNode) | Out-Null
            $corregidos++
        } else {
            $log += "TargetFramework correcto: $tf"
        }

        # Verificar y corregir referencias
        $refs = $xml.SelectNodes("//ProjectReference")
        foreach ($ref in $refs) {
            $rutaRelativa = $ref.Include -replace '\\', '/'
            $rutaCompleta = Join-Path (Split-Path $csproj.FullName -Parent) $rutaRelativa

            if (-not (Test-Path $rutaCompleta)) {
                $log += "Referencia rota: $rutaRelativa"
                $archivoBuscado = Split-Path $rutaRelativa -Leaf
                $posibles = Get-ChildItem -Path "$basePath\$servicio" -Recurse -Filter $archivoBuscado

                if ($posibles.Count -eq 1) {
                    $nuevaRuta = Resolve-Path -Relative -Path $posibles[0].FullName -RelativeBase (Split-Path $csproj.FullName -Parent)
                    $ref.Include = $nuevaRuta -replace '/', '\'
                    $log += "→ Reparada con: $nuevaRuta"
                    $corregidos++
                } elseif ($posibles.Count -gt 1) {
                    $log += "→ Múltiples coincidencias encontradas. No se corrigió automáticamente."
                    $errores++
                } else {
                    $log += "→ No se encontró el archivo referenciado. Requiere revisión manual."
                    $errores++
                }
            } else {
                $log += "Referencia válida: $rutaRelativa"
            }
        }

        # Guardar cambios en el archivo
        $xml.Save($csproj.FullName)
    }

    $logs[$servicio] = $log
}

# Guardar logs por microservicio
foreach ($kvp in $logs.GetEnumerator()) {
    $nombre = $kvp.Key
    $contenido = $kvp.Value -join "`n"
    $rutaLog = Join-Path $basePath "reparacion_$nombre.log"
    $contenido | Out-File -FilePath $rutaLog -Encoding UTF8
    Write-Host "Log generado: $rutaLog"
}

Write-Host "`nResumen del diagnóstico:"
Write-Host "Cambios aplicados: $corregidos"
Write-Host "Problemas no corregidos: $errores"