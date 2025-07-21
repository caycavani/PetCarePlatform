# 👷 Etapa de compilación: usamos el SDK completo para restaurar y compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 📦 Copiamos solo los archivos .csproj para evitar copiar código innecesario de inicio
COPY Review/PetCare.Review.Api/PetCare.Review.Api.csproj Review/PetCare.Review.Api/
COPY Review/PetCare.Review.Infrastructure/PetCare.Review.Infrastructure.csproj Review/PetCare.Review.Infrastructure/
COPY Review/PetCare.Review.Domain/PetCare.Review.Domain.csproj Review/PetCare.Review.Domain/

# 🔧 Restauramos dependencias (nuget) para que estén listas antes del copy total
RUN dotnet restore Review/PetCare.Review.Api/PetCare.Review.Api.csproj

# 📁 Copiamos el resto del código de todos los proyectos
COPY Review/ Review/

# ⚙️ Publicamos la API en modo Release dentro de /app/publish
WORKDIR /src/Review/PetCare.Review.Api
RUN dotnet publish PetCare.Review.Api.csproj -c Release -o /app/publish

# 🛫 Etapa final: imagen más liviana con solo el runtime para ejecutar el .dll
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 🌐 Configuramos la API para escuchar en el puerto 80
ENV ASPNETCORE_URLS=http://+:80

# 🏃 Entrada principal del contenedor (ejecuta la API)
ENTRYPOINT ["dotnet", "PetCare.Review.Api.dll"]
