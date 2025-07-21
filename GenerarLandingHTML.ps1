$basePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\DevOpsPanel\PetCare.DevOpsPanel.Api\wwwroot"
$landing = Join-Path $basePath "landing.html"

if (-not (Test-Path $landing)) {
    Write-Host "📁 Creando landing.html..."
} else {
    Copy-Item $landing "$landing.bak" -Force
    Write-Host "📦 Copia de seguridad creada: landing.html.bak"
}

$html = @'
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <title>Panel de Microservicios</title>
  <style>
    body {
      font-family: "Segoe UI", sans-serif;
      padding: 2rem;
      background-color: #f6f8fa;
    }
    h1 {
      text-align: center;
      font-size: 2rem;
      margin-bottom: 0.2rem;
    }
    #entornoActivo {
      text-align: center;
      font-size: 1rem;
      color: #888;
      margin-bottom: 1rem;
    }
    #selectorEntorno {
      text-align: center;
      margin-bottom: 1.5rem;
    }
    select {
      font-size: 1rem;
      padding: 0.3rem 0.7rem;
    }
    #estadoServicios {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 1rem;
    }
    .service-card {
      background: white;
      border-left: 6px solid #ccc;
      border-radius: 5px;
      padding: 1rem;
      box-shadow: 0 1px 4px rgba(0,0,0,0.08);
      position: relative;
    }
    .service-card.ok { border-color: #2ecc71; }
    .service-card.warn { border-color: #f1c40f; }
    .service-card.fail { border-color: #e74c3c; }
    .service-card h2 {
      margin: 0;
      font-size: 1.1rem;
    }
    .status {
      margin-top: 0.5rem;
      font-weight: bold;
    }
    .latencia {
      font-size: 0.9rem;
      color: #555;
    }
    .restart-btn {
      position: absolute;
      top: 0.5rem;
      right: 1rem;
      font-size: 1.2rem;
      background: none;
      border: none;
      cursor: pointer;
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

function reiniciar(nombre) {
  window.location.href = 'reiniciar://' + nombre;
}

function verificarEstados(servicios) {
  const contenedor = document.getElementById('estadoServicios');
  contenedor.innerHTML = '';
  for (let i = 0; i < servicios.length; i++) {
    const servicio = servicios[i];
    const inicio = performance.now();
    fetch(servicio.url, { cache: 'no-store' })
      .then(function(r) {
        const ms = Math.round(performance.now() - inicio);
        const ok = r.ok;
        const status = ok ? '🟢 OK' : '🟠 Código ' + r.status;
        const clase = ok ? 'ok' : 'warn';
        contenedor.innerHTML += 
          '<div class="service-card ' + clase + '">' +
            '<button class="restart-btn" onclick="reiniciar(\'' + servicio.nombre + '\')">🌀</button>' +
            '<h2>' + servicio.nombre + '</h2>' +
            '<div class="status">' + status + '</div>' +
            '<div class="latencia">⏱️ ' + ms + ' ms</div>' +
          '</div>';
      })
      .catch(function() {
        contenedor.innerHTML += 
          '<div class="service-card fail">' +
            '<button class="restart-btn" onclick="reiniciar(\'' + servicio.nombre + '\')">🌀</button>' +
            '<h2>' + servicio.nombre + '</h2>' +
            '<div class="status">🔴 No responde</div>' +
          '</div>';
      });
  }
}

function cargarServicios() {
  const entorno = document.getElementById('entorno').value;
  document.getElementById('entornoNombre').textContent = entorno;
  localStorage.setItem('entornoSeleccionado', entorno);

  fetch('services.' + entorno + '.json')
    .then(r => r.json())
    .then(data => {
      verificarEstados(data);
      if (intervalo) clearInterval(intervalo);
      intervalo = setInterval(() => verificarEstados(data), 6000);
    })
    .catch(() => {
      document.getElementById('estadoServicios').innerHTML =
        '<div style="color:red;">Error al cargar lista de servicios.</div>';
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
Write-Host "✅ landing.html generado con codificación UTF-8. Emojis funcionando correctamente." -ForegroundColor Green