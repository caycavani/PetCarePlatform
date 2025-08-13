#!/bin/bash

echo "🔄 Iniciando proceso de migración para Auth API..."

# Variables
CONTEXT="AuthDbContext"
PROJECT="Auth/PetCare.Auth.Infrastructure"
STARTUP="Auth/PetCare.Auth.Api"
MIGRATION_NAME="UpdatedUserRoleRefreshToken"

# Paso 1: Eliminar migración previa (si existe y no fue aplicada)
echo "🧹 Eliminando migración previa (si existe)..."
dotnet ef migrations remove --context $CONTEXT --project $PROJECT --startup-project $STARTUP
if [ $? -eq 0 ]; then
  echo "✅ Migración previa eliminada correctamente."
else
  echo "⚠️ No se eliminó ninguna migración (puede que no existiera)."
fi

# Paso 2: Crear nueva migración
echo "🛠️ Generando nueva migración: $MIGRATION_NAME..."
dotnet ef migrations add $MIGRATION_NAME --context $CONTEXT --project $PROJECT --startup-project $STARTUP
if [ $? -eq 0 ]; then
  echo "✅ Migración '$MIGRATION_NAME' creada exitosamente."
else
  echo "❌ Error al crear la migración."
  exit 1
fi

# Paso 3: Aplicar migración
echo "📦 Aplicando migración a la base de datos..."
dotnet ef database update --context $CONTEXT --project $PROJECT --startup-project $STARTUP
if [ $? -eq 0 ]; then
  echo "✅ Migración aplicada correctamente."
else
  echo "❌ Error al aplicar la migración."
  exit 1
fi

echo "🎉 Proceso de migración completado con éxito."
