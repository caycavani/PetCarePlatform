# Ruta del archivo de servicio
$basePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Booking\PetCare.Booking.Api"
$servicePath = Join-Path $basePath "Services\MascotaValidatorService.cs"
$programFile = Join-Path $basePath "Program.cs"

# 1️⃣ Crear archivo MascotaValidatorService.cs si no existe
if (-not (Test-Path $servicePath)) {
    New-Item -ItemType Directory -Force -Path (Split-Path $servicePath) | Out-Null

    $contenido = @"
using System.Net.Http;
using System.Threading.Tasks;

namespace PetCare.Booking.Api.Services
{
    public class MascotaValidatorService
    {
        private readonly HttpClient _httpClient;

        public MascotaValidatorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ValidarMascotaExiste(int mascotaId)
        {
            var response = await _httpClient.GetAsync($"/api/mascotas/{mascotaId}");
            return response.IsSuccessStatusCode;
        }
    }
}
"@

    $contenido | Set-Content -Encoding UTF8 $servicePath
    Write-Output "MascotaValidatorService creado exitosamente."
} else {
    Write-Output "El archivo MascotaValidatorService.cs ya existe."
}

# 2️⃣ Registrar HttpClient en Program.cs
if (Test-Path $programFile) {
    $programContent = Get-Content $programFile

    $builderIndex = $null
    for ($i = 0; $i -lt $programContent.Count; $i++) {
        if ($programContent[$i] -match "WebApplication\.CreateBuilder") {
            $builderIndex = $i
            break
        }
    }

    if ($null -eq $builderIndex) {
        Write-Warning "No se encontró WebApplication.CreateBuilder en Program.cs"
        return
    }

    # Inyección del servicio
    $httpClientInjection = @'
builder.Services.AddHttpClient<MascotaValidatorService>(client =>
{
    client.BaseAddress = new Uri("http://pets-api");
});
'@

    $updatedProgram = @()

    # Insertar usings si faltan
    if ($programContent -notmatch "using PetCare.Booking.Api.Services;") {
        $updatedProgram += "using PetCare.Booking.Api.Services;"
    }

    $updatedProgram += $programContent[0..$builderIndex]
    $updatedProgram += $httpClientInjection
    $updatedProgram += $programContent[($builderIndex + 1)..($programContent.Count - 1)]

    $updatedProgram | Set-Content -Encoding UTF8 $programFile
    Write-Output "MascotaValidatorService registrado correctamente en Program.cs"
} else {
    Write-Warning "Program.cs no encontrado en Booking.Api"
}