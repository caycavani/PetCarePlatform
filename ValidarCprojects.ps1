# Buscar todos los archivos .csproj en subdirectorios
$csprojFiles = Get-ChildItem -Recurse -Filter *.csproj
$errores = 0

Write-Host ""
Write-Host "Validando archivos .csproj, referencias y tipo de salida..." -ForegroundColor Cyan

foreach ($file in $csprojFiles) {
    $path = $file.FullName
    $content = Get-Content $path
    $dir = Split-Path $path
    $nombre = $file.Name

    Write-Host ""
    Write-Host "Revisando: $nombre" -ForegroundColor White

    # Validar TargetFramework
    $framework = ($content | Select-String "<TargetFramework>").ToString()
    if (-not $framework) {
        Write-Host "  ERROR: No se encontró <TargetFramework> en $nombre" -ForegroundColor Red
        $errores++
    } elseif ($framework -notmatch "net8\.0") {
        Write-Host "  ADVERTENCIA: TargetFramework no es net8.0 → $framework" -ForegroundColor Yellow
    } else {
        Write-Host "  OK: TargetFramework → $framework" -ForegroundColor Green
    }

    # Validar OutputType
    $outputTypeLine = ($content | Select-String "<OutputType>").ToString()
    $expectedOutput = ""

    if ($nombre -like "*.Api.csproj") {
        $expectedOutput = "Exe"
    } elseif ($nombre -like "*.Application.csproj" -or $nombre -like "*.Domain.csproj") {
        $expectedOutput = "Library"
    }

    if ($expectedOutput -ne "") {
        if (-not $outputTypeLine) {
            Write-Host "  ERROR: Falta <OutputType> en $nombre (esperado: $expectedOutput)" -ForegroundColor Red
            $errores++
        } elseif ($outputTypeLine -notmatch $expectedOutput) {
            Write-Host "  ERROR: OutputType incorrecto. Esperado: $expectedOutput → Encontrado: $outputTypeLine" -ForegroundColor Red
            $errores++
        } else {
            Write-Host "  OK: OutputType → $expectedOutput" -ForegroundColor Green
        }
    }

    # Validar rutas de ProjectReference
    $references = $content | Select-String "<ProjectReference Include="

    foreach ($ref in $references) {
        $match = [regex]::Match($ref, 'Include="([^"]+)"')
        if ($match.Success) {
            $relativePath = $match.Groups[1].Value
            $absolutePath = Resolve-Path -Path (Join-Path $dir $relativePath) -ErrorAction SilentlyContinue

            if (-not $absolutePath) {
                Write-Host "  ERROR: Referencia inválida → $relativePath" -ForegroundColor Red
                $errores++
            } else {
                Write-Host "  OK: Referencia válida → $relativePath" -ForegroundColor Green
            }
        }
    }
}

Write-Host ""
if ($errores -eq 0) {
    Write-Host "Todos los archivos .csproj, referencias y tipos de salida están correctos." -ForegroundColor Green
} else {
    Write-Host "Se detectaron $errores problema(s) en archivos .csproj." -ForegroundColor Yellow
}