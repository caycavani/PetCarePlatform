#!/bin/bash

# Microservicios a validar (puedes extender esta lista)
SERVICES=(
  "auth-api:http://localhost:5001/api/debug/jwt-config"
  "review-api:http://localhost:5002/api/debug/jwt-config"
  "booking-api:http://localhost:5003/api/debug/jwt-config"
)

# Valores esperados
EXPECTED_ISSUER="http://petcare_auth"
EXPECTED_AUDIENCE="PetCareClientApp"
EXPECTED_SECRET_LENGTH=32  # Ajusta según tu clave real

echo "🔍 Validando configuración JWT en microservicios..."

for service in "${SERVICES[@]}"; do
  NAME="${service%%:*}"
  URL="${service#*:}"

  echo -e "\n➡️ $NAME ($URL)"
  RESPONSE=$(curl -s "$URL")

  ISSUER=$(echo "$RESPONSE" | jq -r '.issuer')
  AUDIENCE=$(echo "$RESPONSE" | jq -r '.audience')
  SECRET_LENGTH=$(echo "$RESPONSE" | jq -r '.secretLength')

  [[ "$ISSUER" == "$EXPECTED_ISSUER" ]] && echo "✅ Issuer OK" || echo "❌ Issuer mismatch: $ISSUER"
  [[ "$AUDIENCE" == "$EXPECTED_AUDIENCE" ]] && echo "✅ Audience OK" || echo "❌ Audience mismatch: $AUDIENCE"
  [[ "$SECRET_LENGTH" -ge "$EXPECTED_SECRET_LENGTH" ]] && echo "✅ Secret length OK" || echo "❌ Secret length too short: $SECRET_LENGTH"
done

echo -e "\n🏁 Validación completada."
