FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Payment/PetCare.Payment.Api/PetCare.Payment.Api.csproj", "Payment/PetCare.Payment.Api/"]
COPY ["Payment/PetCare.Payment.Infrastructure/PetCare.Payment.Infrastructure.csproj", "Payment/PetCare.Payment.Infrastructure/"]
COPY ["Payment/PetCare.Payment.Domain/PetCare.Payment.Domain.csproj", "Payment/PetCare.Payment.Domain/"]
RUN dotnet restore "Payment/PetCare.Payment.Api/PetCare.Payment.Api.csproj"
COPY . .
WORKDIR "/src/Payment/PetCare.Payment.Api"
RUN dotnet publish "PetCare.Payment.Api.csproj" -c Release -o /app/publish

FROM petcareplatform/base-api:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PetCare.Payment.Api.dll"]
