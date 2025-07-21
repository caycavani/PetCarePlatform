# 👷 Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 📦 Solo copiamos los proyectos esenciales
COPY Notification/PetCare.Notification.Api/PetCare.Notification.Api.csproj Notification/PetCare.Notification.Api/
COPY Notification/PetCare.Notification.Infrastructure/PetCare.Notification.Infrastructure.csproj Notification/PetCare.Notification.Infrastructure/
COPY Notification/PetCare.Notification.Domain/PetCare.Notification.Domain.csproj Notification/PetCare.Notification.Domain/

# 🔧 Restaurar dependencias
RUN dotnet restore Notification/PetCare.Notification.Api/PetCare.Notification.Api.csproj

# 📁 Copiamos todo el código fuente después del restore
COPY Notification/ Notification/

# ⚙️ Publicamos el proyecto
WORKDIR /src/Notification/PetCare.Notification.Api
RUN dotnet publish PetCare.Notification.Api.csproj -c Release -o /app/publish

# 🛫 Etapa de ejecución con runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "PetCare.Notification.Api.dll"]
