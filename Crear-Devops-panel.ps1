param (
    [string]$RutaBase = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform",
    [string]$NombreMicroservicio = "DevOpsPanel",
    [int]$Puerto = 5050
)

# 🔍 Validaciones previas
Write-Host "`n🔍 Verificando entorno..."

$dotnetVersion = & dotnet --version
if ([version]$dotnetVersion -lt [version]"8.0.0") {
    Write-Host "❌ Se requiere .NET 8 o superior. Detectado: $dotnetVersion" -ForegroundColor Red
    exit 1
} else {
    Write-Host "✔️ .NET detectado: $dotnetVersion"
}


$carpetaActual = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
$solucionEsperada = Join-Path $carpetaActual "PetCarePlatform.sln"
if (-not (Test-Path $solucionEsperada)) {
    Write-Host "❌ Este script debe ejecutarse desde la raíz del backend donde está PetCarePlatform.sln" -ForegroundColor Red
    exit 1
} else {
    Write-Host "✔️ Ubicación válida: $carpetaActual"
}

try {
    $dockerOk = docker --version
    Write-Host "✔️ Docker detectado: $dockerOk"
}
catch {
    Write-Host "❌ Docker no está instalado o no está en PATH." -ForegroundColor Red
    exit 1
}

# 📁 Estructura
$carpeta = Join-Path $RutaBase $NombreMicroservicio
$proyecto = Join-Path $carpeta "PetCare.$NombreMicroservicio.Api"
New-Item -ItemType Directory -Path $proyecto -Force | Out-Null

dotnet new webapi -o "$proyecto" --framework net8.0 --no-https --force

# 👨‍💻 Controlador
$controllerPath = Join-Path $proyecto "Controllers"
New-Item -ItemType Directory -Path $controllerPath -Force | Out-Null

$controller = @"
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PetCare.$NombreMicroservicio.Api.Controllers
{
    [ApiController]
    [Route("api/docker")]
    public class DevOpsController : ControllerBase
    {
        [HttpPost("up")]
        public IActionResult Up() => Ejecutar("docker-compose up -d");

        [HttpPost("down")]
        public IActionResult Down() => Ejecutar("docker-compose down");

        [HttpGet("ps")]
        public IActionResult Ps() => Ejecutar("docker ps");

        private IActionResult Ejecutar(string comando)
        {
            try
            {
                var psi = new ProcessStartInfo("cmd.exe", "/c " + comando)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                return Ok(output);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
"@
Set-Content -Path (Join-Path $controllerPath "DevOpsController.cs") -Value $controller -Encoding UTF8

# 🌐 landing.html
$wwwroot = Join-Path $proyecto "wwwroot"
New-Item -ItemType Directory -Path $wwwroot -Force | Out-Null
$htmlPath = Join-Path $wwwroot "landing.html"

$html = @"
<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="UTF-8">
<title>PetCare Cockpit</title>
<style>
    body { font-family: sans-serif; margin: 40px; background: #f5f5f5; }
    h1 { color: #333; }
    button { margin: 10px; padding: 12px 20px; font-size: 16px; }
    pre { background: #fff; padding: 1em; border: 1px solid #ccc; }
</style>
</head>
<body>
<h1>🐾 Panel de Control - PetCare</h1>
<button onclick="ejecutar('up')">⏫ docker-compose up</button>
<button onclick="ejecutar('ps')">📋 docker ps</button>
<button onclick="ejecutar('down')">⏬ docker-compose down</button>
<pre id="salida">Esperando comandos...</pre>
<script>
function ejecutar(cmd) {
    fetch('/api/docker/' + cmd, { method: cmd === 'ps' ? 'GET' : 'POST' })
    .then(r => r.text())
    .then(t => document.getElementById('salida').textContent = t)
    .catch(e => alert(e));
}
</script>
</body>
</html>
"@
Set-Content -Path $htmlPath -Value $html -Encoding UTF8

# 🔧 Ajustar Program.cs
$program = Join-Path $proyecto "Program.cs"
(gc $program) -replace 'app\.UseHttpsRedirection\(\);', '' | Set-Content -Encoding UTF8 $program

# 🚀 launchSettings.json
$props = Join-Path $proyecto "Properties"
$launch = @{
    profiles = @{
        "PetCare.DevOpsPanel.Api" = @{
            commandName = "Project"
            applicationUrl = "http://localhost:$Puerto"
            launchBrowser = $true
            launchUrl = "landing.html"
            environmentVariables = @{ ASPNETCORE_ENVIRONMENT = "Development" }
        }
    }
}
$launch | ConvertTo-Json -Depth 5 | Set-Content -Encoding UTF8 (Join-Path $props "launchSettings.json")

# 📌 Agregar al .sln
$solucion = Join-Path $RutaBase "PetCarePlatform.sln"
dotnet sln "$solucion" add "$proyecto"

Write-Host "`n✅ Microservicio 'PetCare.DevOpsPanel.Api' creado e integrado al proyecto."
Write-Host "   Ejecuta en Visual Studio o usa: dotnet run --project $proyecto"
Write-Host "   Abre: http://localhost:$Puerto/landing.html"