# 👷 Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 📦 Copiamos los proyectos esenciales y sus referencias
COPY Notification/PetCare.Notification.Api/PetCare.Notification.Api.csproj Notification/PetCare.Notification.Api/
COPY Notification/PetCare.Notification.Infrastructure/PetCare.Notification.Infrastructure.csproj Notification/PetCare.Notification.Infrastructure/
COPY Notification/PetCare.Notification.Domain/PetCare.Notification.Domain.csproj Notification/PetCare.Notification.Domain/
COPY Shared/PetCare.Shared.DTOs/PetCare.Shared.DTOs.csproj Shared/PetCare.Shared.DTOs/

# 🔧 Restauramos dependencias
RUN dotnet restore Notification/PetCare.Notification.Api/PetCare.Notification.Api.csproj

# 📁 Copiamos todo el código fuente después del restore
COPY Notification/ Notification/
COPY Shared/ Shared/

# ⚙️ Publicamos el proyecto
WORKDIR /src/Notification/PetCare.Notification.Api
RUN dotnet publish PetCare.Notification.Api.csproj -c Release -o /app/publish

# 🛫 Etapa de ejecución con runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 🧾 Variables de entorno para desarrollo
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80

# 📦 Copiamos la publicación desde la etapa de build
COPY --from=build /app/publish .

# 🚪 Exponemos el puerto 80
EXPOSE 80

# 🚀 Ejecutamos la API
ENTRYPOINT ["dotnet", "PetCare.Notification.Api.dll"]
