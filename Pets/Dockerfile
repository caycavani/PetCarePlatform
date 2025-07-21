FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Pets/PetCare.Pets.Api/PetCare.Pets.Api.csproj ./\Pets/PetCare.Pets.Api/
COPY Pets/PetCare.Pets.Application/PetCare.Pets.Application.csproj ./\Pets/PetCare.Pets.Application/
COPY Pets/PetCare.Pets.Domain/PetCare.Pets.Domain.csproj ./\Pets/PetCare.Pets.Domain/
COPY Pets/PetCare.Pets.Infrastructure/PetCare.Pets.Infrastructure.csproj ./\Pets/PetCare.Pets.Infrastructure/

RUN dotnet restore ./Pets/PetCare.Pets.Api/PetCare.Pets.Api.csproj
COPY . .
WORKDIR /src/Pets/PetCare.Pets.Api
RUN dotnet publish PetCare.Pets.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "PetCare.Pets.Api.dll"]
