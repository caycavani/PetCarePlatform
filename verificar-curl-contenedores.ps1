$contenedores = docker ps --format "{{.Names}}"
$reportes = @()

foreach ($nombre in $contenedores) {
    Write-Host ""
    Write-Host "Verificando contenedor: $nombre"

    $resultado = docker exec $nombre bash -c "curl --version" 2>&1

    if ($resultado -match "curl\s+[0-9\.]+") {
        Write-Host "curl está disponible en $nombre"
        $reportes += "$($nombre): curl disponible"
    }
    else {
        Write-Host "curl NO disponible en $nombre"
        $reportes += "$($nombre): curl NO disponible"
    }
}

Write-Host ""
Write-Host "Resumen final:"
foreach ($linea in $reportes) {
    Write-Host $linea
}
