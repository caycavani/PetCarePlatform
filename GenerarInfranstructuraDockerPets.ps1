param (
    [string]$ApiPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Pets\PetCare.Pets.Api"
)

# Dockerfile
$dockerFile = Join-Path $ApiPath "Dockerfile"
$dockerContent = @"
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "PetCare.Pets.Api.csproj"
RUN dotnet publish "PetCare.Pets.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PetCare.Pets.Api.dll"]
"@
Set-Content -Path $dockerFile -Value $dockerContent -Encoding UTF8

# launchSettings.json
$launchPath = Join-Path $ApiPath "Properties\launchSettings.json"
$launchDir = Split-Path $launchPath
if (-not (Test-Path $launchDir)) {
    New-Item -ItemType Directory -Path $launchDir -Force | Out-Null
}
$launchContent = @"
{
  "profiles": {
    "PetCare.Pets.Api": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5100",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
"@
Set-Content -Path $launchPath -Value $launchContent -Encoding UTF8

Write-Host "`n✅ Dockerfile y launchSettings.json generados para PetCare.Pets.Api" -ForegroundColor Green