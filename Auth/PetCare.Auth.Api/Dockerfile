FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Auth/PetCare.Auth.Api/PetCare.Auth.Api.csproj", "Auth/PetCare.Auth.Api/"]
COPY ["Auth/PetCare.Auth.Infrastructure/PetCare.Auth.Infrastructure.csproj", "Auth/PetCare.Auth.Infrastructure/"]
COPY ["Auth/PetCare.Auth.Domain/PetCare.Auth.Domain.csproj", "Auth/PetCare.Auth.Domain/"]
RUN dotnet restore "Auth/PetCare.Auth.Api/PetCare.Auth.Api.csproj"
COPY . .
WORKDIR "/src/Auth/PetCare.Auth.Api"
RUN dotnet publish "PetCare.Auth.Api.csproj" -c Release -o /app/publish

FROM petcareplatform/base-api:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "PetCare.Auth.Api.dll"]
