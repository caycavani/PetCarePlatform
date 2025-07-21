param (
    [string]$SolutionRoot = "."
)

function Get-SafeNamespaceFromPath($path, $base) {
    $relative = $path.Replace($base, "").TrimStart('\')
    $parts = $relative -split "[\\/]"
    $cleanParts = @()
    foreach ($part in $parts) {
        if ($part -match "\.cs$") { continue }
        if ($part -match "obj|bin") { continue }
        $cleanParts += ($part -replace "\.cs$", "") -replace "\W", ""
    }
    return "PetCarePlatform." + ($cleanParts -join ".")
}

$csFiles = Get-ChildItem -Path $SolutionRoot -Recurse -Filter *.cs

foreach ($archivo in $csFiles) {
    $lines = Get-Content $archivo.FullName -ErrorAction SilentlyContinue
    if (-not $lines) { $lines = @() }

    $fileIsEmpty = $lines.Count -eq 0
    $hasClass = $lines | Where-Object { $_ -match "^\s*(public|internal)?\s*(class|interface|struct)\s" }
    $hasNamespace = $lines | Where-Object { $_ -match "^namespace\s" }
    $textBlock = $lines -join "`n"
    $hasBraces = $textBlock -match "\{" -and $textBlock -match "\}"

    if ($fileIsEmpty -or -not $hasClass -or -not $hasNamespace -or -not $hasBraces) {
        $className = [System.IO.Path]::GetFileNameWithoutExtension($archivo.Name)
        $namespace = Get-SafeNamespaceFromPath $archivo.FullName $SolutionRoot

        $newContent = @()
        $newContent += "using System;"
        $newContent += ""
        $newContent += "namespace $namespace"
        $newContent += "{"
        $newContent += "    public class $className"
        $newContent += "    {"
        $newContent += "        // Archivo restaurado automáticamente"
        $newContent += "    }"
        $newContent += "}"

        Copy-Item $archivo.FullName "$($archivo.FullName).bak" -Force
        $newContent | Set-Content -Path $archivo.FullName -Encoding UTF8
        Write-Host "Restaurado: $($archivo.FullName)"
    }
}
