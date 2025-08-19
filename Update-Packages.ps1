# Ruta base donde están tus proyectos
$basePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform"

# Paquetes que deseas forzar a versión 8.0.5
$packages = @(
    "Microsoft.EntityFrameworkCore",
    "Microsoft.EntityFrameworkCore.SqlServer",
    "Microsoft.EntityFrameworkCore.Design",
    "Microsoft.Extensions.Configuration",
    "Microsoft.Extensions.Configuration.Json",
    "Microsoft.Extensions.Configuration.FileExtensions",
    "Microsoft.Extensions.Caching.Abstractions",
    "Microsoft.Extensions.Caching.Memory",
    "Microsoft.Extensions.DependencyInjection",
    "Microsoft.Extensions.DependencyInjection.Abstractions",
    "Microsoft.Extensions.Logging",
    "Microsoft.Extensions.Logging.Abstractions",
    "Microsoft.Extensions.Options",
    "Microsoft.Extensions.Primitives"
)

# Buscar todos los archivos .csproj
$projects = Get-ChildItem -Path $basePath -Recurse -Filter *.csproj

foreach ($proj in $projects) {
    $projPath = $proj.FullName
    $projName = $proj.Name

    # Validar si el archivo contiene un SDK válido
    $content = Get-Content $projPath -Raw
    if ($content -match "<Project Sdk=") {
        Write-Host "`nActualizando paquetes en: $projName" -ForegroundColor Cyan
        foreach ($pkg in $packages) {
            try {
                dotnet add "$projPath" package $pkg --version 8.0.5
            } catch {
                Write-Host "Error al agregar $pkg en $projName" -ForegroundColor Red
            }
        }
    } else {
        Write-Host "`nProyecto ignorado (sin SDK): $projName" -ForegroundColor Yellow
    }
}
