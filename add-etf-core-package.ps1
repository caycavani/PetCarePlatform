param (
    [string]$ServiceName = "Payment"
)

$csproj = Get-ChildItem -Recurse -Filter "PetCare.$ServiceName.Infrastructure.csproj" | Select-Object -First 1

if (-not $csproj) {
    Write-Host "❌ No se encontró el proyecto Infrastructure para '$ServiceName'."
    exit 1
}

$projectPath = $csproj.Directory.FullName

$packages = @(
    "Microsoft.EntityFrameworkCore",
    "Microsoft.EntityFrameworkCore.SqlServer",
    "Microsoft.EntityFrameworkCore.Relational",
    "Microsoft.EntityFrameworkCore.Design",
    "Microsoft.Extensions.Configuration",
    "Microsoft.Extensions.Configuration.Json",
    "Microsoft.Extensions.Configuration.FileExtensions"
)

foreach ($pkg in $packages) {
    dotnet add "$projectPath" package $pkg
}

Write-Host "✅ Paquetes EF Core agregados a '$ServiceName'."