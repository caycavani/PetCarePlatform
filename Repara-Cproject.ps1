$csprojPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Booking\PetCare.Booking.Api\PetCare.Booking.Api.csproj"

$content = @"
<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include=""..\PetCare.Booking.Infrastructure\PetCare.Booking.Infrastructure.csproj"" />
  </ItemGroup>
</Project>
"@

[System.IO.File]::WriteAllText($csprojPath, $content, [System.Text.Encoding]::UTF8)