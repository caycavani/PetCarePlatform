# ┌────────────────────────────────────────────┐
# │ 👷 Etapa de compilación: restaurar y publicar │
# └────────────────────────────────────────────┘
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 📦 Copiamos solo los archivos .csproj necesarios para restauración temprana
COPY Review/PetCare.Review.Api/PetCare.Review.Api.csproj Review/PetCare.Review.Api/
COPY Review/PetCare.Review.Infrastructure/PetCare.Review.Infrastructure.csproj Review/PetCare.Review.Infrastructure/
COPY Review/PetCare.Review.Domain/PetCare.Review.Domain.csproj Review/PetCare.Review.Domain/
COPY Shared/PetCare.Shared.DTOs/PetCare.Shared.DTOs.csproj Shared/PetCare.Shared.DTOs/

# 🔧 Restauramos dependencias (nuget) antes de copiar todo el código
RUN dotnet restore Review/PetCare.Review.Api/PetCare.Review.Api.csproj \
    && echo "✅ Restauración exitosa: PetCare.Review.Api.csproj"

# 📁 Copiamos el resto del código fuente
COPY Review/ Review/
COPY Shared/ Shared/

# ⚙️ Publicamos la API en modo Release dentro de /app/publish
WORKDIR /src/Review/PetCare.Review.Api
RUN dotnet publish PetCare.Review.Api.csproj -c Release -o /app/publish \
    && echo "✅ Publicación exitosa: PetCare.Review.Api.csproj"

# ┌────────────────────────────────────────────┐
# │ 🛫 Etapa final: imagen de ejecución liviana │
# └────────────────────────────────────────────┘
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 📦 Copiamos los artefactos publicados desde la etapa de build
COPY --from=build /app/publish .

# 🌐 Configuramos la API para escuchar en el puerto 80
ENV ASPNETCORE_URLS=http://+:80

# 🏃 Entrada principal del contenedor (ejecuta la API)
ENTRYPOINT ["dotnet", "PetCare.Review.Api.dll"]
