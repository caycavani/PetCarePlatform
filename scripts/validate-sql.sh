#!/bin/bash

# 🎯 CONFIGURACIÓN
SQLSERVER_HOST="192.168.10.8"
SQLSERVER_PORT=1433
SQLSERVER_USER="petcare_user"
SQLSERVER_PASSWORD="Nivacathy2033$#"
SQLSERVER_TIMEOUT=5
MAX_RETRIES=10
WAIT_SECONDS=3

# 🎨 COLORES
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # Sin color

# 🧾 LOG HEADER
echo -e "${YELLOW}🔍 Validando conectividad a SQL Server (${SQLSERVER_HOST}:${SQLSERVER_PORT})...${NC}"

# 🕰️ RETRIES
for ((i=1;i<=MAX_RETRIES;i++)); do
    echo -e "${YELLOW}🔁 Intento ${i} de ${MAX_RETRIES}...${NC}"

    timeout $SQLSERVER_TIMEOUT bash -c \
    "echo > /dev/tcp/${SQLSERVER_HOST}/${SQLSERVER_PORT}" 2>/dev/null

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Conexión exitosa con SQL Server.${NC}"
        exit 0
    else
        echo -e "${RED}⛔ No se pudo establecer conexión.${NC}"
        sleep $WAIT_SECONDS
    fi
done

echo -e "${RED}❌ SQL Server no respondió tras ${MAX_RETRIES} intentos. Abortando migración.${NC}"
exit 1
