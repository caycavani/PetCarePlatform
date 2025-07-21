$basePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\DevOpsPanel\PetCare.DevOpsPanel.Api\wwwroot"
$landing = Join-Path $basePath "landing.html"

# Crear archivos JSON de servicios por entorno
$entornos = @{
  "dev" = @(
    @{ nombre = "Reservas"; url = "http://localhost:5100/health" }
    @{ nombre = "Notificaciones"; url = "http://localhost:5101/health" }
    @{ nombre = "Usuarios"; url = "http://localhost:5102/health" }
  )
  "qa" = @(
    @{ nombre = "Reservas"; url = "http://qa.reservas.local/health" }
    @{ nombre = "Notificaciones"; url = "http://qa.notifs.local/health" }
    @{ nombre = "Usuarios"; url = "http://qa.usuarios.local/health" }
  )
  "prod" = @(
    @{ nombre = "Reservas"; url = "https://reservas.pets.com/api/health" }
    @{ nombre = "Notificaciones"; url = "https://notifs.pets.com/api/health" }
    @{ nombre = "Usuarios"; url = "https://usuarios.pets.com/api/health" }
  )
}

foreach ($env in $entornos.Keys) {
  $jsonPath = Join-Path $basePath "services.$env.json"
  $entornos[$env] | ConvertTo-Json -Depth 3 | Set-Content -Path $jsonPath -Encoding UTF8
  Write-Host "✅ services.$env.json creado"
}

# Backup y reemplazo del landing.html
if (-not (Test-Path $landing)) {
  Write-Host "❌ landing.html no encontrado en: $landing" -ForegroundColor Red
  exit 1
}
Copy-Item $landing "$landing.bak" -Force

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
      margin-bottom: 0.5rem;
    }
    #entornoActivo {
      text-align: center;
      font-size: 1rem;
      color: #888;
      margin-bottom: 1.5rem;
    }
    #selectorEntorno {
      display: flex;
      justify-content: center;
      margin-bottom: 1.5rem;
    }
    select {
      font-size: 1rem;
      padding: 0.4rem 0.8rem;
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
  <div id="entornoActivo">Entorno: <span id="entornoNombre">--</span></div>
  <div id="selectorEntorno">
    <label for="entorno">Cambiar entorno: </label>
    <select id="entorno" onchange="cargarServicios()">
      <option value="dev">Dev</option>
      <option value="qa">QA</option>
      <option value="prod">Prod</option>
    </select>
  </div>
  <div id="estadoServicios">Cargando...</div>

  <script>
    let intervalo = null;

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
      const entorno = document.getElementById('entorno').value;
      document.getElementById('entornoNombre').textContent = entorno;
      localStorage.setItem('entornoSeleccionado', entorno);

      const archivo = 'services.' + entorno + '.json';
      fetch(archivo)
        .then(function(r) { return r.json(); })
        .then(function(data) {
          verificarEstados(data);
          if (intervalo) clearInterval(intervalo);
          intervalo = setInterval(function() { verificarEstados(data); }, 6000);
        })
        .catch(function() {
          document.getElementById('estadoServicios').innerHTML = '<div style="color:red;">Error al cargar ' + archivo + '</div>';
        });
    }

    window.onload = function() {
      const guardado = localStorage.getItem('entornoSeleccionado') || 'dev';
      document.getElementById('entorno').value = guardado;
      cargarServicios();
    };
  </script>
</body>
</html>
'@

$html | Set-Content -Path $landing -Encoding UTF8
Write-Host "✅ Panel actualizado con selector de entorno, persistencia y visibilidad." -ForegroundColor Cyan