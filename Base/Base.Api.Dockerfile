FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy
WORKDIR /app

# 🔧 Instalación garantizada
RUN apt-get update \
    && apt-get install -y curl bash iputils-ping \
    && rm -rf /var/lib/apt/lists/*

LABEL maintainer="Carlos"
EXPOSE 80
