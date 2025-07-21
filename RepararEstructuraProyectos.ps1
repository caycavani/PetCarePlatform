$basePath = Get-Location
$servicios = @("Auth", "Booking", "Notification", "Payment", "Pets", "Review")
$puertoBase = 5001
$compose = @()
$compose += "version: '3.8'"
$compose += "services:"

foreach ($i in 0..($servicios.Count - 1)) {
    $servicio = $servicios[$i]
    $puerto = $puertoBase + $i
    $nombre = "PetCare.$servicio.Api"
    $rutaDockerfile = Join-Path $basePath "$servicio\Dockerfile"

    # Generar Dockerfile
    $contenido = @"
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./$servicio/$nombre/$nombre.csproj
WORKDIR /src/$servicio/$nombre
RUN dotnet publish $nombre.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "$nombre.dll"]
"@
    $contenido | Out-File -FilePath $rutaDockerfile -Encoding UTF8
    Write-Host "✅ Dockerfile generado: $rutaDockerfile"

    # Agregar al docker-compose
    $compose += "  $($servicio.ToLower()):"
    $compose += "    build:"
    $compose += "      context: ."
    $compose += "      dockerfile: $servicio/Dockerfile"
    $compose += "    ports:"
    $compose += "      - $($puerto):80"
    $compose += "    restart: unless-stopped"
}

# Guardar docker-compose.yml
$composePath = Join-Path $basePath "docker-compose.yml"
$compose -join "`n" | Out-File -FilePath $composePath -Encoding UTF8
Write-Host "`n✅ docker-compose.yml generado: $composePath"