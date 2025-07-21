$landingPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\DevOpsPanel\PetCare.DevOpsPanel.Api\wwwroot\landing.html"

if (-not (Test-Path $landingPath)) {
    Write-Host "No se encontró landing.html en: $landingPath" -ForegroundColor Red
    exit 1
}

$backup = "$landingPath.bak"
Copy-Item $landingPath $backup -Force
Write-Host "Copia de seguridad creada en: $backup"

$html = Get-Content $landingPath -Raw

if ($html -match "estadoServicios") {
    Write-Host "El panel de estado ya está integrado. No se hicieron cambios." -ForegroundColor Yellow
    exit 0
}

$bloque = @'
<!-- Panel de Estado de Microservicios -->
<div>
  <h2>Estado de Microservicios</h2>
  <div id="estadoServicios">Cargando...</div>
</div>

<script>
const servicios = [
  { nombre: "Reservas", url: "http://localhost:5100/health" },
  { nombre: "Notificaciones", url: "http://localhost:5101/health" },
  { nombre: "Usuarios", url: "http://localhost:5102/health" }
];

function verificarEstados() {
  const contenedor = document.getElementById("estadoServicios");
  contenedor.innerHTML = "";

  servicios.forEach(servicio => {
    fetch(servicio.url)
      .then(r => {
        const estado = r.ok ? "🟢 OK" : `🔴 Código ${r.status}`;
        contenedor.innerHTML += `<div>${servicio.nombre} — ${estado}</div>`;
      })
      .catch(() => {
        contenedor.innerHTML += `<div>${servicio.nombre} — 🔴 No responde</div>`;
      });
  });
}

setInterval(verificarEstados, 5000);
window.onload = verificarEstados;
</script>
'@

if ($html -match "</body>") {
    $htmlNuevo = $html -replace "</body>", "$bloque`n</body>"
    $htmlNuevo | Set-Content -Path $landingPath -Encoding UTF8
    Write-Host "Panel de estado insertado correctamente." -ForegroundColor Green
} else {
    Write-Host "No se encontró la etiqueta </body>. Inserción cancelada." -ForegroundColor Red
}