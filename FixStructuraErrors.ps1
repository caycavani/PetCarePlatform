$projectRoot = Get-Location
$logDir = Join-Path $projectRoot "logs"
$buildLog = Join-Path $logDir "BuildErrors.log"
$entityLog = Join-Path $logDir "EntidadesYReferencias.log"

if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }
if (Test-Path $entityLog) { Remove-Item $entityLog }

# === 1. Extraer nombres de entidades faltantes desde BuildErrors.log ===
if (-not (Test-Path $buildLog)) {
    Write-Host "No se encontró BuildErrors.log. Ejecuta primero dotnet build con logging." -ForegroundColor Red
    exit 1
}

$errors = Get-Content $buildLog | Where-Object { $_ -match 'CS0246|CS0234' }
$missingEntities = @()

foreach ($line in $errors) {
    if ($line -match 'CS0246.*''(.+?)''') {
        $missingEntities += $matches[1]
    } elseif ($line -match 'CS0234.*''(.+?)''') {
        $nsParts = $matches[1] -split '\.'
        if ($nsParts.Count -gt 0) {
            $missingEntities += $nsParts[-1]
        }
    }
}

$missingEntities = $missingEntities | Sort-Object -Unique

# === 2. Inferir propiedades por entidad ===
function Get-EntityProperties {
    param ($entityName)

    switch ($entityName) {
        "Pet" {
            return @(
                "public Guid Id { get; set; }",
                "public string Name { get; set; }",
                "public string Breed { get; set; }",
                "public int Age { get; set; }",
                "public Guid OwnerId { get; set; }"
            )
        }
        "BookingEntity" {
            return @(
                "public Guid Id { get; set; }",
                "public Guid PetId { get; set; }",
                "public Guid CaregiverId { get; set; }",
                "public DateTime StartDate { get; set; }",
                "public DateTime EndDate { get; set; }",
                "public string Status { get; set; }"
            )
        }
        "PaymentEntity" {
            return @(
                "public Guid Id { get; set; }",
                "public Guid BookingId { get; set; }",
                "public decimal Amount { get; set; }",
                "public string Currency { get; set; }",
                "public string Status { get; set; }",
                "public DateTime PaidAt { get; set; }"
            )
        }
        "ReviewEntity" {
            return @(
                "public Guid Id { get; set; }",
                "public Guid BookingId { get; set; }",
                "public int Rating { get; set; }",
                "public string Comment { get; set; }",
                "public DateTime CreatedAt { get; set; }"
            )
        }
        "NotificationEntity" {
            return @(
                "public Guid Id { get; set; }",
                "public Guid RecipientId { get; set; }",
                "public string Message { get; set; }",
                "public DateTime SentAt { get; set; }",
                "public string Type { get; set; }"
            )
        }
        default {
            return @(
                "public Guid Id { get; set; }",
                "public DateTime CreatedAt { get; set; }",
                "public string Status { get; set; }"
            )
        }
    }
}

# === 3. Crear entidades y agregarlas al .csproj si es necesario ===
foreach ($entity in $missingEntities) {
    $targetProj = Get-ChildItem -Recurse -Filter *.csproj | Where-Object { $_.FullName -match "\\$entity\\.*Domain" } | Select-Object -First 1
    if (-not $targetProj) {
        $targetProj = Get-ChildItem -Recurse -Filter *.csproj | Where-Object { $_.FullName -match "\\Domain\\" -and $_.FullName -match "$entity" } | Select-Object -First 1
    }
    if (-not $targetProj) { continue }

    $projDir = Split-Path $targetProj.FullName
    $entitiesDir = Join-Path $projDir "Entities"
    if (-not (Test-Path $entitiesDir)) {
        New-Item -ItemType Directory -Path $entitiesDir | Out-Null
        Add-Content $entityLog "[$($targetProj.BaseName)] Carpeta Entities creada."
    }

    $entityFile = Join-Path $entitiesDir "$entity.cs"
    if (-not (Test-Path $entityFile)) {
        $namespace = "$($targetProj.BaseName).Entities"
        $props = Get-EntityProperties -entityName $entity
        $body = ($props | ForEach-Object { "        $_" }) -join "`n"

        $code = @"
using System;

namespace $namespace
{
    public class $entity
    {
$body
    }
}
"@

        [System.IO.File]::WriteAllText($entityFile, $code, [System.Text.Encoding]::UTF8)
        Add-Content $entityLog "Entidad $entity.cs creada en $entitiesDir"
        Write-Host "$entity.cs generado en $($targetProj.BaseName)" -ForegroundColor Green
    }

    # Verificar inclusión en el .csproj
    $relativePath = $entityFile.Replace($projDir + "\", "").Replace("\", "/")
    $csprojContent = Get-Content $targetProj.FullName
    $included = $csprojContent | Where-Object { $_ -match [regex]::Escape($relativePath) }

    if (-not $included) {
        $insertLine = $csprojContent | Select-String "</Project>" | Select-Object -First 1
        if ($insertLine) {
            $index = $insertLine.LineNumber - 1
            $csprojContent = $csprojContent[0..($index - 1)] + "  <ItemGroup>`n    <Compile Include=`"$relativePath`" />`n  </ItemGroup>" + $csprojContent[$index..($csprojContent.Count - 1)]
            Set-Content -Path $targetProj.FullName -Value $csprojContent -Encoding UTF8
            Add-Content $entityLog "Entidad $entity.cs incluida manualmente en $($targetProj.Name)"
            Write-Host "$entity.cs incluido en $($targetProj.Name)" -ForegroundColor Yellow
        }
    }
}

# === 4. Solución inmediata: limpiar bin/obj y forzar recarga ===
Write-Host "`n🧹 Limpiando bin/ y obj/ para forzar recarga de Visual Studio..." -ForegroundColor Cyan
Get-ChildItem -Recurse -Directory -Include bin,obj | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "✅ Limpieza completada. Ahora puedes:" -ForegroundColor Green
Write-Host "1. Cerrar Visual Studio si está abierto."
Write-Host "2. Volver a abrir la solución."
Write-Host "3. Hacer clic derecho en el proyecto → Reload Project."
Write-Host "4. Verificar que las entidades aparecen correctamente."

Write-Host "`nEntidades generadas. Revisa logs\EntidadesYReferencias.log para ver los detalles." -ForegroundColor Cyan