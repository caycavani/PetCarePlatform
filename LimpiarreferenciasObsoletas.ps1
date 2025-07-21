<#
.SYNOPSIS
  Valida que cada COPY en tus Dockerfiles apunte a un archivo o carpeta existente.
#>
$errors = 0

Get-ChildItem -Path $PSScriptRoot -Recurse -Filter Dockerfile |
ForEach-Object {
  $dfPath = $_.FullName
  $lines  = Get-Content $dfPath
  for ($i=0; $i -lt $lines.Count; $i++) {
    $l = $lines[$i].Trim()
    if ($l -match '^COPY\s+(.+?)\s+') {
      # ruta relativa al Dockerfile
      $srcRaw = $matches[1]
      $src     = $srcRaw -replace '^(\.\/|\/)',''   # quitar ./ o /
      $fullSrc = Join-Path -Path (Split-Path $dfPath) -ChildPath $src

      if (-not (Test-Path $fullSrc)) {
        Write-Host "ERROR: En '$dfPath' (línea $($i+1)) -> '$srcRaw' NO EXISTE." -ForegroundColor Red
        $errors++
      }
    }
  }
}

if ($errors -gt 0) {
  Write-Host "`nSe detectaron $errors error(es) en COPYs de Dockerfile(s)." -ForegroundColor Red
  exit 1
}
else {
  Write-Host "✔ Todos los COPYs apuntan a rutas válidas." -ForegroundColor Green
  exit 0
}