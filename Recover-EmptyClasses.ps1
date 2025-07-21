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
            ($_ -match "^using\s") -or ($_ -match "