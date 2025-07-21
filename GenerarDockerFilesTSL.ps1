$microservicios = @(
    @{ Nombre = "Booking"; Ruta = "Booking"; Proyecto = "PetCare.Booking" },
    @{ Nombre = "Notification"; Ruta = "Notification"; Proyecto = "PetCare.Notification" },
    @{ Nombre = "Payment"; Ruta = "Payment"; Proyecto = "PetCare.Payment" },
    @{ Nombre = "Review"; Ruta = "Review"; Proyecto = "PetCare.Review" },
    @{ Nombre = "Pets"; Ruta = "Pets"; Proyecto = "PetCare.Pets" }
)

foreach ($svc in $microservicios) {
    $nombre = $svc.Nombre
    $ruta = $svc.Ruta
    $proyecto = $svc.Proyecto
    $apiPath = Join-Path $ruta "$proyecto.Api"
    $domainPath = Join-Path $ruta "$proyecto.Domain"
    $dockerfilePath = Join-Path $apiPath "Dockerfile"

    $contenido = @"
# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHANDLER=0
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV NUGET_XMLDOC_MODE=skip

WORKDIR /src

COPY ./$ruta/$proyecto.Api/$proyecto.Api.csproj ./$ruta/$proyecto.Api/
COPY ./$ruta/$proyecto.Domain/$proyecto.Domain.csproj ./$ruta/$proyecto.Domain/

RUN dotnet restore ./$ruta/$proyecto.Api/$proyecto.Api.csproj

COPY . .

WORKDIR /src/$ruta/$proyecto.Api
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "$proyecto.Api.dll"]
"@

    if (-not (Test-Path $apiPath)) {
        Write-Host "❌ No se encontró la ruta: $apiPath"
        continue
    }

    $contenido | Out-File -FilePath $dockerfilePath -Encoding UTF8
    Write-Host "✅ Dockerfile generado para $nombre en: $dockerfilePath"
}