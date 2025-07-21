<#
.SYNOPSIS
  Verifica y agrega configuración Swagger en Program.cs si no está presente.
#>

function Write-Log {
    param($msg, $level = 'INFO')
    $colors = @{ INFO = 'White'; SUCCESS = 'Green'; WARN = 'Yellow'; ERROR = 'Red' }
    Write-Host "[$level] $msg" -ForegroundColor $colors[$level]
}

# 1. Buscar Program.cs
Write-Log "Buscando archivo Program.cs..." INFO
$programFile = Get-ChildItem -Recurse -Filter Program.cs | Where-Object { $_.FullName -like "*Api*" } | Select-Object -First 1

if (-not $programFile) {
    Write-Log "No se encontró Program.cs en un proyecto API." ERROR
    exit 1
}

Write-Log "Archivo encontrado: $($programFile.FullName)" INFO
$content = Get-Content $programFile.FullName -Raw
$modified = $false

# 2. Verificar y agregar AddSwaggerGen
if ($content -notmatch 'AddSwaggerGen\s*\(') {
    $content = $content -replace '(builder\.Services\.AddControllers\s*\(\s*\)\s*;)', "`$1`r`n    builder.Services.AddSwaggerGen();"
    Write-Log "Agregado: builder.Services.AddSwaggerGen();" SUCCESS
    $modified = $true
}

# 3. Verificar y agregar UseSwagger y UseSwaggerUI
if ($content -notmatch 'UseSwagger\s*\(\s*\)' -or $content -notmatch 'UseSwaggerUI\s*\(\s*\)') {
    $content = $content -replace '(var app = builder\.Build\s*\(\s*\)\s*;)', "`$1`r`napp.UseSwagger();`r`napp.UseSwaggerUI();"
    Write-Log "Agregado: app.UseSwagger(); y app.UseSwaggerUI();" SUCCESS
    $modified = $true
}

# 4. Guardar si hubo cambios
if ($modified) {
    Set-Content $programFile.FullName -Value $content -Encoding UTF8
    Write-Log "Program.cs actualizado correctamente." SUCCESS
} else {
    Write-Log "Program.cs ya contiene la configuración Swagger." INFO
}