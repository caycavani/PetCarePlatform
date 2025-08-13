FROM mcr.microsoft.com/dotnet/sdk:8.0.100 AS build
WORKDIR /src
COPY ["Pets/PetCare.Pets.Api/PetCare.Pets.Api.csproj", "Pets/PetCare.Pets.Api/"]
COPY ["Pets/PetCare.Pets.Infrastructure/PetCare.Pets.Infrastructure.csproj", "Pets/PetCare.Pets.Infrastructure/"]
COPY ["Pets/PetCare.Pets.Domain/PetCare.Pets.Domain.csproj", "Pets/PetCare.Pets.Domain/"]
RUN dotnet restore "Pets/PetCare.Pets.Api/PetCare.Pets.Api.csproj"
COPY . .
WORKDIR "/src/Pets/PetCare.Pets.Api"
RUN dotnet publish "PetCare.Pets.Api.csproj" -c Release -o /app/publish

FROM petcareplatform/base-api:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PetCare.Pets.Api.dll"]
