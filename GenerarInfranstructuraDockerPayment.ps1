param (
    [string]$ApiPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Payment\PetCare.Payment.Api"
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
RUN dotnet restore "PetCare.Payment.Api.csproj"
RUN dotnet publish "PetCare.Payment.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PetCare.Payment.Api.dll"]
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
    "PetCare.Payment.Api": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5300",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
"@
Set-Content -Path $launchPath -Value $launchContent -Encoding UTF8

Write-Host "`n✅ Dockerfile y launchSettings.json generados para PetCare.Payment.Api" -ForegroundColor Green