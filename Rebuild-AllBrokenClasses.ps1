param (
    [string]$SolutionRoot = "."
)

function Get-SafeNamespaceFromPath($path, $base) {
    $relative = $path.Substring($base.Length).TrimStart('\','/')
    $parts = $relative -split "[\\/]"
    $validParts = $parts | Where-Object {
        $_ -notmatch "^([a-zA-Z]:)?$" -and
        $_ -notmatch "bin|obj|Debug|Release" -and
        $_ -notmatch "\.cs$"
    }
    $cleanParts = $validParts | ForEach-Object {
        ($_ -replace "\.cs$", "") -replace "[^a-zA-Z0-9_]", ""
    }
    return "PetCarePlatform." + ($cleanParts -join ".")
}

$csFiles = Get-ChildItem -Path $SolutionRoot -Recurse -Filter *.cs
foreach ($archivo in $csFiles) {
    $lines = Get-Content $archivo.FullName -ErrorAction SilentlyContinue
    if (-not $lines) { $lines = @() }

    $text = $lines -join "`n"
    $hasNamespace = $text -match "\bnamespace\b"
    $hasClass = $text -match "\b(class|interface|struct)\b"
    $hasBraces = $text -match "\{" -and $text -match "\}"
    $isBroken = (-not $hasNamespace) -or (-not $hasClass) -or (-not $hasBraces)

    if ($isBroken) {
        $className = [System.IO.Path]::GetFileNameWithoutExtension($archivo.Name)
        $namespace = Get-SafeNamespaceFromPath $archivo.FullName $SolutionRoot
        $preserved = $lines | Where-Object {
            ($_ -match "^using\s") -or ($_ -match "^

\[") -or ($_ -match "//") -or ($_ -match "#")
        }

        $rebuild = @()
        if ($preserved -notcontains "using System;") {
            $rebuild += "using System;"
        }
        $rebuild += $preserved | Select-Object -Unique
        $rebuild += ""
        $rebuild += "namespace $namespace"
        $rebuild += "{"
        $rebuild += "    public class $className"
        $rebuild += "    {"
        $rebuild += "        // Clase reconstruida automáticamente"
        $rebuild += "    }"
        $rebuild += "}"

        Copy-Item $archivo.FullName "$($archivo.FullName).bak" -Force
        $rebuild | Set-Content $archivo.FullName -Encoding UTF8
        Write-Host "Reconstruido: $($archivo.FullName)"
    }
}
