# Buscar todos los archivos .csproj en subdirectorios
$csprojFiles = Get-ChildItem -Recurse -Filter *.csproj
$errores = 0
$modificados = 0

Write-Host ""
Write-Host "Validando y corrigiendo archivos .csproj..." -ForegroundColor Cyan

foreach ($file in $csprojFiles) {
    $path = $file.FullName
    $lines = Get-Content $path
    $dir = Split-Path $path
    $nombre = $file.Name

    Write-Host ""
    Write-Host "Revisando: $nombre" -ForegroundColor White

    # Validar TargetFramework
    $frameworkLine = $lines | Select-String "<TargetFramework>"
    if (-not $frameworkLine) {
        Write-Host "  ERROR: No se encontró <TargetFramework>" -ForegroundColor Red
        $errores++
    } elseif ($frameworkLine -notmatch "net8\.0") {
        Write-Host "  ADVERTENCIA: TargetFramework no es net8.0 → $($frameworkLine.Line)" -ForegroundColor Yellow
    } else {
        Write-Host "  OK: TargetFramework → $($frameworkLine.Line)" -ForegroundColor Green
    }

    # Determinar tipo esperado
    $expectedOutput = ""
    if ($nombre -like "*.Api.csproj") {
        $expectedOutput = "Exe"
    } elseif ($nombre -like "*.Application.csproj" -or $nombre -like "*.Domain.csproj") {
        $expectedOutput = "Library"
    }

    # Validar OutputType
    $outputTypeLine = $lines | Select-String "<OutputType>"
    if ($expectedOutput -ne "") {
        if (-not $outputTypeLine) {
            Write-Host "  FALTA <OutputType> (esperado: $expectedOutput)" -ForegroundColor Yellow

            # Insertar OutputType después de TargetFramework
            $newLines = @()
            $inserted = $false

            foreach ($line in $lines) {
                $newLines += $line
                if (-not $inserted -and $line -match "<TargetFramework>") {
                    $indentMatch = [regex]::Match($line, "^\s*")
                    $indent = if ($indentMatch.Success) { $indentMatch.Value } else { "  " }
                    $newLines += "$indent<OutputType>$expectedOutput</OutputType>"
                    $inserted = $true
                }
            }

            if ($inserted) {
                Set-Content -Path $path -Value $newLines
                Write-Host "  MODIFICADO: Se agregó <OutputType>$expectedOutput</OutputType>" -ForegroundColor Yellow
                $modificados++
            }
        } elseif ($outputTypeLine -notmatch $expectedOutput) {
            Write-Host "  ERROR: OutputType incorrecto. Esperado: $expectedOutput → Encontrado: $($outputTypeLine.Line)" -ForegroundColor Red
            $errores++
        } else {
            Write-Host "  OK: OutputType → $expectedOutput" -ForegroundColor Green
        }
    }

    # Validar rutas de ProjectReference
    $references = $lines | Select-String "<ProjectReference Include="
    foreach ($ref in $references) {
        $match = [regex]::Match($ref.Line, 'Include="([^"]+)"')
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
if ($errores -eq 0 -and $modificados -eq 0) {
    Write-Host "Todos los archivos .csproj están correctos. Nada que corregir." -ForegroundColor Green
} elseif ($errores -eq 0) {
    Write-Host "$modificados archivo(s) .csproj fueron actualizados con <OutputType>." -ForegroundColor Yellow
} else {
    Write-Host "Se detectaron $errores problema(s) y se corrigieron $modificados archivo(s)." -ForegroundColor Red
}