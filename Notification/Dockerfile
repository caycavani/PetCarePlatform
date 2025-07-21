FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Notification/PetCare.Notification.Api/PetCare.Notification.Api.csproj ./\Notification/PetCare.Notification.Api/
COPY Notification/PetCare.Notification.Application/PetCare.Notification.Application.csproj ./\Notification/PetCare.Notification.Application/
COPY Notification/PetCare.Notification.Domain/PetCare.Notification.Domain.csproj ./\Notification/PetCare.Notification.Domain/
COPY Notification/PetCare.Notification.Infrastructure/PetCare.Notification.Infrastructure.csproj ./\Notification/PetCare.Notification.Infrastructure/

RUN dotnet restore ./Notification/PetCare.Notification.Api/PetCare.Notification.Api.csproj
COPY . .
WORKDIR /src/Notification/PetCare.Notification.Api
RUN dotnet publish PetCare.Notification.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "PetCare.Notification.Api.dll"]
