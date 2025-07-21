FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Booking/PetCare.Booking.Api/PetCare.Booking.Api.csproj ./\Booking/PetCare.Booking.Api/
COPY Booking/PetCare.Booking.Application/PetCare.Booking.Application.csproj ./\Booking/PetCare.Booking.Application/
COPY Booking/PetCare.Booking.Domain/PetCare.Booking.Domain.csproj ./\Booking/PetCare.Booking.Domain/
COPY Booking/PetCare.Booking.Infrastructure/PetCare.Booking.Infrastructure.csproj ./\Booking/PetCare.Booking.Infrastructure/

RUN dotnet restore ./Booking/PetCare.Booking.Api/PetCare.Booking.Api.csproj
COPY . .
WORKDIR /src/Booking/PetCare.Booking.Api
RUN dotnet publish PetCare.Booking.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "PetCare.Booking.Api.dll"]
