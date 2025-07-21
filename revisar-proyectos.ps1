param (
    [string]$Servicio = "Auth",
    [switch]$Limpiar
)

$basePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\$Servicio"

$projects = @{
    "Domain"        = "$basePath\PetCare.$Servicio.Domain\PetCare.$Servicio.Domain.csproj"
    "Application"   = "$basePath\PetCare.$Servicio.Application\PetCare.$Servicio.Application.csproj"
    "Infrastructure"= "$basePath\PetCare.$Servicio.Infrastructure\PetCare.$Servicio.Infrastructure.csproj"
    "Api"           = "$basePath\PetCare.$Servicio.Api\PetCare.$Servicio.Api.csproj"
}

$references = @{
    "Application"   = @("Domain")
    "Infrastructure"= @("Domain", "Application")
    "Api"           = @("Domain", "Application", "Infrastructure")
}

Write-Host ""
Write-Host "Revisión de referencias para el microservicio '$Servicio'..."

foreach ($proj in $projects.Keys) {
    $projPath = $projects[$proj]
    
    if (-not (Test-Path $projPath)) {
        Write-Warning "Proyecto '$proj' no encontrado en '$projPath'"
        continue
    }

    Write-Host ""
    Write-Host "Proyecto detectado: $proj"

    $output = dotnet list "$projPath" reference | Where-Object { $_ -match '\.csproj' }
    $currentRefs = @()

    foreach ($line in $output) {
        $ref = ($line -split '\\')[-1] -replace '\.csproj$', ''
        $currentRefs += $ref
    }

    if ($references.ContainsKey($proj)) {
        foreach ($expected in $references[$proj]) {
            $targetPath = $projects[$expected]

            if ($currentRefs -contains $expected) {
                Write-Host "'$proj' ya referencia a '$expected'"
            } else {
                if (Test-Path $targetPath) {
                    Write-Host "Agregando referencia: '$proj' → '$expected'"
                    dotnet add "$projPath" reference "$targetPath"
                } else {
                    Write-Warning "No se encontró el proyecto destino '$expected'"
                }
            }
        }
    } else {
        Write-Host "El proyecto '$proj' no tiene referencias definidas en el modelo"
    }
}

if ($Limpiar) {
    Write-Host ""
    Write-Host "Limpiando carpetas bin/obj del servicio '$Servicio'..."
    Get-ChildItem $basePath -Recurse -Include bin,obj | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "Limpieza completada."
}

Write-Host ""
Write-Host "Revisión completada sin errores."
