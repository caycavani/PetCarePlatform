# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y dependencias
COPY Booking/PetCare.Booking.Api/PetCare.Booking.Api.csproj Booking/PetCare.Booking.Api/
COPY Booking/PetCare.Booking.Infrastructure/PetCare.Booking.Infrastructure.csproj Booking/PetCare.Booking.Infrastructure/
COPY Booking/PetCare.Booking.Domain/PetCare.Booking.Domain.csproj Booking/PetCare.Booking.Domain/
COPY Shared/PetCare.Shared.DTOs/PetCare.Shared.DTOs.csproj Shared/PetCare.Shared.DTOs/

# Restaurar dependencias
RUN dotnet restore Booking/PetCare.Booking.Api/PetCare.Booking.Api.csproj

# Copiar el resto del microservicio
COPY Booking/ Booking/
COPY Shared/ Shared/

# Compilar y publicar
WORKDIR /src/Booking/PetCare.Booking.Api
RUN dotnet publish PetCare.Booking.Api.csproj -c Release -o /app/publish

# Etapa final para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "PetCare.Booking.Api.dll"]
