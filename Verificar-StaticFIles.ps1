# Ruta al archivo Program.cs
$programPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\DevOpsPanel\PetCare.DevOpsPanel.Api\Program.cs"

Write-Host "`n🔍 Verificando si 'app.UseStaticFiles();' está presente..."

if (-Not (Test-Path $programPath)) {
    Write-Host "❌ No se encontró Program.cs en la ruta esperada:" -ForegroundColor Red
    Write-Host "   $programPath"
    exit 1
}

# Leer contenido del archivo
$contenido = Get-Content $programPath

# Verificar si ya existe
if ($contenido -match 'app\s*\.\s*UseStaticFiles\s*\(\s*\)\s*;') {
    Write-Host "✅ 'app.UseStaticFiles();' ya está presente." -ForegroundColor Green
} else {
    Write-Host "⚠️ No se encontró 'app.UseStaticFiles();'. Insertando..." -ForegroundColor Yellow

    # Buscar la línea con 'var app = builder.Build();'
    $indice = $contenido.FindIndex({ $_ -match 'var\s+app\s*=\s*builder\.Build\s*\(\s*\)\s*;' })

    if ($indice -ge 0) {
        $contenido = $contenido[0..$indice] + 'app.UseStaticFiles();' + $contenido[($indice + 1)..($contenido.Count - 1)]
        $contenido | Set-Content -Path $programPath -Encoding UTF8
        Write-Host "✅ Línea insertada correctamente después de 'var app = builder.Build();'" -ForegroundColor Green
    } else {
        Write-Host "❌ No se encontró 'var app = builder.Build();'. Inserción cancelada." -ForegroundColor Red
    }
}