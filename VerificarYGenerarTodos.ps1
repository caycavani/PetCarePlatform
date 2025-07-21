$servicios = @("Auth", "Booking", "Notification", "Payment", "Pets", "Review")
$errores = 0
$basePath = Get-Location
$dockerComposePath = Join-Path $basePath "docker-compose.yml"

Write-Host ""
Write-Host "Verificando archivos .csproj requeridos para cada microservicio..." -ForegroundColor Cyan

foreach ($servicio in $servicios) {
    Write-Host "`nMicroservicio: $servicio" -ForegroundColor Cyan

    $proyectos = @(
        "PetCare.$servicio.Api",
        "PetCare.$servicio.Application",
        "PetCare.$servicio.Domain",
        "PetCare.$servicio.Infrastructure"
    )

    foreach ($proyecto in $proyectos) {
        $rutaRelativa = "$servicio\$proyecto\$proyecto.csproj"
        $rutaCompleta = Join-Path -Path $basePath -ChildPath $rutaRelativa

        if (Test-Path $rutaCompleta) {
            Write-Host "[OK]    $rutaRelativa" -ForegroundColor Green
        } else {
            Write-Host "[FALTA] $rutaRelativa" -ForegroundColor Red
            $errores++
        }
    }
}

Write-Host ""
if ($errores -eq 0) {
    Write-Host "Todos los archivos .csproj fueron encontrados. Generando Dockerfiles..." -ForegroundColor Green

    $dockerCompose = @"
version: '3.8'

services:
"@

    foreach ($servicio in $servicios) {
        $rutaDockerfile = Join-Path -Path $basePath -ChildPath "$servicio\Dockerfile"

        # Generar Dockerfile
        $contenido = @"
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY $servicio/PetCare.$servicio.Api/PetCare.$servicio.Api.csproj ./\$servicio/PetCare.$servicio.Api/
COPY $servicio/PetCare.$servicio.Application/PetCare.$servicio.Application.csproj ./\$servicio/PetCare.$servicio.Application/
COPY $servicio/PetCare.$servicio.Domain/PetCare.$servicio.Domain.csproj ./\$servicio/PetCare.$servicio.Domain/
COPY $servicio/PetCare.$servicio.Infrastructure/PetCare.$servicio.Infrastructure.csproj ./\$servicio/PetCare.$servicio.Infrastructure/

RUN dotnet restore ./$servicio/PetCare.$servicio.Api/PetCare.$servicio.Api.csproj
COPY . .
WORKDIR /src/$servicio/PetCare.$servicio.Api
RUN dotnet publish PetCare.$servicio.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "PetCare.$servicio.Api.dll"]
"@

        $contenido | Out-File -FilePath $rutaDockerfile -Encoding UTF8
        Write-Host "Dockerfile generado en $rutaDockerfile" -ForegroundColor Green

        # Agregar al docker-compose.yml
        $puerto = 5000 + ($servicios.IndexOf($servicio) + 1)
        $dockerCompose += @"
  $($servicio.ToLower())-api:
    build:
      context: .
      dockerfile: $servicio/Dockerfile
    ports:
      - "$puerto:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development

"@
    }

    # Guardar docker-compose.yml
    $dockerCompose | Out-File -FilePath $dockerComposePath -Encoding UTF8
    Write-Host "`ndocker-compose.yml generado en $dockerComposePath" -ForegroundColor Green

} else {
    Write-Host "`nSe encontraron $errores archivo(s) .csproj faltantes. No se generaron Dockerfiles ni docker-compose.yml." -ForegroundColor Yellow
    Write-Host "Verifica que cada microservicio tenga esta estructura:" -ForegroundColor Yellow
    Write-Host "  <Servicio>/PetCare.<Servicio>.<Capa>/PetCare.<Servicio>.<Capa>.csproj" -ForegroundColor Gray
}