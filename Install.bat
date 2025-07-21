@echo off
setlocal

:: Ruta raíz del proyecto
set SOLUTION_PATH=C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform

:: Versión estable de .NET SDK
set SDK_VERSION=8.0.100

echo ==========================================================
echo 🔍 Verificando si el SDK %SDK_VERSION% está instalado...
echo ==========================================================
dotnet --list-sdks | findstr %SDK_VERSION% > nul
if errorlevel 1 (
    echo ❌ No se encontró el SDK %SDK_VERSION%.
    echo 🌐 Abriendo página oficial para instalarlo...
    start https://dotnet.microsoft.com/en-us/download/dotnet/8.0
    goto :EOF
)

echo.
echo =====================================
echo 📝 Generando archivo global.json...
echo =====================================

powershell -Command " @{
    sdk = @{ version = '%SDK_VERSION%' }
} | ConvertTo-Json -Depth 3 | Out-File -FilePath '%SOLUTION_PATH%\global.json' -Encoding UTF8"

echo.
echo =====================================
echo 🔄 Reemplazando TargetFramework net9.0...
echo =====================================

powershell -Command ^
    "Get-ChildItem -Recurse -Path '%SOLUTION_PATH%' -Filter *.csproj | ForEach-Object { ^
        $c = Get-Content $_.FullName; ^
        $n = $c -replace '<TargetFramework>net9\.0.*?</TargetFramework>', '<TargetFramework>net8.0</TargetFramework>'; ^
        if ($c -ne $n) { Set-Content $_.FullName $n; echo '🔧 Actualizado: ' + $_.FullName } ^
    }"

echo.
echo =====================================
echo 🧹 Limpiando y restaurando solución...
echo =====================================

cd /d "%SOLUTION_PATH%"
dotnet clean
dotnet restore

echo.
echo ✅ Entorno estabilizado con .NET %SDK_VERSION%.
pause