$rootPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform"
$csprojFiles = Get-ChildItem -Path $rootPath -Recurse -Filter *.csproj

$assemblyNames = @{}

Write-Host "`n[INFO] Escaneando proyectos en: $rootPath`n"

foreach ($file in $csprojFiles) {
    [xml]$xml = Get-Content $file.FullName
    $projectPath = $file.FullName

    # AssemblyName
    $assemblyName = $xml.Project.PropertyGroup.AssemblyName
    if (-not $assemblyName) {
        $assemblyName = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
    }

    if ($assemblyNames.ContainsKey($assemblyName)) {
        Write-Host "[ERROR] Colisión de AssemblyName: '$assemblyName'"
        Write-Host "   - Proyecto 1: $($assemblyNames[$assemblyName])"
        Write-Host "   - Proyecto 2: $projectPath`n"
    } else {
        $assemblyNames[$assemblyName] = $projectPath
    }

    # Referencias duplicadas dentro del mismo proyecto
    $refs = $xml.SelectNodes("//ProjectReference")
    $seenRefs = @{}
    foreach ($ref in $refs) {
        $refPath = [System.IO.Path]::GetFullPath((Join-Path $file.DirectoryName $ref.Include))
        if ($seenRefs.ContainsKey($refPath)) {
            Write-Host "[WARNING] Referencia duplicada en el mismo proyecto:"
            Write-Host "   - Proyecto: $projectPath"
            Write-Host "   - Referencia duplicada: $refPath`n"
        } else {
            $seenRefs[$refPath] = $true
        }
    }

    # Referencias manuales
    $manualRefs = $xml.SelectNodes("//Reference")
    foreach ($manualRef in $manualRefs) {
        $include = $manualRef.Include
        $hintPath = $manualRef.HintPath
        if ($hintPath -and $include -like "*PetCare.Shared.DTOs*") {
            Write-Host "[ALERTA] Referencia manual detectada:"
            Write-Host "   - Proyecto: $projectPath"
            Write-Host "   - Referencia: $include"
            Write-Host "   - HintPath: $hintPath`n"
        }
    }
}

Write-Host "`n[OK] Escaneo completado.`n"
