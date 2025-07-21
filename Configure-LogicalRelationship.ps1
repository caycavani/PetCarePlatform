# Ruta base del backend
$basePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform"

# Rutas clave
$bookingInfra = Join-Path $basePath "Booking\PetCare.Booking.Infrastructure"
$bookingApi = Join-Path $basePath "Booking\PetCare.Booking.Api"
$reservaFile = Join-Path $bookingInfra "Persistence\BookingDbContext.cs"
$programFile = Join-Path $bookingApi "Program.cs"
$serviceFile = Join-Path $bookingApi "Services\MascotaValidatorService.cs"

# 1️⃣ Agregar MascotaId a la entidad Reserva
if (Test-Path $reservaFile) {
    $content = Get-Content $reservaFile
    if ($content -notmatch "MascotaId") {
        $updated = $content -replace "(public string Estado.*)", "`$1`n`n        // Relación lógica con Mascota (Pets)`n        public int MascotaId { get; set; }`n        public string? NombreMascota { get; set; }"
        $updated | Set-Content -Encoding UTF8 $reservaFile
        Write-Host "🧩 Campo MascotaId agregado a Reserva"
    } else {
        Write-Host "✅ MascotaId ya está presente en Reserva"
    }
} else {
    Write-Warning "❌ No se encontró BookingDbContext.cs"
}

# 2️⃣ Crear servicio de validación HTTP
if (-not (Test-Path $serviceFile)) {
    New-Item -ItemType Directory -Force -Path (Split-Path $serviceFile) | Out-Null
    @"
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
"@ | Set-Content -Encoding UTF8 $serviceFile
    Write-Host "🔧 MascotaValidatorService creado"
} else {
    Write-Host "✅ MascotaValidatorService ya existe"
}

# 3️⃣ Registrar HttpClient y el servicio en Program.cs
if (Test-Path $programFile) {
    $programContent = Get-Content $programFile

    if ($programContent -notmatch "AddHttpClient<MascotaValidatorService>") {
        $builderLineIndex = -1
        for ($i = 0; $i -lt $programContent.Count; $i++) {
            if ($programContent[$i] -match "WebApplication\.CreateBuilder") {
                $builderLineIndex = $i
                break
            }
        }

        if ($builderLineIndex -eq -1) {
            Write-Warning "⚠️ No se encontró el builder en Program.cs"
        } else {
            $injection = @'
builder.Services.AddHttpClient<MascotaValidatorService>(client =>
{
    client.BaseAddress = new Uri("http://pets-api");
});
'@

            $updatedContent = @()
            $updatedContent += $programContent[0..$builderLineIndex]
            $updatedContent += $injection
            $updatedContent += $programContent[($builderLineIndex + 1)..($programContent.Length - 1)]

            # Agregar using si no existe
            if ($programContent -notmatch "using PetCare.Booking.Api.Services;") {
                $updatedContent.Insert(0, 'using PetCare.Booking.Api.Services;')
            }

            $updatedContent | Set-Content -Encoding UTF8 $programFile
            Write-Host "HttpClient y servicio registrados en Program.cs"
        }
    } else {
        Write-Host "HttpClient ya está registrado"
    }
} else {
    Write-Warning "No se encontró Program.cs en Booking.Api"
}