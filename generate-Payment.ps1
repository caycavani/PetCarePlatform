$Service = "Payment"
$BasePath = "."  
$ServerName = "XBSFSW01"
$MigrationName = "InitialCreate"

$infraPath = Join-Path $BasePath "PetCare.$Service.Infrastructure"
$apiPath = Join-Path $BasePath "PetCare.$Service.Api"
$persistencePath = Join-Path $infraPath "Persistence"
$dbContextName = "${Service}DbContext"
$dbContextFile = Join-Path $persistencePath "$dbContextName.cs"
$factoryFile = Join-Path $persistencePath "${dbContextName}Factory.cs"
$appsettingsFile = Join-Path $apiPath "appsettings.json"
$csprojInfra = Join-Path $infraPath "PetCare.$Service.Infrastructure.csproj"
$csprojApi = Join-Path $apiPath "PetCare.$Service.Api.csproj"
$databaseName = "PetCare${Service}Db"

foreach ($path in @($infraPath, $apiPath, $persistencePath)) {
    if (-not (Test-Path $path)) {
        New-Item -ItemType Directory -Path $path -Force | Out-Null
    }
}

Set-Content -Path $dbContextFile -Value @"
using Microsoft.EntityFrameworkCore;

namespace PetCare.$Service.Infrastructure.Persistence;

public class $dbContextName : DbContext
{
    public $dbContextName(DbContextOptions<$dbContextName> options) : base(options) { }

    public DbSet<object> Dummy { get; set; }
}
"@ -Encoding UTF8

Set-Content -Path $factoryFile -Value @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PetCare.$Service.Infrastructure.Persistence;

public class ${dbContextName}Factory : IDesignTimeDbContextFactory<$dbContextName>
{
    public $dbContextName CreateDbContext(string[] args)
    {
        var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "".."", ""PetCare.$Service.Api"");
        var configFile = Path.Combine(apiProjectPath, ""appsettings.json"");

        if (!File.Exists(configFile))
            throw new FileNotFoundException(""No se encontrÃ³ appsettings.json en el proyecto API"", configFile);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile(""appsettings.json"", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString(""DefaultConnection"");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(""No se encontrÃ³ la cadena de conexiÃ³n 'DefaultConnection'."");

        var optionsBuilder = new DbContextOptionsBuilder<$dbContextName>();
        optionsBuilder.UseSqlServer(connectionString);

        return new $dbContextName(optionsBuilder.Options);
    }
}
"@ -Encoding UTF8

if (-not (Test-Path $appsettingsFile)) {
    $jsonContent = @"
{
  ""ConnectionStrings"": {
    ""DefaultConnection"": ""Server=$ServerName;Database=$databaseName;Trusted_Connection=True;TrustServerCertificate=True;""
  }
}
"@
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($appsettingsFile, $jsonContent, $utf8NoBom)
}

if (-not (Test-Path $csprojInfra)) {
    Set-Content -Path $csprojInfra -Value @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
    <PackageReference Include=""Microsoft.EntityFrameworkCore"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.SqlServer"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.Design"" />
    <PackageReference Include=""Microsoft.Extensions.Configuration"" />
    <PackageReference Include=""Microsoft.Extensions.Configuration.Json"" />
    <PackageReference Include=""Microsoft.Extensions.Configuration.FileExtensions"" />
  </ItemGroup>
</Project>
"@ -Encoding UTF8
}

if (-not (Test-Path $csprojApi)) {
    Set-Content -Path $csprojApi -Value @"
<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.Design"" />
  </ItemGroup>
</Project>
"@ -Encoding UTF8
}

Push-Location $apiPath
try {
    dotnet ef migrations add $MigrationName `
        --project $csprojInfra `
        --startup-project $csprojApi `
        --output-dir "Persistence/Migrations"

    dotnet ef database update `
        --project $csprojInfra `
        --startup-project $csprojApi

    Write-Host "âœ… Migraciones aplicadas para $Service"
} catch {
    Write-Host "âŒ Error al aplicar migraciones para $Service: $($_.Exception.Message)"
}
Pop-Location
