$projectRoot = Get-Location
$logDir = Join-Path $projectRoot "logs"
if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }

$csFiles = Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch "\\obj\\" -and $_.FullName -notmatch "\\bin\\" }

$summary = @()

foreach ($file in $csFiles) {
    $path = $file.FullName
    $filename = $file.Name
    $logFile = Join-Path $logDir "$filename.log"
    $lines = Get-Content $path -ErrorAction SilentlyContinue
    if (-not $lines) { continue }

    # --- Correcciones estructurales ---
    $usings = $lines | Where-Object { $_ -match '^\s*using\s' }
    $lines = $lines | Where-Object { $_ -notmatch '^\s*using\s' }
    $lines = $usings + "" + $lines

    $lines = $lines | Where-Object { $_.Trim() -ne 'public' }

    $openBraces = ($lines -join "`n") -split '{' | Measure-Object | Select-Object -ExpandProperty Count
    $closeBraces = ($lines -join "`n") -split '}' | Measure-Object | Select-Object -ExpandProperty Count
    if ($openBraces -gt $closeBraces) {
        $lines += '}' * ($openBraces - $closeBraces)
    }

    $openParens = ($lines -join "`n") -split '\(' | Measure-Object | Select-Object -ExpandProperty Count
    $closeParens = ($lines -join "`n") -split '\)' | Measure-Object | Select-Object -ExpandProperty Count
    if ($openParens -gt $closeParens) {
        $lines[-1] += ')' * ($openParens - $closeParens)
    }

    Set-Content $path -Value $lines -Encoding UTF8
    Add-Content $logFile "Estructura corregida"

    # --- Roslynator ---
    $roslynatorOutput = roslynator fix "$path" --verbosity diagnostic 2>&1
    $roslynatorFixed = ($roslynatorOutput | Where-Object { $_ -match 'Fixed diagnostic' }).Count
    $roslynatorUnfixed = ($roslynatorOutput | Where-Object { $_ -match 'Unfixed diagnostic' }).Count
    $roslynatorUnfixable = ($roslynatorOutput | Where-Object { $_ -match 'Unfixable diagnostic' }).Count

    Add-Content $logFile "`nRoslynator:"
    Add-Content $logFile "  Fixed: $roslynatorFixed"
    Add-Content $logFile "  Unfixed: $roslynatorUnfixed"
    Add-Content $logFile "  Unfixable: $roslynatorUnfixable"

    $summary += [PSCustomObject]@{
        Archivo     = $filename
        Roslynator  = "$roslynatorFixed fix / $roslynatorUnfixed unfixed / $roslynatorUnfixable unfixable"
    }
}

# --- Compilacion global ---
$solutionFile = Get-ChildItem -Recurse -Filter *.sln | Select-Object -First 1
if (-not $solutionFile) {
    Write-Host "No se encontro archivo .sln. Abortando." -ForegroundColor Red
    exit 1
}

Write-Host "`nCompilando solucion completa..." -ForegroundColor Cyan
$buildOutput = dotnet build "$($solutionFile.FullName)" --nologo --verbosity minimal 2>&1

$syntaxErrors = @()
$semanticErrors = @()
$otherErrors = @()

foreach ($line in $buildOutput) {
    if ($line -match 'CS1\d{3}|CS8\d{3}') {
        $syntaxErrors += $line
    } elseif ($line -match 'CS0\d{3}|CS2\d{3}|CS5\d{3}') {
        $semanticErrors += $line
    } elseif ($line -match 'error') {
        $otherErrors += $line
    }
}

$buildLog = Join-Path $logDir "BuildErrors.log"
Add-Content -Path $buildLog -Value "===== ERRORES DE SINTAXIS ====="
$syntaxErrors | ForEach-Object { Add-Content -Path $buildLog -Value $_ }

Add-Content -Path $buildLog -Value "`n===== ERRORES SEMANTICOS ====="
$semanticErrors | ForEach-Object { Add-Content -Path $buildLog -Value $_ }

Add-Content -Path $buildLog -Value "`n===== OTROS ERRORES ====="
$otherErrors | ForEach-Object { Add-Content -Path $buildLog -Value $_ }

Write-Host "`nResumen de compilacion:"
Write-Host "  Sintaxis: $($syntaxErrors.Count)"
Write-Host "  Semanticos: $($semanticErrors.Count)"
Write-Host "  Otros: $($otherErrors.Count)"
Write-Host "  - $buildLog (errores de compilacion)" -ForegroundColor Yellow

# --- Resumen general ---
$summary | Format-Table | Out-String | Set-Content -Path (Join-Path $logDir "ResumenGeneral.log")
Write-Host "`nAnalisis completo. Revisa la carpeta 'logs' para ver los resultados." -ForegroundColor Cyan