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
    Write-Host "Todos los archivos .csproj fueron encontrados. Generando Dockerfiles y docker-compose.yml..." -ForegroundColor Green

    $dockerCompose = @()
    $dockerCompose += "services:"

    foreach ($servicio in $servicios) {
        $rutaDockerfile = Join-Path -Path $basePath -ChildPath "$servicio\Dockerfile"

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

        $indice = [array]::IndexOf($servicios, $servicio)
        if ($indice -ge 0) {
            $puerto = 5001 + $indice
        } else {
            Write-Host "Advertencia: no se pudo calcular el puerto para $servicio. Usando 5099." -ForegroundColor Yellow
            $puerto = 5099
        }

        $dockerCompose += "  $($servicio.ToLower())-api:"
        $dockerCompose += "    build:"
        $dockerCompose += "      context: ."
        $dockerCompose += "      dockerfile: $servicio/Dockerfile"
        $dockerCompose += "    ports:"
        $dockerCompose += "      - `"$($puerto):80`""
        $dockerCompose += "    environment:"
        $dockerCompose += "      - ASPNETCORE_ENVIRONMENT=Development"
        $dockerCompose += ""
    }

    $dockerCompose | Out-File -FilePath $dockerComposePath -Encoding UTF8
    Write-Host "`ndocker-compose.yml generado correctamente." -ForegroundColor Green

    $levantar = Read-Host "`n¿Deseas ejecutar 'docker-compose up --build'? (s/n)"
    if ($levantar -eq "s") {
        docker-compose up --build
    } else {
        Write-Host "Puedes ejecutar manualmente: docker-compose up --build" -ForegroundColor Yellow
    }

} else {
    Write-Host "`nSe encontraron $errores archivo(s) .csproj faltantes. No se generaron Dockerfiles ni docker-compose.yml." -ForegroundColor Yellow
    Write-Host "Verifica que cada microservicio tenga esta estructura:" -ForegroundColor Yellow
    Write-Host "  <Servicio>/PetCare.<Servicio>.<Capa>/PetCare.<Servicio>.<Capa>.csproj" -ForegroundColor Gray
}