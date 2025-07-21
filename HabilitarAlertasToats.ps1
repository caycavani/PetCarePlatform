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
      margin: 0;
    }
    h1 {
      text-align: center;
      font-size: 2rem;
      margin: 0;
    }
    #entornoActivo {
      text-align: center;
      color: #888;
      margin-top: 0.3rem;
    }
    #selectorEntorno {
      text-align: center;
      margin: 1rem 0;
    }
    #alertasContador {
      text-align: center;
      font-size: 1rem;
      margin-bottom: 1rem;
      color: #c0392b;
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
Write-Host "✅ landing.html generado con alertas, toasts y codificación UTF-8." -ForegroundColor Green