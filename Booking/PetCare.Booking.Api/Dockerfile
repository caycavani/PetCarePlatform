# 👷 Etapa de compilación: usamos SDK de .NET 8 para compilar y restaurar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 📦 Copiar archivos de proyecto (.csproj) individuales
COPY Booking/PetCare.Booking.Api/PetCare.Booking.Api.csproj Booking/PetCare.Booking.Api/
COPY Booking/PetCare.Booking.Infrastructure/PetCare.Booking.Infrastructure.csproj Booking/PetCare.Booking.Infrastructure/
COPY Booking/PetCare.Booking.Domain/PetCare.Booking.Domain.csproj Booking/PetCare.Booking.Domain/
COPY Shared/PetCare.Shared.DTOs/PetCare.Shared.DTOs.csproj Shared/PetCare.Shared.DTOs/

# 🔧 Restaurar dependencias NuGet
RUN dotnet restore Booking/PetCare.Booking.Api/PetCare.Booking.Api.csproj

# 📁 Copiar el resto del código fuente
COPY Booking/ Booking/
COPY Shared/PetCare.Shared.DTOs/ Shared/PetCare.Shared.DTOs/

# ⚙️ Publicar la API en modo Release
WORKDIR /src/Booking/PetCare.Booking.Api
RUN dotnet publish PetCare.Booking.Api.csproj -c Release -o /app/publish

# 🛫 Etapa final: imagen liviana con solo el runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 🌐 Configurar la API para escuchar por el puerto 80
ENV ASPNETCORE_URLS=http://+:80

# 🚀 Ejecutar la API
ENTRYPOINT ["dotnet", "PetCare.Booking.Api.dll"]
