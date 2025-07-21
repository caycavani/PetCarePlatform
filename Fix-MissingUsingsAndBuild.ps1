param (
    [string]$SolutionRoot = "."
)

$tipoNamespaceMap = @{
    "Guid"         = "using System;"
    "DateTime"     = "using System;"
    "Exception"    = "using System;"
    "Task"         = "using System.Threading.Tasks;"
    "Task<"        = "using System.Threading.Tasks;"
    "IEnumerable<" = "using System.Collections.Generic;"
}

$archivosCorregidos = 0
Write-Host "Corrigiendo usings..."

$csFiles = Get-ChildItem -Path $SolutionRoot -Recurse -Filter *.cs

foreach ($archivo in $csFiles) {
    $lines = Get-Content $archivo.FullName
    if (-not $lines -or $lines.Count -eq 0) { continue }

    $usingsNecesarios = @()
    foreach ($tipo in $tipoNamespaceMap.Keys) {
        if ($lines -match "\b$tipo\b") {
            $using = $tipoNamespaceMap[$tipo]
            if ($lines -notcontains $using) {
                $usingsNecesarios += $using
            }
        }
    }

    if ($usingsNecesarios.Count -eq 0) { continue }

    $namespaceIndex = ($lines | Select-String "^namespace\s").LineNumber | Select-Object -First 1
    if (-not $namespaceIndex -or $namespaceIndex -lt 1) { continue }

    # Eliminar usings que están después del namespace
    $cleanedLines = @()
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match "^using\s" -and $i -ge ($namespaceIndex - 1)) { continue }
        $cleanedLines += $lines[$i]
    }

    $insertPos = $namespaceIndex - 1
    $preBlock = if ($insertPos -gt 0) { $cleanedLines[0..($insertPos - 1)] } else { @() }
    $postBlock = $cleanedLines[$insertPos..($cleanedLines.Count - 1)]
    $finalLines = $preBlock + $usingsNecesarios + $postBlock

    $finalLines | Set-Content $archivo.FullName
    Write-Host "Usings corregidos en: $($archivo.FullName)"
    $archivosCorregidos++
}

Write-Host ""
Write-Host "Total de archivos modificados: $archivosCorregidos"

function Build-Solution {
    Write-Host ""
    Write-Host "Compilando solución..."
    $logPath = "$([Environment]::GetFolderPath('Desktop'))\BuildErrors_$(Get-Date -Format 'yyyyMMdd_HHmmss').log"

    Push-Location $SolutionRoot
    $buildResult = dotnet build 2>&1
    Pop-Location

    $buildResult | Set-Content $logPath

    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "Compilación exitosa. Log en: $logPath"
    } else {
        Write-Host ""
        Write-Host "Errores detectados durante la compilación:"
        $buildResult | Where-Object { $_ -match "error\s+[A-Z]?[0-9]{4}" } | ForEach-Object {
            Write-Host $_
        }
        Write-Host ""
        Write-Host "Log guardado en: $logPath"
    }
}

Build-Solution
