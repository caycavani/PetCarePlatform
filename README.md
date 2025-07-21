📦 PetCarePlatform – Ecosistema de Microservicios para Cuidado de Mascotas
Este proyecto contiene seis microservicios desarrollados con .NET y arquitectura hexagonal, organizados para operar en contenedores usando Docker y orquestados con docker-compose.

🧱 Microservicios incluidos
| Servicio | Puerto | Función | 
| Auth | 5001 | Registro y login de usuarios | 
| Pets | 5002 | Gestión de mascotas | 
| Booking | 5003 | Creación y confirmación de reservas | 
| Payment | 5004 | Procesamiento de pagos | 
| Review | 5005 | Valoraciones de cuidadores | 
| Notification | 5006 | Envío de notificaciones | 


Requisitos
- .NET 7 SDK
- Docker Desktop (Windows/macOS) o Docker Engine en Linux
- PowerShell o Bash

🛠️ Pasos para levantar la solución completa
- Clona el repositorio o ubica el workspace
cd "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform"


- Genera los Dockerfile necesarios
Desde PowerShell:
.\crear-dockerfiles.ps1


- Ejecuta docker-compose
docker-compose up --build -d


Esto:
- Compilará las APIs
- Levantará una base de datos SQL Server por servicio
- Expondrá los puertos de cada API

🌐 Acceso rápido
- Swagger UI:
- http://localhost:5001/swagger → Auth
- http://localhost:5002/swagger → Pets
- ... y así sucesivamente
- SQL Server:
- Usa localhost,1433 (ó 1434, 1435…) según el microservicio
- Usuario: sa
- Password: Your_password123

✅ Servicios incluidos en docker-compose.yml
Cada servicio contiene:
- API ASP.NET Core
- Dockerfile generado automáticamente
- SQL Server como contenedor dependiente
- Variable de entorno ConnectionStrings__Default configurada

📌 Notas adicionales
- Puedes extender el sistema usando Azure Event Grid o colas RabbitMQ.
- Usa docker-compose logs -f para observar eventos o errores.
- Puedes añadir reverse proxy (e.g. YARP o NGINX) para facilitar enrutamiento.


