param (
    [string]$Nombre
)

Start-Process powershell -ArgumentList "-ExecutionPolicy Bypass -File `"$PSScriptRoot\ReiniciarServicio.ps1`" -Servicio `"$Nombre`"" -Verb RunAs