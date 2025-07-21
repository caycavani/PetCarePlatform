# Generador automático de README.md para PetCarePlatform
# Arquitectura Hexagonal | Autor: Carlos

$readmePath = ".\README.md"

$content = @"
# 🐾 PetCare Platform

## 🎯 Propósito de la solución

PetCare Platform es una solución distribuida basada en microservicios orientada a facilitar la gestión de atención y cuidado de mascotas. Permite autenticación, registro de mascotas, pagos, reservas, notificaciones y opiniones. Está diseñada con enfoque modular, extensible y robusto.

---

## 🧱 Arquitectura implementada

- **Estilo arquitectónico**: Hexagonal (Ports and Adapters)
  - Capas por microservicio:
    - 🧠 Application: casos de uso
    - 📦 Domain: entidades y lógica pura
    - 🔌 Infrastructure: adaptadores externos (DB, HTTP, JWT)
    - 🌐 Api: controladores HTTP
- Microservicios: auth, pets, booking, review, notification, payment
- Proyecto compartido: PetCare.Shared.DTOs
- Contenedores: Docker Compose para orquestación
- Base de datos: SQL Server (contenedor)

---

## 🚀 Ejecución con Docker Compose

### 🔧 Requisitos

- Docker CLI o Docker Desktop
- Docker Compose v1.29+
- Puertos libres: 1433, 5001–5006

### 🐳 Comando de despliegue

```bash
docker-compose up --build -d
