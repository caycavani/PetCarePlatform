FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Review/PetCare.Review.Api/PetCare.Review.Api.csproj ./\Review/PetCare.Review.Api/
COPY Review/PetCare.Review.Application/PetCare.Review.Application.csproj ./\Review/PetCare.Review.Application/
COPY Review/PetCare.Review.Domain/PetCare.Review.Domain.csproj ./\Review/PetCare.Review.Domain/
COPY Review/PetCare.Review.Infrastructure/PetCare.Review.Infrastructure.csproj ./\Review/PetCare.Review.Infrastructure/

RUN dotnet restore ./Review/PetCare.Review.Api/PetCare.Review.Api.csproj
COPY . .
WORKDIR /src/Review/PetCare.Review.Api
RUN dotnet publish PetCare.Review.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "PetCare.Review.Api.dll"]
