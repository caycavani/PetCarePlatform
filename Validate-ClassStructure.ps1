param (
    [string]$SolutionRoot = "."
)

$logFileName = "EstructuraClases_Log_" + (Get-Date -Format 'yyyyMMdd_HHmmss') + ".txt"
$logPath = [System.IO.Path]::Combine([Environment]::GetFolderPath("Desktop"), $logFileName)
$report = @()

$csFiles = Get-ChildItem -Path $SolutionRoot -Recurse -Filter *.cs

foreach ($archivo in $csFiles) {
    $lines = Get-Content $archivo.FullName
    if (-not $lines -or $lines.Count -eq 0) {
        $report += "ARCHIVO VACIO --> " + $archivo.FullName
        continue
    }

    # Buscar definicion de clase, interface o struct
    $tieneDefinicion = $false
    foreach ($line in $lines) {
        if ($line -match "^\s*(public|internal)?\s*(class|interface|struct)\s") {
            $tieneDefinicion = $true
            break
        }
    }

    if (-not $tieneDefinicion) {
        $report += "SIN DEFINICION DE CLASE --> " + $archivo.FullName
    }

    # Buscar namespace
    $tieneNamespace = $lines | Where-Object { $_ -match "^namespace\s" }
    if (-not $tieneNamespace) {
        $report += "SIN NAMESPACE --> " + $archivo.FullName
    }

    # Buscar llaves de apertura y cierre
    $textoCompleto = $lines -join "`n"
    if ($textoCompleto -notmatch "\{" -or $textoCompleto -notmatch "\}") {
        $report += "LLAVES INCOMPLETAS --> " + $archivo.FullName
    }
}

if ($report.Count -eq 0) {
    $report += "Todos los archivos tienen estructura valida."
}

$report | Set-Content -Path $logPath -Encoding UTF8
Write-Host ""
Write-Host "Analisis completado. Revisa el log generado en el escritorio:"
Write-Host "$logPath"
