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
      background-color: #f6f8fa;
      padding: 2rem;
    }
    h1 {
      text-align: center;
      font-size: 2rem;
    }
    #entornoActivo {
      text-align: center;
      margin-bottom: 0.5rem;
      color: #888;
    }
    #selectorEntorno {
      text-align: center;
      margin-bottom: 1.5rem;
    }
    select {
      font-size: 1rem;
      padding: 0.4rem 0.8rem;
    }
    #estadoServicios {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 1rem;
    }
    .service-card {
      background: white;
      padding: 1rem;
      border-left: 6px solid #ccc;
      border-radius: 6px;
      box-shadow: 0 1px 4px rgba(0,0,0,0.08);
      position: relative;
    }
    .service-card.ok { border-color: #2ecc71; }
    .service-card.warn { border-color: #f1c40f; }
    .service-card.fail { border-color: #e74c3c; }
    .service-card.alerta {
      animation: pulso 1s infinite;
    }
    @keyframes pulso {
      0% { box-shadow: 0 0 0 rgba(231, 76, 60, 0); }
      50% { box-shadow: 0 0 10px rgba(231, 76, 60, 0.5); }
      100% { box-shadow: 0 0 0 rgba(231, 76, 60, 0); }
    }
    .restart-btn {
      position: absolute;
      right: 1rem;
      top: 0.7rem;
      font-size: 1.1rem;
      background: none;
      border: none;
      cursor: pointer;
    }
    .latencia {
      font-size: 0.85rem;
      color: #555;
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
const contadorErrores = new Map();
const umbralLatencia = 500; // en milisegundos
const maxErrores = 2; // fallos consecutivos antes de marcar alerta

function reiniciar(nombre) {
  window.location.href = 'reiniciar://' + nombre;
}

function crearTarjeta(servicio, estado, mensaje, latenciaMs, alerta) {
  const clase = estado === 'ok' ? 'ok' : (estado === 'warn' ? 'warn' : 'fail');
  const extra = alerta ? ' alerta' : '';
  const latenciaHTML = latenciaMs ? `<div class="latencia">⏱️ ${latenciaMs} ms</div>` : '';
  return `
    <div class="service-card ${clase}${extra}">
      <button class="restart-btn" onclick="reiniciar('${servicio.nombre}')">🌀</button>
      <h2>${servicio.nombre}</h2>
      <div class="status">${mensaje}</div>
      ${latenciaHTML}
    </div>
  `;
}

function verificarEstados(servicios) {
  const contenedor = document.getElementById('estadoServicios');
  contenedor.innerHTML = '';

  for (let i = 0; i < servicios.length; i++) {
    const servicio = servicios[i];
    const t0 = performance.now();

    fetch(servicio.url, { cache: 'no-store' })
      .then(r => {
        const t1 = performance.now();
        const ms = Math.round(t1 - t0);
        const ok = r.ok;
        const nombre = servicio.nombre;
        const estado = ok ? 'ok' : 'warn';
        const mensaje = ok ? '🟢 OK' : `🟠 Código ${r.status}`;

        // Reset contador si fue exitosa
        contadorErrores.set(nombre, 0);

        const alerta = ms > umbralLatencia;
        contenedor.innerHTML += crearTarjeta(servicio, estado, mensaje, ms, alerta);
      })
      .catch(() => {
        const nombre = servicio.nombre;
        const errores = (contadorErrores.get(nombre) || 0) + 1;
        contadorErrores.set(nombre, errores);

        const alerta = errores >= maxErrores;
        contenedor.innerHTML += crearTarjeta(servicio, 'fail', '🔴 No responde', null, alerta);
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
      contadorErrores.clear();
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
Write-Host "✅ Panel actualizado con lógica de alertas visuales por latencia y fallos." -ForegroundColor Green