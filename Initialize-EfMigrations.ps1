Function Initialize-EfMigration {
    param (
        [string]$MicroserviceName,
        [string]$DbContextName,
        [string]$InfrastructurePath,
        [string]$StartupPath
    )

    $infraProjPath = Get-ChildItem -Path $InfrastructurePath -Filter *.csproj -File | Select-Object -First 1 | ForEach-Object { $_.FullName }
    $startupProjPath = Get-ChildItem -Path $StartupPath -Filter *.csproj -File | Select-Object -First 1 | ForEach-Object { $_.FullName }

    if (-not (Test-Path $infraProjPath)) {
        Write-Host "No se encontró el proyecto Infrastructure en: $InfrastructurePath" -ForegroundColor Yellow
        return
    }

    if (-not (Test-Path $startupProjPath)) {
        Write-Host "No se encontró el proyecto Startup en: $StartupPath" -ForegroundColor Yellow
        return
    }

    $firstLine = Get-Content $infraProjPath -TotalCount 1
    if ($firstLine -notmatch "Sdk=") {
        Write-Host "Proyecto no compatible con SDK-style: $infraProjPath" -ForegroundColor Red
        return
    }

    $migrationName = "Initial$MicroserviceName`Schema"

    try {
        dotnet ef migrations add $migrationName `
            --project $infraProjPath `
            --startup-project $startupProjPath `
            --context $DbContextName `
            --msbuildprojectextensionspath "$(Join-Path $PSScriptRoot 'obj')" `
            --verbose

        dotnet ef database update `
            --project $infraProjPath `
            --startup-project $startupProjPath `
            --context $DbContextName `
            --msbuildprojectextensionspath "$(Join-Path $PSScriptRoot 'obj')" `
            --verbose

        Write-Host "Migración completada: $migrationName" -ForegroundColor Green
    }
    catch {
        Write-Host "Error al procesar la migración: $_" -ForegroundColor Red
    }
}

$nombre     = Read-Host "Nombre del microservicio (ejemplo: Pets)"
$contexto   = Read-Host "Nombre del DbContext (ejemplo: PetDbContext)"
$infraRuta  = Read-Host "Ruta de la carpeta Infrastructure (ejemplo: src\\PetCare.Pets\\Infrastructure)"
$apiRuta    = Read-Host "Ruta de la carpeta Api o Startup (ejemplo: src\\PetCare.Pets\\Api)"

Initialize-EfMigration -MicroserviceName $nombre -DbContextName $contexto -InfrastructurePath $infraRuta -StartupPath $apiRuta
