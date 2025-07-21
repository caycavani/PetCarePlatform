param (
    [string]$SolutionRoot = "."
)

$csFiles = Get-ChildItem -Path $SolutionRoot -Recurse -Filter *.cs

foreach ($archivo in $csFiles) {
    $lines = Get-Content $archivo.FullName -ErrorAction SilentlyContinue
    if (-not $lines) { $lines = @() }

    $needsNamespace = -not ($lines | Where-Object { $_ -match "^namespace\s" })
    $needsClass = -not ($lines | Where-Object { $_ -match "^\s*(public|internal)?\s*(class|interface|struct)\s" })
    $needsBraces = ($lines -join "`n") -notmatch "\{" -or ($lines -join "`n") -notmatch "\}"

    if ($lines.Count -eq 0 -or $needsNamespace -or $needsClass -or $needsBraces) {
        $className = [System.IO.Path]::GetFileNameWithoutExtension($archivo.Name)
        $namespaceParts = $archivo.FullName -replace [regex]::Escape($SolutionRoot), "" -replace "^[\\\/]+", "" -split "[\\\/]"
        $namespace = "PetCarePlatform"
        foreach ($part in $namespaceParts) {
            if ($part -notmatch "\.cs$" -and $part -notmatch "obj|bin") {
                $namespace += "." + ($part -replace "\.cs$", "")
            }
        }

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

        # Backup y restauración
        Copy-Item $archivo.FullName "$($archivo.FullName).bak" -Force
        $newContent | Set-Content $archivo.FullName -Encoding UTF8

        Write-Host "Restaurado: $($archivo.FullName)"
    }
}

Write-Host "`nReparación completada."
