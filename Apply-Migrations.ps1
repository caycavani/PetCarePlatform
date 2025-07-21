param (
  [string]$MigrationName = "InitialSchema",
  [string]$ServerInstance = "localhost",
  [string]$BackupFolder = "DatabaseBackups"
)

$logFile = "migration-log.txt"
$configPath = "migration-config.json"

if (!(Test-Path $configPath)) {
    Write-Host "❌ No se encontró $configPath" -ForegroundColor Red
    exit 1
}

$config = Get-Content $configPath | ConvertFrom-Json
if (!(Test-Path $BackupFolder)) { New-Item -ItemType Directory -Path $BackupFolder | Out-Null }

foreach ($service in $config) {
    $name = $service.ServiceName
    $context = $service.Context
    $infra = $service.InfraPath
    $api = $service.ApiPath
    $db = $service.DatabaseName

    Write-Host "`n🚀 Procesando $name..." -ForegroundColor Cyan
    Add-Content $logFile "`n[$(Get-Date)] ${name}: Iniciado"

    $migCmd = "dotnet ef migrations add $MigrationName --project `"$infra`" --startup-project `"$api`" --context $context"
    Invoke-Expression $migCmd

    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Error generando migración en $name" -ForegroundColor Red
        Add-Content $logFile "[$(Get-Date)] ${name}: Error en migración"
        continue
    }

    try {
        $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
        $backupPath = Join-Path $BackupFolder "$db-$timestamp.bak"
        $sql = "BACKUP DATABASE [$db] TO DISK = N'$backupPath' WITH INIT, FORMAT"
        Invoke-Sqlcmd -ServerInstance $ServerInstance -Query $sql
        Write-Host "🛡️ Backup creado: $backupPath" -ForegroundColor Yellow
        Add-Content $logFile "[$(Get-Date)] ${name}: Backup creado en $backupPath"
    }
    catch {
        Write-Host "⚠️ Error creando backup de $db" -ForegroundColor DarkYellow
        Add-Content $logFile "[$(Get-Date)] ${name}: Error creando backup ($($_.Exception.Message))"
    }

    $updateCmd = "dotnet ef database update --project `"$infra`" --startup-project `"$api`" --context $context"
    Invoke-Expression $updateCmd

    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Migración aplicada correctamente a $db" -ForegroundColor Green
        Add-Content $logFile "[$(Get-Date)] ${name}: Migración aplicada"
    } else {
        Write-Host "❌ Error aplicando migración en $name" -ForegroundColor Red
        Add-Content $logFile "[$(Get-Date)] ${name}: Error en update"
    }
}
