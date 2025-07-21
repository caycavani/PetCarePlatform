$reportPath = "InMemoryRepositoryFixReport.txt"
$buildErrorLog = "BuildErrors.log"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

if (Test-Path $reportPath) { Remove-Item $reportPath }
if (Test-Path $buildErrorLog) { Remove-Item $buildErrorLog }

Add-Content -Path $reportPath -Value "---`n[$timestamp] Inicio del proceso de correccion`n---`n"
Add-Content -Path $buildErrorLog -Value "---`n[$timestamp] Errores de compilacion`n---`n"

function Write-Log {
    param($msg, $level = 'INFO', $logOnly = $false)
    $colors = @{ INFO = 'White'; SUCCESS = 'Green'; WARN = 'Yellow'; ERROR = 'Red' }
    if (-not $logOnly) {
        Write-Host "[$level] $msg" -ForegroundColor $colors[$level]
    }
    Add-Content -Path $reportPath -Value "[$level] $msg"
}

# Detectar archivo .sln
$solutionFile = Get-ChildItem -Recurse -Filter *.sln | Select-Object -First 1
if (-not $solutionFile) {
    Write-Log "No se encontro archivo .sln en la solucion. Abortando." "ERROR"
    exit 1
}
Write-Log "Solucion detectada: $($solutionFile.FullName)" "INFO"

# Buscar archivos InMemory*Repository.cs
$repoFiles = Get-ChildItem -Recurse -Filter *Repository.cs | Where-Object { $_.Name -like "InMemory*Repository.cs" }

if (-not $repoFiles) {
    Write-Log "No se encontraron clases InMemory*Repository.cs." "WARN"
    exit 0
}

