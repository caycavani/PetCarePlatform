param (
    [int]$PuertoBase = 5000,
    [string]$RutaDestino = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform",
    [string[]]$Servicios = @("Auth", "Pets", "Booking", "Review", "Notification", "Payment")
)

$contenido = @"
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Panel de Microservicios - PetCare</title>
    <style>
        body { font-family: Segoe UI, sans-serif; background: #f5f5f5; margin: 40px; }
        h1 { color: #333; }
        table { border-collapse: collapse; width: 100%; margin-top: 20px; background: #fff; }
        th, td { padding: 12px 16px; border-bottom: 1px solid #ccc; text-align: left; }
        tr:hover { background: #f0f0f0; }
        a { text-decoration: none; color: #0078d7; }
    </style>
</head>
<body>
    <h1>🐾 Panel de Microservicios - PetCare</h1>
    <table>
        <tr><th>Servicio</th><th>Swagger</th><th>Health</th></tr>
"@

for ($i = 0; $i -lt $Servicios.Count; $i++) {
    $nombre = $Servicios[$i]
    $puerto = $PuertoBase + $i
    $contenido += "        <tr><td>$nombre</td><td><a href='http://localhost:$puerto/swagger' target='_blank'>Swagger</a></td><td><a href='http://localhost:$puerto/health' target='_blank'>Health</a></td></tr>"
}

# Línea para el dashboard visual
$puertoUI = $PuertoBase + $Servicios.IndexOf("Auth")
$contenido += "        <tr><td><strong>Health Dashboard</strong></td><td colspan='2'><a href='http://localhost:$puertoUI/health-ui' target='_blank'>Ver interfaz unificada</a></td></tr>"

$contenido += @"
    </table>
</body>
</html>
"@

$ruta = Join-Path $RutaDestino "landing.html"
$contenido | Set-Content -Path $ruta -Encoding UTF8

Write-Host "`n✅ landing.html creado en:"
Write-Host "   $ruta"
Write-Host "`n💡 Ábrelo en el navegador para acceder rápidamente a todos los servicios."