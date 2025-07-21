$servicios = @("Auth", "Booking", "Notification", "Payment", "Pets", "Review")
$basePath = Get-Location
$objetivo = "net8.0"
$errores = 0
$corregidos = 0

Write-Host "`nCorrigiendo errores críticos en microservicios..."

foreach ($servicio in $servicios) {
    $apiPath = Join-Path $basePath "$servicio\PetCare.$servicio.Api"
    $csprojPath = Join-Path $apiPath "PetCare.$servicio.Api.csproj"
    $programPath = Join-Path $apiPath "Program.cs"

    # 1. Corregir .csproj si es necesario
    if (Test-Path $csprojPath) {
        $contenido = Get-Content $csprojPath -Raw
        $modificado = $false

        if ($contenido -notmatch "Microsoft.NET.Sdk.Web") {
            $contenido = $contenido -replace "Microsoft.NET.Sdk", "Microsoft.NET.Sdk.Web"
            $modificado = $true
        }

        if ($contenido -notmatch "<TargetFramework>net8.0</TargetFramework>") {
            $contenido = $contenido -replace "<TargetFramework>.*?</TargetFramework>", "<TargetFramework>net8.0</TargetFramework>"
            $modificado = $true
        }

        if ($modificado) {
            $contenido | Out-File -FilePath $csprojPath -Encoding UTF8
            Write-Host "SDK y TargetFramework corregidos en: $csprojPath"
            $corregidos++
        }
    }

    # 2. Crear Program.cs si no existe o está vacío
    if (-not (Test-Path $programPath) -or (Get-Content $programPath | Where-Object { $_.Trim() -ne "" }).Count -eq 0) {
        $programContent = @'
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "API funcionando");

app.Run();
'@
        $programContent | Out-File -FilePath $programPath -Encoding UTF8
        Write-Host "Program.cs creado para $servicio"
        $corregidos++
    }
}

# 3. Analizar posibles errores CS0118 en Domain\Repositories
Write-Host "`nAnalizando posibles errores CS0118 en Domain\Repositories..."

foreach ($servicio in $servicios) {
    $repoPath = Join-Path $basePath "$servicio\PetCare.$servicio.Domain\Repositories"
    if (Test-Path $repoPath) {
        $archivos = Get-ChildItem -Path $repoPath -Filter *.cs
        foreach ($archivo in $archivos) {
            $lineas = Get-Content $archivo.FullName
            foreach ($linea in $lineas) {
                if ($linea -match "\b$servicio\b\s*=" -or $linea -match "\b$servicio\b\(") {
                    Write-Host "Posible mal uso de '$servicio' como tipo en: $($archivo.FullName)"
                    Write-Host "  Sugerencia: asegúrate de que '$servicio' sea una clase o usa un nombre calificado como '$servicio.Models.$servicio'"
                    $errores++
                }
            }
        }
    }
}

Write-Host "`nResumen:"
Write-Host "Correcciones aplicadas: $corregidos"
Write-Host "Posibles errores de código detectados: $errores"