foreach ($file in $repoFiles) {
    Write-Log "`nArchivo: $($file.FullName)" "INFO" $true
    Write-Log "Revisando: $($file.FullName)" "INFO"
    $original = Get-Content $file.FullName -Raw
    $fixed = $original
    $modified = $false

    if (-not [string]::IsNullOrWhiteSpace($fixed)) {
        # Usings
        if ($fixed -notmatch 'using\s+System\.Collections\.Generic\s*;') {
            $fixed = "using System.Collections.Generic;" + "`r`n" + $fixed
            $modified = $true
            Write-Log "Se agrego using System.Collections.Generic;" "SUCCESS"
        }
        if ($fixed -notmatch 'using\s+System\.Threading\.Tasks\s*;') {
            $fixed = "using System.Threading.Tasks;" + "`r`n" + $fixed
            $modified = $true
            Write-Log "Se agrego using System.Threading.Tasks;" "SUCCESS"
        }
        if ($fixed -match '\.\s*(Where|Select|Any|First|Single|Count)\s*\(' -and $fixed -notmatch 'using\s+System\.Linq\s*;') {
            $fixed = "using System.Linq;" + "`r`n" + $fixed
            $modified = $true
            Write-Log "Se detecto uso de LINQ y se agrego using System.Linq;" "SUCCESS"
        }

        # Namespace
        if ($fixed -match 'namespace\s+[^\s{]+[\r\n]+[^\{]') {
            $fixed = [regex]::Replace($fixed, '(namespace\s+[^\s{]+)[\r\n]+', { param($m) "$($m.Groups[1].Value) {" + "`r`n" })
            $fixed += "`r`n}"
            $modified = $true
            Write-Log "Se encapsulo el namespace con llaves." "SUCCESS"
        }

        # Tipo de entidad
        $entityType = ''
        if ($fixed -match 'List<([A-Za-z0-9_]+)>\s+_store\s*=\s*new') {
            $entityType = $matches[1]
            if ($fixed -match 'new\(\);') {
                $fixed = $fixed -replace 'new\(\);', "new List<$entityType>();"
                $modified = $true
                Write-Log "Se reemplazo new() por new List<$entityType>()." "SUCCESS"
            }
        }

        # AddAsync
        if ($fixed -match 'public\s+Task\s+AddAsync\(.*\)\s*{[^}]*}') {
            if ($fixed -notmatch 'return\s+Task\.CompletedTask') {
                $fixed = $fixed -replace '(AddAsync\(.*\)\s*{[^}]*?)\}', '$1 return Task.CompletedTask; }'
                $modified = $true
                Write-Log "Se agrego return Task.CompletedTask en AddAsync." "SUCCESS"
            }
            if ($fixed -notmatch '///\s*<summary>.*AddAsync') {
                $fixed = $fixed -replace '(public\s+Task\s+AddAsync)', '/// <summary>`r`n    /// Adds an entity to the in-memory store.`r`n    /// </summary>`r`n    $1'
                $modified = $true
                Write-Log "Se agrego comentario XML a AddAsync." "SUCCESS"
            }
        }

        # GetAllAsync
        if ($entityType -and $fixed -notmatch 'public\s+Task<.*IEnumerable<.*>>\s+GetAllAsync') {
            $getAllMethod = @"
    /// <summary>
    /// Returns all $entityType entities from the in-memory store.
    /// </summary>
    public Task<IEnumerable<$entityType>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<$entityType>>(_store);
    }
"@
            $fixed = $fixed -replace '(^\s*\})\s*$', "$getAllMethod`r`n`$1"
            $modified = $true
            Write-Log "Se agrego el metodo GetAllAsync() para $entityType." "SUCCESS"
        }

        # Interfaz, constructor y partial
        if ($fixed -match 'class\s+([A-Za-z0-9_]+)\s*(:\s*([A-Za-z0-9_\.]+))?') {
            $className = $matches[1]
            $interface = $matches[3]
            if (-not $interface) {
                Write-Log "La clase $className no implementa ninguna interfaz." "WARN"
            } else {
                Write-Log "La clase $className implementa $interface." "INFO"
            }

            if ($fixed -notmatch "$className\s*\(\s*\)") {
                $constructor = @"
    /// <summary>
    /// Initializes a new instance of the $className class.
    /// </summary>
    public $className()
    {
    }
"@
                $fixed = $fixed -replace '(^\s*\})\s*$', "$constructor`r`n`$1"
                $modified = $true
                Write-Log "Se agrego constructor sin parametros para $className." "SUCCESS"
            }

            if ($fixed -match "public\s+class\s+$className" -and $fixed -notmatch "partial\s+class\s+$className") {
                $fixed = $fixed -replace "public\s+class\s+$className", "public partial class $className"
                $modified = $true
                Write-Log "Se convirtio la clase $className en partial class." "SUCCESS"
            }
        }

        # Propiedad Items
        if ($entityType -and $fixed -notmatch 'public\s+IEnumerable<.*>\s+Items\s*=>\s*_store') {
            $itemsProp = @"
    /// <summary>
    /// Gets all items in the in-memory store.
    /// </summary>
    public IEnumerable<$entityType> Items => _store;
"@
            $fixed = $fixed -replace '(^\s*\})\s*$', "$itemsProp`r`n`$1"
            $modified = $true
            Write-Log "Se agrego propiedad publica Items para $entityType." "SUCCESS"
        }

        # Documentar métodos públicos sin comentario XML con <param> y <returns>
        $methodPattern = '(?m)^[ \t]*public\s+(?!class|interface|enum|delegate)([^\(]+)\s+([A-Za-z0-9_]+)\s*\(([^\)]*)\)\s*\{'
        $fixed = [regex]::Replace($fixed, $methodPattern, {
            param($m)
            $returnType = $m.Groups[1].Value.Trim()
            $methodName = $m.Groups[2].Value.Trim()
            $paramList = $m.Groups[3].Value.Trim()

            $summary = "/// <summary>`r`n    /// Executes $methodName.`r`n    /// </summary>`r`n"
            $paramXml = ""
            if ($paramList -ne "") {
                $params = $paramList -split '\s*,\s*'
                                foreach ($p in $params) {
                    if ($p -match '\s*([A-Za-z0-9_<>]+)\s+([A-Za-z0-9_]+)') {
                        $paramName = $matches[2]
                        $paramXml += "/// <param name=""$paramName"">The $paramName parameter.</param>`r`n"
                    }
                }
            }

            $returnsXml = ""
            if ($returnType -notmatch '^\s*void\s*$' -and $returnType -notmatch '^\s*Task\s*$') {
                $returnsXml = "/// <returns>The result of $methodName.</returns>`r`n"
            }

            $modified = $true
            Write-Log "Se documento automaticamente el metodo $methodName con parametros y retorno." "SUCCESS"
            return "$summary$paramXml$returnsXml$($m.Value)"
        })

        # Sugerir async/await si el método devuelve Task pero no es async
        $asyncSuggestionPattern = '(?m)^[ \t]*public\s+(?!async)(Task<[^>]+>|Task)\s+([A-Za-z0-9_]+)\s*\([^\)]*\)\s*\{[^\}]*?(Task\.FromResult|Task\.CompletedTask)'
        foreach ($match in [regex]::Matches($fixed, $asyncSuggestionPattern)) {
            $methodName = $match.Groups[2].Value
            Write-Log "El metodo $methodName devuelve Task pero no es async. Considera usar async/await para mayor claridad." "WARN"
        }

        # Convertir automáticamente a async/await si aplica
        $fixed = [regex]::Replace($fixed, '(?m)^([ \t]*public\s+)(Task<[^>]+>|Task)(\s+[A-Za-z0-9_]+\s*\([^\)]*\)\s*\{[^\}]*?)(return\s+)(Task\.FromResult|Task\.CompletedTask)', {
            param($m)
            $prefix = $m.Groups[1].Value
            $returnType = $m.Groups[2].Value
            $signature = $m.Groups[3].Value
            $returnStmt = $m.Groups[4].Value
            $taskCall = $m.Groups[5].Value

            $modified = $true
            Write-Log "Se convirtio automaticamente un metodo a async/await." "SUCCESS"

            return "${prefix}async $returnType$signature${returnStmt}await $taskCall"
        })
    } else {
        Write-Log "El contenido del archivo $($file.FullName) es nulo o vacío. Se omite." "WARN"
    }

    # Guardar si hubo modificaciones
    if ($modified) {
        Copy-Item $file.FullName "$($file.FullName).bak" -Force
        Set-Content $file.FullName -Value $fixed -Encoding UTF8
        Write-Log "Archivo corregido y respaldado como .bak" "SUCCESS"
    } else {
        Write-Log "No se requirio ninguna modificacion." "INFO"
    }
} # Fin del foreach

# Preguntar si se desea compilar
$shouldBuild = Read-Host "`nDeseas ejecutar dotnet build para validar los cambios? (s/n)"
if ($shouldBuild -eq 's') {
    Write-Log "`nEjecutando dotnet build para validar..." "INFO"
    $buildResult = dotnet build "$($solutionFile.FullName)" --nologo --verbosity minimal 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Log "La compilacion fallo. Detalles:" "ERROR"
        $buildResult | ForEach-Object {
            Write-Log $_ "ERROR" $true
            Add-Content -Path $buildErrorLog -Value $_
        }
        Write-Log "Los errores de compilacion se han guardado en: $buildErrorLog" "ERROR"
        exit 1
    } else {
        Write-Log "Compilacion exitosa. Todos los cambios son validos." "SUCCESS"
    }
} else {
    Write-Log "Compilacion omitida por el usuario." "INFO"
}