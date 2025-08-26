# ┌─────────────────────────────┐
# │ Etapa 1: Build y publicación │
# └─────────────────────────────┘
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 📦 Copiar archivos de proyecto para restauración anticipada
COPY Auth/PetCare.Auth.Api/PetCare.Auth.Api.csproj Auth/PetCare.Auth.Api/
COPY Auth/PetCare.Auth.Infrastructure/PetCare.Auth.Infrastructure.csproj Auth/PetCare.Auth.Infrastructure/
COPY Auth/PetCare.Auth.Domain/PetCare.Auth.Domain.csproj Auth/PetCare.Auth.Domain/
COPY Shared/PetCare.Shared.DTOs/PetCare.Shared.DTOs.csproj Shared/PetCare.Shared.DTOs/

# 🔄 Restaurar dependencias (evita errores de referencia cruzada)
RUN dotnet restore Auth/PetCare.Auth.Api/PetCare.Auth.Api.csproj

# 📁 Copiar el resto del código fuente
COPY Auth/ Auth/
COPY Shared/ Shared/

# 🛠 Publicar el proyecto principal
WORKDIR /src/Auth/PetCare.Auth.Api
RUN dotnet publish PetCare.Auth.Api.csproj -c Release -o /app/publish \
    && echo "✅ Publicación exitosa: PetCare.Auth.Api.csproj"

# ┌─────────────────────────────┐
# │ Etapa 2: Imagen de ejecución │
# └─────────────────────────────┘
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 🧩 Instalar librería requerida por SQL Server (Kerberos)
RUN apt-get update && apt-get install -y libgssapi-krb5-2 \
    && echo "✅ Librería libgssapi-krb5-2 instalada"

# 📦 Copiar artefactos publicados desde la etapa de build
COPY --from=build /app/publish .

# 🌐 Configurar puerto de escucha
ENV ASPNETCORE_URLS=http://+:80

# 🚀 Ejecutar la aplicación
ENTRYPOINT ["dotnet", "PetCare.Auth.Api.dll"]
