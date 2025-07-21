$landing = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\DevOpsPanel\PetCare.DevOpsPanel.Api\wwwroot\landing.html"
$jsonPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\DevOpsPanel\PetCare.DevOpsPanel.Api\wwwroot\services.json"

# Crear services.json
$servicios = @(
    @{ nombre = "Reservas"; url = "http://localhost:5100/health" }
    @{ nombre = "Notificaciones"; url = "http://localhost:5101/health" }
    @{ nombre = "Usuarios"; url = "http://localhost:5102/health" }
)
$servicios | ConvertTo-Json -Depth 3 | Set-Content -Path $jsonPath -Encoding UTF8
Write-Host "✅ Archivo services.json creado en: $jsonPath" -ForegroundColor Green

# Verificar landing.html
if (-not (Test-Path $landing)) {
    Write-Host "❌ landing.html no encontrado en: $landing" -ForegroundColor Red
    exit 1
}

# Backup
$backup = "$landing.bak"
Copy-Item $landing $backup -Force
Write-Host "🔁 Backup creado en: $backup"

# Contenido HTML con carga dinámica
$html = @'
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <title>Panel de Microservicios</title>
  <style>
    body {
      margin: 0;
      padding: 2rem;
      font-family: "Segoe UI", sans-serif;
      background-color: #f6f8fa;
      color: #2c3e50;
    }
    h1 {
      text-align: center;
      font-size: 2rem;
      margin-bottom: 2rem;
    }
    #estadoServicios {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
      gap: 1rem;
    }
    .service-card {
      background: white;
      padding: 1.2rem 1rem;
      border-left: 6px solid #ccc;
      border-radius: 6px;
      box-shadow: 0 1px 4px rgba(0,0,0,0.08);
    }
    .service-card.ok { border-color: #2ecc71; }
    .service-card.warn { border-color: #f1c40f; }
    .service-card.fail { border-color: #e74c3c; }
    .service-card h2 {
      margin: 0;
      font-size: 1.1rem;
      color: #34495e;
    }
    .status {
      margin-top: 0.5rem;
      font-weight: bold;
      font-size: 1rem;
    }
  </style>
</head>
<body>
  <h1>🧭 Estado de Microservicios</h1>
  <div id="estadoServicios">Cargando...</div>

  <script>
    function verificarEstados(servicios) {
      const contenedor = document.getElementById('estadoServicios');
      contenedor.innerHTML = '';

      for (let i = 0; i < servicios.length; i++) {
        const servicio = servicios[i];

        fetch(servicio.url, { cache: 'no-store' })
          .then(function(r) {
            const ok = r.ok;
            const status = ok ? '🟢 OK' : '🟠 Código ' + r.status;
            const estado = ok ? 'ok' : 'warn';
            contenedor.innerHTML += '<div class="service-card ' + estado + '"><h2>' + servicio.nombre + '</h2><div class="status">' + status + '</div></div>';
          })
          .catch(function() {
            contenedor.innerHTML += '<div class="service-card fail"><h2>' + servicio.nombre + '</h2><div class="status">🔴 No responde</div></div>';
          });
      }
    }

    function cargarServicios() {
      fetch('services.json')
        .then(function(r) { return r.json(); })
        .then(function(data) {
          verificarEstados(data);
          setInterval(function() { verificarEstados(data); }, 6000);
        })
        .catch(function() {
          document.getElementById('estadoServicios').innerHTML = '<div style="color:red;">Error al cargar services.json</div>';
        });
    }

    window.onload = cargarServicios;
  </script>
</body>
</html>
'@

$html | Set-Content -Path $landing -Encoding UTF8
Write-Host "✅ landing.html actualizado para usar services.json dinámico." -ForegroundColor Cyan