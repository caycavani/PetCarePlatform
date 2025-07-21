param (
    [string]$SolutionRoot = "."
)

$archivosReparados = 0
Write-Host "Iniciando reparación de usings dentro del namespace..."

$csFiles = Get-ChildItem -Path $SolutionRoot -Recurse -Filter *.cs

foreach ($archivo in $csFiles) {
    $contenido = Get-Content $archivo.FullName
    if (-not $contenido -or $contenido.Count -eq 0) { continue }

    $usingsAntes = @()
    $usingsDentro = @()
    $namespaceIndex = -1

    for ($i = 0; $i -lt $contenido.Count; $i++) {
        $linea = $contenido[$i]

        if ($linea -match "^namespace\s") {
            $namespaceIndex = $i
            continue
        }

        if ($linea -match "^using\s.*;") {
            if ($namespaceIndex -eq -1) {
                $usingsAntes += $linea
            } else {
                $usingsDentro += $linea
                $contenido[$i] = ""
            }
        }
    }

    if ($usingsDentro.Count -gt 0) {
        $usingsFinal = ($usingsAntes + $usingsDentro) | Select-Object -Unique
        $nuevoContenido = @()
        $nuevoContenido += $usingsFinal
        $nuevoContenido += ""
        $nuevoContenido += $contenido

        # Backup
        Copy-Item $archivo.FullName "$($archivo.FullName).bak" -Force
        $nuevoContenido | Set-Content $archivo.FullName
        Write-Host "Reparado: $($archivo.FullName)"
        $archivosReparados++
    }
}

Write-Host "`nReparación completada. Total de archivos corregidos: $archivosReparados"
