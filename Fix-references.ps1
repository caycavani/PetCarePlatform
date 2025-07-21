<#
  Optimizado para tardar mucho menos: 
  - No recorre todo en -Recurse.
  - Lista explícita de módulos.
#>

param()

$root    = $PSScriptRoot
# Define aquí tus microservicios:
$modules = @('Booking','Payment','Notification','Review')

function Log($msg){ Write-Host "[INFO] $msg" -ForegroundColor Cyan }

function Fix-Module {
  param($mod)
  $modPath    = Join-Path $root $mod
  $domainProj = Join-Path $modPath "$mod.Domain\$mod.Domain.csproj"

  if (-not (Test-Path $domainProj)) {
    Write-Host "[WARN] No existe $mod.Domain" -ForegroundColor Yellow
    return
  }

  foreach ($layer in 'Application','Infrastructure') {
    $projFile = Join-Path $modPath "PetCare.$mod.$layer\PetCare.$mod.$layer.csproj"
    if (-not (Test-Path $projFile)) { continue }

    [xml]$xml = Get-Content $projFile

    # Eliminar referencias vacías
    $dirty = $false
    $xml.Project.ItemGroup.ProjectReference |
      Where-Object { -not $_.Include } |
      ForEach-Object { $_.ParentNode.RemoveChild($_) | Out-Null; $dirty = $true }

    if ($dirty) { $xml.Save($projFile) ; Log "$mod.$layer: Eliminadas referencias vacías" }

    # Asegurar referencia a Domain
    $refs = $xml.Project.ItemGroup.ProjectReference | ForEach-Object { $_.Include }
    $rel  = [IO.Path]::GetRelativePath((Split-Path $projFile), $domainProj)
    if ($refs -notcontains $rel) {
      $ig = $xml.Project.ItemGroup |
            Where-Object { $_.ProjectReference } |
            Select-Object -First 1
      if (-not $ig) {
        $ig = $xml.CreateElement('ItemGroup')
        $xml.Project.AppendChild($ig) | Out-Null
      }
      $pr = $xml.CreateElement('ProjectReference')
      $pr.SetAttribute('Include', $rel)
      $ig.AppendChild($pr) | Out-Null
      $xml.Save($projFile)
      Log "$mod.$layer: Añadida referencia a Domain"
    }
  }

  # Si existe API, forzar SDK-Web y refs
  $apiProj = Join-Path $modPath "PetCare.$mod.Api\PetCare.$mod.Api.csproj"
  if (Test-Path $apiProj) {
    [xml]$xml = Get-Content $apiProj
    if ($xml.Project.Sdk -ne 'Microsoft.NET.Sdk.Web') {
      $xml.Project.SetAttribute('Sdk','Microsoft.NET.Sdk.Web')
      Log "$mod.Api: SDK ajustado a Web"
    }

    foreach ($target in @($domainProj, (Join-Path $modPath "$mod.Application\$mod.Application.csproj"))) {
      if (Test-Path $target) {
        $refs = $xml.Project.ItemGroup.ProjectReference | ForEach-Object { $_.Include }
        $rel  = [IO.Path]::GetRelativePath((Split-Path $apiProj), $target)
        if ($refs -notcontains $rel) {
          $ig = $xml.Project.ItemGroup |
                Where-Object { $_.ProjectReference } |
                Select-Object -First 1
          if (-not $ig) {
            $ig = $xml.CreateElement('ItemGroup')
            $xml.Project.AppendChild($ig) | Out-Null
          }
          $pr = $xml.CreateElement('ProjectReference')
          $pr.SetAttribute('Include', $rel)
          $ig.AppendChild($pr) | Out-Null
          Log "$mod.Api: Añadida referencia a $([IO.Path]::GetFileNameWithoutExtension $target)"
        }
      }
    }

    # Swashbuckle
    $has = $xml.Project.ItemGroup.PackageReference |
           Where-Object { $_.Include -eq 'Swashbuckle.AspNetCore' }
    if (-not $has) {
      Log "$mod.Api: Instalando Swashbuckle.AspNetCore"
      dotnet add $apiProj package Swashbuckle.AspNetCore --version 6.5.0 | Out-Null
    }

    $xml.Save($apiProj)
  }
}

Log "Iniciando fix rápido de microservicios…"
foreach ($m in $modules) { Fix-Module $m }
Log "Terminado. Ahora corre:"
Log "  dotnet build PetCarePlatform.sln -c Release -m"