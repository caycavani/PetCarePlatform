param (
    [string]$SolutionRoot = "."
)

function Get-SafeNamespaceFromPath($path, $base) {
    $relative = $path.Replace($base, "").TrimStart('\')
    $parts = $relative -split "[\\/]"
    $valid = $parts | Where-Object { $_ -notmatch "bin|obj" -and $_ -notmatch "\.cs$" }
    return "PetCarePlatform." + ($valid -join ".")
}

$csFiles = Get-ChildItem -Path $SolutionRoot -Recurse -Filter *.cs

foreach ($archivo in $csFiles) {
    $lines = Get-Content $archivo.FullName -ErrorAction SilentlyContinue
    if (-not $lines) { $lines = @() }

    $hasClass = $lines | Where-Object { $_ -match "^\s*(public|internal)?\s*(class|interface|struct)\s" }
    $hasNamespace = $lines | Where-Object { $_ -match "^namespace\s" }
    $textBlock = $lines -join "`n"
    $hasOpenBrace = $textBlock -match "\{"
    $hasCloseBrace = $textBlock -match "\}"

    $isIncomplete = (-not $hasClass) -or (-not $hasNamespace) -or (-not $hasOpenBrace) -or (-not $hasCloseBrace)

    if ($isIncomplete) {
        $className = [System.IO.Path]::GetFileNameWithoutExtension($archivo.Name)
        $namespace = Get-SafeNamespaceFromPath $archivo.FullName $SolutionRoot

        $preservedLines = $lines | Where-Object {
            ($_ -match "^using\s") -or ($_ -match "^

\[") -or ($_ -match "//") -or ($_ -match "#")
        }

        $newContent = @()
        $newContent += $preservedLines | Select-Object -Unique
        $newContent += ""
        $newContent += "namespace $namespace"
        $newContent += "{"
        $newContent += "    public class $className"
        $newContent += "    {"
        $newContent += "        // Archivo reconstruido automáticamente"
        $newContent += "    }"
        $newContent += "}"

        Copy-Item $archivo.FullName "$($archivo.FullName).bak" -Force
        $newContent | Set-Content $archivo.FullName -Encoding UTF8

        Write-Host "Reconstruido: $($archivo.FullName)"
    }
}
