# 👷 Etapa de compilación: usamos SDK de .NET 8 para compilar y restaurar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 📦 Copiar archivos de proyecto (.csproj) individuales para evitar copiar todo el código de inicio
COPY Payment/PetCare.Payment.Api/PetCare.Payment.Api.csproj Payment/PetCare.Payment.Api/
COPY Payment/PetCare.Payment.Infrastructure/PetCare.Payment.Infrastructure.csproj Payment/PetCare.Payment.Infrastructure/
COPY Payment/PetCare.Payment.Domain/PetCare.Payment.Domain.csproj Payment/PetCare.Payment.Domain/
COPY Shared/PetCare.Shared.DTOs/PetCare.Shared.DTOs.csproj Shared/PetCare.Shared.DTOs/

# 🔧 Restaurar dependencias NuGet (resuelve todos los project references antes de copiar el resto del código)
RUN dotnet restore Payment/PetCare.Payment.Api/PetCare.Payment.Api.csproj

# 📁 Copiar el resto del código fuente de Payment y solo el proyecto compartido necesario
COPY Payment/ Payment/
COPY Shared/PetCare.Shared.DTOs/ Shared/PetCare.Shared.DTOs/

# ⚙️ Publicar la API en modo Release
WORKDIR /src/Payment/PetCare.Payment.Api
RUN dotnet publish PetCare.Payment.Api.csproj -c Release -o /app/publish

# 🛫 Etapa final: imagen liviana con solo el runtime para ejecutar el .dll compilado
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 🌐 Configurar la API para escuchar por el puerto 80
ENV ASPNETCORE_URLS=http://+:80

# 🚀 Ejecutar la API
ENTRYPOINT ["dotnet", "PetCare.Payment.Api.dll"]
