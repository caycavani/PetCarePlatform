param (
    [string]$SolutionRoot = ".",
    [string]$NamespaceFavorito = "PetCare.Auth.Application"
)

function Detect-Duplicates {
    Write-Host "Detectando archivos duplicados en diferentes espacios de nombre..." -ForegroundColor Cyan

    $archivosPorNombre = Get-ChildItem -Path $SolutionRoot -Recurse -Filter *.cs |
        Where-Object { $_.Name -ne "Program.cs" } |
        Group-Object -Property Name |
        Where-Object { $_.Count -gt 1 }

    foreach ($grupo in $archivosPorNombre) {
        $nombreArchivo = $grupo.Name
        $paths = $grupo.Group | ForEach-Object { $_.FullName }

        $namespaces = @{}
        foreach ($path in $paths) {
            $contenido = Get-Content $path
            $namespaceLine = $contenido | Select-String "namespace" | Select-Object -First 1
            if ($namespaceLine) {
                $ns = $namespaceLine.ToString().Trim() -replace "^namespace\s+", ""
                $namespaces[$path] = $ns
            } else {
                $namespaces[$path] = "SIN_NAMESPACE"
            }
        }

        $uniqueNamespaces = $namespaces.Values | Select-Object -Unique
        if ($uniqueNamespaces.Count -gt 1) {
            Write-Host "`nConflicto: '$nombreArchivo' está definido en múltiples espacios de nombre." -ForegroundColor Yellow
            $namespaces.GetEnumerator() | ForEach-Object {
                Write-Host "  → [$($_.Value)] $($_.Key)" -ForegroundColor Gray
            }

            foreach ($kv in $namespaces.GetEnumerator()) {
                if ($kv.Value -ne $NamespaceFavorito) {
                    Write-Host "Eliminando duplicado fuera del espacio '$NamespaceFavorito': $($kv.Key)" -ForegroundColor Red
                    Remove-Item $kv.Key -Force
                }
            }

            Write-Host "Se mantuvo '$nombreArchivo' en namespace '$NamespaceFavorito'" -ForegroundColor Green
        }
    }

    Write-Host ""
    Write-Host "Revisión de duplicados completada." -ForegroundColor Cyan
}

function Build-Solution {
    Write-Host "`nCompilando solución..." -ForegroundColor Cyan

    $logPath = "$([Environment]::GetFolderPath('Desktop'))\BuildErrors_$(Get-Date -Format 'yyyyMMdd_HHmmss').log"

    Push-Location $SolutionRoot
    $buildResult = dotnet build 2>&1
    Pop-Location

    $buildResult | Set-Content $logPath

    if ($LASTEXITCODE -eq 0) {
        Write-Host "`nCompilación exitosa." -ForegroundColor Green
        Write-Host "Registro guardado en: $logPath" -ForegroundColor DarkGray
    } else {
        Write-Host "`nErrores durante la compilación (también guardados en el log):" -ForegroundColor Red
        $errores = $buildResult | Where-Object { $_ -match "error\s+[A-Z]?[0-9]{4}" }

        if ($errores.Count -eq 0) {
            Write-Host "No se pudieron extraer errores detallados. Revisa el log completo en:" -ForegroundColor Yellow
        } else {
            $errores | ForEach-Object {
                $linea = $_
                $archivo = ($linea -split ":")[0]
                $detalles = ($linea -split ":") | Select-Object -Skip 1 -Join ":"
                Write-Host "`nArchivo: $archivo" -ForegroundColor Gray
                Write-Host "Error → $detalles" -ForegroundColor Yellow
            }
        }

        Write-Host "`nRegistro guardado en: $logPath" -ForegroundColor DarkGray
    }
}

# Ejecutar análisis y compilación
Detect-Duplicates
Build-Solution
