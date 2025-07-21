param (
    [string]$BasePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Auth",
    [string]$SolutionName = "PetCarePlatform",
    [string]$ServiceName = "Auth"
)

$servicePrefix = "PetCare.$ServiceName"
$folders = @("Api", "Application", "Domain", "Infrastructure")

Write-Host "`n[+] Generando microservicio '$ServiceName'..." -ForegroundColor Cyan

# Crear proyectos
foreach ($folder in $folders) {
    $projectPath = Join-Path $BasePath "$servicePrefix.$folder"
    dotnet new classlib -n "$servicePrefix.$folder" -o $projectPath | Out-Null
    Write-Host "[OK] Proyecto creado: $servicePrefix.$folder"
}

# Crear proyecto API como Web API
$apiPath = Join-Path $BasePath "$servicePrefix.Api"
Remove-Item "$apiPath\Class1.cs" -ErrorAction SilentlyContinue
dotnet new webapi -n "$servicePrefix.Api" -o $apiPath --no-https | Out-Null
Write-Host "[OK] Proyecto API generado: $servicePrefix.Api"

# Ruta de la solución
$solutionPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\PetCarePlatform.sln"

# Verificar que la solución exista
if (-not (Test-Path $solutionPath)) {
    Write-Host "[ERROR] La solución '$SolutionName.sln' no existe en $BasePath" -ForegroundColor Red
    exit 1
}

# Agregar proyectos a la solución
foreach ($folder in $folders) {
    $projPath = Join-Path $BasePath "$servicePrefix.$folder\$servicePrefix.$folder.csproj"
    dotnet sln $solutionPath add $projPath | Out-Null
    Write-Host "[OK] Agregado a solución: $servicePrefix.$folder"
}

$apiProj = Join-Path $BasePath "$servicePrefix.Api\$servicePrefix.Api.csproj"
dotnet sln $solutionPath add $apiProj | Out-Null
Write-Host "[OK] Agregado a solución: $servicePrefix.Api"

# Agregar referencias entre proyectos
dotnet add "$apiPath" reference `
    (Join-Path $BasePath "$servicePrefix.Application\$servicePrefix.Application.csproj") `
    (Join-Path $BasePath "$servicePrefix.Domain\$servicePrefix.Domain.csproj") | Out-Null

dotnet add "$BasePath\$servicePrefix.Application" reference `
    (Join-Path $BasePath "$servicePrefix.Domain\$servicePrefix.Domain.csproj") | Out-Null

dotnet add "$BasePath\$servicePrefix.Infrastructure" reference `
    (Join-Path $BasePath "$servicePrefix.Domain\$servicePrefix.Domain.csproj") | Out-Null

Write-Host "`n[OK] Microservicio '$ServiceName' generado y agregado a la solución '$SolutionName.sln'." -ForegroundColor Green