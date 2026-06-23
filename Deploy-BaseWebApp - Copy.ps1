#Requires -RunAsAdministrator

<#
.SYNOPSIS
    Script de despliegue automatico para BaseWebApp en IIS
.DESCRIPTION
    Despliega aplicacion Vue 3 (Frontend) y .NET 9 API (Backend) en IIS con SSL
.EXAMPLE
    .\Deploy-BaseWebApp.ps1 -UseSelfSignedCert
#>

[CmdletBinding()]
param(
    [string]$DomainName = "clinica.cusaem.gob.mx",
    [string]$ApiDomainName = "api.clinica.cusaem.gob.mx",
    [int]$ApiPortHttp = 5000,
    [int]$ApiPortHttps = 5003,
    [int]$FrontendPortHttp = 80,
    [int]$FrontendPortHttps = 443,
    [string]$SqlServer = "localhost",
    [string]$DatabaseName = "BaseWebAppDB",
    [string]$SqlUser = "",
    [string]$SqlPassword = "",
    [switch]$CreateDatabase,
    [switch]$UseSelfSignedCert,
    [string]$CertificatePath = "",
    [SecureString]$CertificatePassword,
    [string]$RepoPath = (Get-Location).Path,
    [string]$DeployPath = "C:\inetpub\wwwroot\BaseWebApp"
)

$ErrorActionPreference = "Stop"
$script:LogFile = "$PSScriptRoot\deploy-log-$(Get-Date -Format 'yyyyMMdd-HHmmss').txt"

#region Helper Functions

function Write-Log {
    param(
        [string]$Message,
        [ValidateSet('INFO','SUCCESS','WARNING','ERROR')]
        [string]$Level = 'INFO'
    )
    
    $timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    $color = switch($Level) {
        'INFO' { 'Cyan' }
        'SUCCESS' { 'Green' }
        'WARNING' { 'Yellow' }
        'ERROR' { 'Red' }
    }
    
    $logMessage = "[$timestamp] [$Level] $Message"
    Write-Host $logMessage -ForegroundColor $color
    Add-Content -Path $script:LogFile -Value $logMessage
}

function Test-Administrator {
    $currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Get-WindowsVersion {
    $os = Get-CimInstance Win32_OperatingSystem
    $version = [System.Environment]::OSVersion.Version
    
    if ($os.Caption -like "*Server*") {
        return "Windows Server"
    } elseif ($version.Major -eq 10 -and $version.Build -ge 22000) {
        return "Windows 11"
    } elseif ($version.Major -eq 10) {
        return "Windows 10"
    } else {
        return "Unknown Windows Version"
    }
}

function Test-Prerequisites {
    Write-Log "Verificando prerequisitos..." "INFO"
    
    $osVersion = Get-WindowsVersion
    Write-Log "Sistema operativo: $osVersion" "INFO"
    
    $iisFeature = Get-WindowsOptionalFeature -Online -FeatureName IIS-WebServer -ErrorAction SilentlyContinue
    if (-not $iisFeature -or $iisFeature.State -ne 'Enabled') {
        Write-Log "IIS no esta instalado. Instalando..." "WARNING"
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer -All -NoRestart
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerManagementTools -All -NoRestart
    }
    Write-Log "IIS instalado correctamente" "SUCCESS"
    
    Write-Log "Verificando .NET 9..." "INFO"
    $dotnetVersion = dotnet --list-runtimes 2>$null | Select-String "Microsoft.AspNetCore.App 9"
    if (-not $dotnetVersion) {
        Write-Log ".NET 9 no encontrado. Debe instalar .NET 9 Hosting Bundle" "ERROR"
        Write-Log "Descargue desde: https://dotnet.microsoft.com/download/dotnet/9.0" "ERROR"
        throw ".NET 9 no instalado"
    }
    Write-Log ".NET 9 detectado: $dotnetVersion" "SUCCESS"
    
    Write-Log "Verificando Node.js..." "INFO"
    try {
        $nodeVersion = node --version 2>$null
        Write-Log "Node.js version: $nodeVersion" "SUCCESS"
    } catch {
        Write-Log "Node.js no encontrado. Debe instalar Node.js manualmente" "ERROR"
        Write-Log "Descargue desde: https://nodejs.org/" "ERROR"
        throw "Node.js no instalado"
    }
    
    $appcmd = "$env:SystemRoot\System32\inetsrv\appcmd.exe"
    $hasModule = (& $appcmd list modules) -match "AspNetCoreModuleV2"
    if (-not $hasModule) {
        Write-Log "AspNetCoreModuleV2 no encontrado. Instale .NET Hosting Bundle" "ERROR"
        throw "AspNetCoreModuleV2 no instalado"
    }
    Write-Log "AspNetCoreModuleV2 instalado" "SUCCESS"
    
    if ($CreateDatabase) {
        Write-Log "Verificando conectividad con SQL Server..." "INFO"
        try {
            $testConnection = "Server=$SqlServer;Database=master;Integrated Security=True;TrustServerCertificate=True;"
            $conn = New-Object System.Data.SqlClient.SqlConnection($testConnection)
            $conn.Open()
            $conn.Close()
            Write-Log "Conexion a SQL Server exitosa" "SUCCESS"
        } catch {
            Write-Log "Error conectando a SQL Server: $_" "ERROR"
            throw "No se puede conectar a SQL Server"
        }
    }
}

function Install-Certificate {
    Write-Log "Configurando certificado SSL..." "INFO"
    
    if ($UseSelfSignedCert) {
        Write-Log "Generando certificado auto-firmado..." "INFO"
        
        $cert = New-SelfSignedCertificate -DnsName $DomainName, $ApiDomainName, "localhost" -CertStoreLocation "Cert:\LocalMachine\My" -NotAfter (Get-Date).AddYears(2) -KeyLength 2048 -KeyAlgorithm RSA -HashAlgorithm SHA256 -FriendlyName "BaseWebApp SSL Certificate"
        
        $rootStore = Get-Item "Cert:\LocalMachine\Root"
        $rootStore.Open("ReadWrite")
        $rootStore.Add($cert)
        $rootStore.Close()
        
        $script:CertThumbprint = $cert.Thumbprint
        Write-Log "Certificado generado: $($script:CertThumbprint)" "SUCCESS"
        
    } elseif ($CertificatePath) {
        Write-Log "Instalando certificado desde archivo..." "INFO"
        
        $certPassword = $CertificatePassword
        if (-not $certPassword) {
            $certPassword = Read-Host "Ingrese la contrasena del certificado" -AsSecureString
        }
        
        $cert = Import-PfxCertificate -FilePath $CertificatePath -CertStoreLocation "Cert:\LocalMachine\My" -Password $certPassword
        
        $script:CertThumbprint = $cert.Thumbprint
        Write-Log "Certificado instalado: $($script:CertThumbprint)" "SUCCESS"
    } else {
        Write-Log "No se especifico certificado. Solo se configurara HTTP" "WARNING"
        $script:CertThumbprint = $null
    }
}

function Add-SslBinding {
    param(
        [string]$SiteName,
        [int]$Port,
        [string]$HostName
    )
    
    if (-not $script:CertThumbprint) {
        Write-Log "No hay certificado disponible para $SiteName" "WARNING"
        return
    }
    
    Write-Log "Configurando SSL binding para $SiteName en puerto $Port" "INFO"
    
    $appcmd = "$env:SystemRoot\System32\inetsrv\appcmd.exe"
    
    & $appcmd set site /site.name:"$SiteName" /"+bindings.[protocol='https',bindingInformation='*:${Port}:${HostName}']" 2>&1 | Out-Null
    
    $cleanThumbprint = $script:CertThumbprint -replace '\s',''
    
    netsh http delete sslcert "ipport=0.0.0.0:$Port" 2>$null | Out-Null
    
    $appId = [Guid]::NewGuid().ToString()
    $result = netsh http add sslcert "ipport=0.0.0.0:$Port" "certhash=$cleanThumbprint" "appid={$appId}" certstorename=MY 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Log "SSL configurado correctamente en puerto $Port" "SUCCESS"
    } else {
        Write-Log "Advertencia al configurar SSL: $result" "WARNING"
    }
}

function Update-HostsFile {
    Write-Log "Actualizando archivo hosts..." "INFO"
    
    $hostsPath = "$env:SystemRoot\System32\drivers\etc\hosts"
    $hostsContent = Get-Content $hostsPath
    
    $entries = @(
        "127.0.0.1 $DomainName",
        "127.0.0.1 $ApiDomainName"
    )
    
    foreach ($entry in $entries) {
        if ($hostsContent -notcontains $entry) {
            Add-Content -Path $hostsPath -Value $entry
            Write-Log "Agregada entrada: $entry" "INFO"
        }
    }
    
    Write-Log "Archivo hosts actualizado" "SUCCESS"
}

function Build-Frontend {
    Write-Log "Compilando Frontend (Vue 3)..." "INFO"
    
    $frontendPath = Join-Path $RepoPath "SIPRE-WEB"
    
    if (-not (Test-Path $frontendPath)) {
        throw "No se encontro la carpeta SIPRE-WEB en $RepoPath"
    }
    
    Push-Location $frontendPath
    
    try {
        if (-not (Test-Path "package.json")) {
            throw "No se encontro package.json en $frontendPath"
        }
        
        Write-Log "Limpiando instalaciones previas..." "INFO"
        
        # Limpiar archivos locales
        @('node_modules', 'package-lock.json', 'dist') | ForEach-Object {
            if (Test-Path $_) {
                Remove-Item $_ -Recurse -Force -ErrorAction SilentlyContinue
            }
        }
        
        # Configurar variables de entorno para npm install
        $env:NODE_OPTIONS = "--max-old-space-size=4096"
        
        Write-Log "Instalando dependencias npm (incluyendo devDependencies)..." "INFO"
        Write-Log "Esto puede tardar 5-10 minutos..." "INFO"
        
        # Instalar TODAS las dependencias
        npm install --legacy-peer-deps --include=dev
        
        if ($LASTEXITCODE -ne 0) {
            throw "npm install fallo con codigo: $LASTEXITCODE"
        }
        
        # Verificar instalacion
        if (-not (Test-Path "node_modules")) {
            throw "node_modules no fue creado"
        }
        
        # Verificar que vue-cli-service se instalo
        $vueCliPaths = @(
            "node_modules\.bin\vue-cli-service.cmd",
            "node_modules\.bin\vue-cli-service",
            "node_modules\@vue\cli-service\bin\vue-cli-service.js"
        )
        
        $vueCliFound = $false
        foreach ($path in $vueCliPaths) {
            if (Test-Path $path) {
                $vueCliFound = $true
                Write-Log "vue-cli-service encontrado en: $path" "SUCCESS"
                break
            }
        }
        
        if (-not $vueCliFound) {
            Write-Log "vue-cli-service no fue instalado. Instalando manualmente..." "WARNING"
            npm install --save-dev @vue/cli-service
            
            if ($LASTEXITCODE -ne 0) {
                throw "No se pudo instalar @vue/cli-service"
            }
        }
        
        $modulesCount = (Get-ChildItem "node_modules" -Directory).Count
        Write-Log "Dependencias instaladas: $modulesCount paquetes" "SUCCESS"
        
        # ============================================
        # CONFIGURAR VARIABLES DE ENTORNO PARA PRODUCCION
        # ============================================
        Write-Log "Configurando variables de entorno para produccion..." "INFO"
        
        # Leer archivo .env existente si existe
        $envVars = @{}
        if (Test-Path ".env") {
            Write-Log "Leyendo configuracion de .env..." "INFO"
            Get-Content ".env" | ForEach-Object {
                if ($_ -match '^\s*([^#][^=]+)\s*=\s*(.+)\s*$') {
                    $key = $matches[1].Trim()
                    $value = $matches[2].Trim().Trim('"').Trim("'")
                    $envVars[$key] = $value
                }
            }
        }
        
        # Actualizar URLs de API para produccion
        $apiBaseUrl = "https://${ApiDomainName}:${ApiPortHttps}/api"
        $redirectUri = "https://${DomainName}/pages/users/user-info"
        $postLogoutRedirectUri = "https://${DomainName}/"
        
        Write-Log "URL API configurada: $apiBaseUrl" "INFO"
        Write-Log "URL Frontend configurada: https://${DomainName}" "INFO"
        
        # Crear .env.production con todas las variables
        $envProductionContent = @"
# Configuracion de Azure AD / MSAL
VUE_APP_MSAL_CLIENT_ID="$($envVars['VUE_APP_MSAL_CLIENT_ID'])"
VUE_APP_MSAL_AUTHORITY="$($envVars['VUE_APP_MSAL_AUTHORITY'])"
VUE_APP_MSAL_REDIRECT_URI="$redirectUri"
VUE_APP_MSAL_POST_LOGOUT_REDIRECT_URI="$postLogoutRedirectUri"

# Azure API Management
VUE_APP_OCP_HEADER="$($envVars['VUE_APP_OCP_HEADER'])"
VUE_APP_OCP_KEY="$($envVars['VUE_APP_OCP_KEY'])"

# URLs de API
VUE_APP_API_BASE_URL="$apiBaseUrl"
VUE_APP_API_BASE_URL_PROD="$apiBaseUrl"
VUE_APP_API_AZURE_URL="$apiBaseUrl/AuthAzure"

# Azure AD
VUE_APP_AZURE_DOMAIN="$($envVars['VUE_APP_AZURE_DOMAIN'])"
VUE_APP_SCOPE_API="$($envVars['VUE_APP_SCOPE_API'])"

# Storage
VUE_APP_STORAGE_SECRET="$($envVars['VUE_APP_STORAGE_SECRET'])"

# Ambiente
VUE_APP_ENVIRONMENT=production
NODE_ENV=production
"@
        
        # Guardar .env.production
        $envProductionContent | Out-File -FilePath ".env.production" -Encoding UTF8 -Force
        Write-Log "Archivo .env.production creado con configuracion de produccion" "SUCCESS"
        
        # Mostrar configuracion para verificacion
        Write-Log "Configuracion de produccion:" "INFO"
        Write-Log "  - Client ID: $($envVars['VUE_APP_MSAL_CLIENT_ID'])" "INFO"
        Write-Log "  - Authority: $($envVars['VUE_APP_MSAL_AUTHORITY'])" "INFO"
        Write-Log "  - Redirect URI: $redirectUri" "INFO"
        Write-Log "  - API URL: $apiBaseUrl" "INFO"
        Write-Log "  - Azure Domain: $($envVars['VUE_APP_AZURE_DOMAIN'])" "INFO"
        
        # AHORA SI establecer NODE_ENV para el build
        $env:NODE_ENV = "production"
        
        # ============================================
        # COMPILAR APLICACION
        # ============================================
        Write-Log "Compilando aplicacion Vue..." "INFO"
        Write-Log "Esto puede tardar 5-10 minutos..." "INFO"
        Write-Log "Warnings de ESLint son normales y no afectan la compilacion" "INFO"
        
        # Ejecutar build
        npm run build
        $buildExitCode = $LASTEXITCODE
        
        Start-Sleep -Seconds 2
        
        # Verificar resultado
        $distPath = Join-Path $frontendPath "dist"
        
        if (-not (Test-Path $distPath)) {
            Write-Log "No se genero la carpeta dist" "ERROR"
            Write-Log "Codigo de salida: $buildExitCode" "ERROR"
            
            # Intentar con npx como alternativa
            Write-Log "Intentando con npx vue-cli-service build..." "WARNING"
            npx vue-cli-service build
            
            Start-Sleep -Seconds 2
            
            if (-not (Test-Path $distPath)) {
                throw "No se pudo generar la carpeta dist con ningun metodo"
            }
        }
        
        $distFiles = Get-ChildItem -Path $distPath -Recurse -File -ErrorAction SilentlyContinue
        
        if ($distFiles.Count -eq 0) {
            throw "La carpeta dist esta vacia"
        }
        
        $script:FrontendDistPath = $distPath
        $totalSize = ($distFiles | Measure-Object -Property Length -Sum).Sum / 1MB
        
        Write-Log "Compilacion exitosa!" "SUCCESS"
        Write-Log "Archivos generados: $($distFiles.Count)" "SUCCESS"
        Write-Log "Tamano total: $([math]::Round($totalSize, 2)) MB" "SUCCESS"
        
        # Verificar que los archivos contienen las variables de entorno
        $indexPath = Join-Path $distPath "index.html"
        if (Test-Path $indexPath) {
            Write-Log "Archivo index.html generado correctamente" "SUCCESS"
        }
        
        # Listar archivos JavaScript principales
        $jsFiles = Get-ChildItem -Path "$distPath\js" -Filter "*.js" -ErrorAction SilentlyContinue | Select-Object -First 3
        if ($jsFiles) {
            Write-Log "Archivos JavaScript generados:" "INFO"
            foreach ($file in $jsFiles) {
                $size = [math]::Round($file.Length / 1KB, 2)
                Write-Log "  - $($file.Name) ($size KB)" "INFO"
            }
        }
        
    } catch {
        Write-Log "Error durante compilacion del Frontend: $_" "ERROR"
        
        # Diagnostico detallado
        Write-Log "Diagnostico:" "INFO"
        
        if (Test-Path "package.json") {
            $pkg = Get-Content "package.json" | ConvertFrom-Json
            Write-Log "  Script build: $($pkg.scripts.build)" "INFO"
        }
        
        if (Test-Path "node_modules") {
            $modulesCount = (Get-ChildItem "node_modules" -Directory -ErrorAction SilentlyContinue).Count
            Write-Log "  Paquetes instalados: $modulesCount" "INFO"
            
            if (Test-Path "node_modules\@vue\cli-service") {
                Write-Log "  @vue/cli-service: INSTALADO" "INFO"
            } else {
                Write-Log "  @vue/cli-service: NO INSTALADO" "ERROR"
            }
        }
        
        if (Test-Path ".env.production") {
            Write-Log "  .env.production: CREADO" "INFO"
        } else {
            Write-Log "  .env.production: NO CREADO" "ERROR"
        }
        
        throw
    } finally {
        Pop-Location
    }
}

function Configure-SqlPermissions {
    if (-not $CreateDatabase) {
        Write-Log "Configuracion de permisos SQL omitida (CreateDatabase no especificado)" "INFO"
        return
    }
    
    Write-Log "Configurando permisos SQL Server para Application Pool..." "INFO"
    
    $appPoolIdentity = "IIS APPPOOL\BaseWebApp-API-Pool"
    $databases = @("API", "Clinica")
    
    # Script SQL para crear login
    $createLoginScript = @"
USE [master]
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'$appPoolIdentity')
BEGIN
    CREATE LOGIN [$appPoolIdentity] FROM WINDOWS WITH DEFAULT_DATABASE=[master]
    PRINT 'Login creado: $appPoolIdentity'
END
ELSE
BEGIN
    PRINT 'Login ya existe: $appPoolIdentity'
END
GO
"@
    
    # Script para dar permisos en cada base de datos
    $grantPermissionsTemplate = @"
USE [{0}]
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'$appPoolIdentity')
BEGIN
    CREATE USER [$appPoolIdentity] FOR LOGIN [$appPoolIdentity]
    PRINT 'Usuario creado en {0}'
END

ALTER ROLE [db_datareader] ADD MEMBER [$appPoolIdentity]
ALTER ROLE [db_datawriter] ADD MEMBER [$appPoolIdentity]
ALTER ROLE [db_ddladmin] ADD MEMBER [$appPoolIdentity]

PRINT 'Permisos asignados en {0}'
GO
"@
    
    try {
        # Verificar que sqlcmd esta disponible
        $sqlcmdPath = Get-Command sqlcmd -ErrorAction SilentlyContinue
        
        if (-not $sqlcmdPath) {
            Write-Log "sqlcmd no encontrado. Permisos SQL deben configurarse manualmente" "WARNING"
            Write-Log "Ejecuta este comando en SQL Server Management Studio:" "WARNING"
            Write-Log "  CREATE LOGIN [$appPoolIdentity] FROM WINDOWS" "WARNING"
            return
        }
        
        Write-Log "Creando login en SQL Server..." "INFO"
        
        # Crear archivo temporal con el script
        $loginScriptFile = "$env:TEMP\create-sql-login.sql"
        $createLoginScript | Out-File -FilePath $loginScriptFile -Encoding UTF8 -Force
        
        # Ejecutar script
        sqlcmd -S $SqlServer -E -i $loginScriptFile 2>&1 | Out-Null
        
        if ($LASTEXITCODE -eq 0) {
            Write-Log "Login creado correctamente" "SUCCESS"
        } else {
            Write-Log "Advertencia al crear login (puede ya existir)" "WARNING"
        }
        
        # Dar permisos en cada base de datos
        foreach ($db in $databases) {
            Write-Log "Configurando permisos en base de datos: $db" "INFO"
            
            $dbScript = $grantPermissionsTemplate -f $db
            $dbScriptFile = "$env:TEMP\grant-permissions-$db.sql"
            $dbScript | Out-File -FilePath $dbScriptFile -Encoding UTF8 -Force
            
            sqlcmd -S $SqlServer -E -i $dbScriptFile 2>&1 | Out-Null
            
            if ($LASTEXITCODE -eq 0) {
                Write-Log "Permisos configurados en $db" "SUCCESS"
            } else {
                Write-Log "Advertencia al configurar permisos en $db" "WARNING"
            }
            
            Remove-Item $dbScriptFile -Force -ErrorAction SilentlyContinue
        }
        
        Remove-Item $loginScriptFile -Force -ErrorAction SilentlyContinue
        
        Write-Log "Configuracion de permisos SQL completada" "SUCCESS"
        
    } catch {
        Write-Log "Error al configurar permisos SQL: $_" "WARNING"
        Write-Log "Configura manualmente los permisos en SQL Server" "WARNING"
    }
}

function Publish-API {
    Write-Log "Publicando API (.NET 9)..." "INFO"
    
    $apiPath = Join-Path $RepoPath "API"
    
    if (-not (Test-Path $apiPath)) {
        throw "No se encontro la carpeta API en $RepoPath"
    }
    
    Push-Location $apiPath
    
    try {
        Write-Log "Compilando y publicando API..." "INFO"
        dotnet publish API.csproj -c Release -o "$env:TEMP\api-publish" 2>&1 | Out-Null
        
        if (-not (Test-Path "$env:TEMP\api-publish\API.dll")) {
            throw "No se genero API.dll"
        }
        
        Write-Log "API publicada exitosamente" "SUCCESS"
        
    } finally {
        Pop-Location
    }
}

function New-Database {
    if (-not $CreateDatabase) {
        Write-Log "Omitiendo creacion de base de datos" "INFO"
        return
    }
    
    Write-Log "Creando base de datos $DatabaseName..." "INFO"
    
    try {
        $connectionString = if ($SqlUser) {
            "Server=$SqlServer;Database=master;User Id=$SqlUser;Password=$SqlPassword;TrustServerCertificate=True;"
        } else {
            "Server=$SqlServer;Database=master;Integrated Security=True;TrustServerCertificate=True;"
        }
        
        $createDbScript = "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '$DatabaseName') BEGIN CREATE DATABASE [$DatabaseName]; END"
        
        $conn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $conn.Open()
        
        $cmd = New-Object System.Data.SqlClient.SqlCommand($createDbScript, $conn)
        $cmd.ExecuteNonQuery() | Out-Null
        
        $conn.Close()
        
        Write-Log "Base de datos $DatabaseName configurada" "SUCCESS"
        
    } catch {
        Write-Log "Error al crear base de datos: $_" "ERROR"
        throw
    }
}

function Deploy-ToIIS {
    Write-Log "Desplegando a IIS..." "INFO"
    
    $frontendDest = Join-Path $DeployPath "frontend"
    $apiDest = Join-Path $DeployPath "api"
    
    New-Item -ItemType Directory -Path $frontendDest -Force | Out-Null
    New-Item -ItemType Directory -Path $apiDest -Force | Out-Null
    New-Item -ItemType Directory -Path "$apiDest\logs" -Force | Out-Null
    
    Write-Log "Copiando archivos del Frontend..." "INFO"
    Copy-Item -Path "$script:FrontendDistPath\*" -Destination $frontendDest -Recurse -Force
    
    Write-Log "Copiando archivos del API..." "INFO"
    Copy-Item -Path "$env:TEMP\api-publish\*" -Destination $apiDest -Recurse -Force
    
    Write-Log "Creando web.config para API..." "INFO"
    $webConfigXml = '<?xml version="1.0" encoding="utf-8"?><configuration><location path="." inheritInChildApplications="false"><system.webServer><handlers><add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" /></handlers><aspNetCore processPath="dotnet" arguments=".\API.dll" stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout" hostingModel="InProcess"><environmentVariables><environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" /><environmentVariable name="ASPNETCORE_HTTPS_PORT" value="' + $ApiPortHttps + '" /></environmentVariables></aspNetCore></system.webServer></location></configuration>'
    
    [System.IO.File]::WriteAllText("$apiDest\web.config", $webConfigXml, [System.Text.Encoding]::ASCII)
    
    Write-Log "Archivos desplegados exitosamente" "SUCCESS"
}

function New-IISSites {
    Write-Log "Configurando sitios en IIS..." "INFO"
    
    $appcmd = "$env:SystemRoot\System32\inetsrv\appcmd.exe"
    
    $apiPoolName = "BaseWebApp-API-Pool"
    $frontendPoolName = "BaseWebApp-Frontend-Pool"
    $apiSiteName = "BaseWebApp-API"
    $frontendSiteName = "BaseWebApp-Frontend"
    
    # ============================================
    # CREAR APPLICATION POOLS
    # ============================================
    Write-Log "Creando Application Pools..." "INFO"
    
    # API Pool
    $apiPoolExists = & $appcmd list apppool $apiPoolName 2>$null
    if ($apiPoolExists) {
        Write-Log "Eliminando pool existente: $apiPoolName" "INFO"
        & $appcmd stop apppool /apppool.name:$apiPoolName 2>$null
        Start-Sleep -Seconds 2
        & $appcmd delete apppool $apiPoolName 2>$null
    }
    
    Write-Log "Creando Application Pool: $apiPoolName" "INFO"
    & $appcmd add apppool /name:$apiPoolName /managedRuntimeVersion:"" /managedPipelineMode:Integrated
    & $appcmd set apppool $apiPoolName /processModel.identityType:ApplicationPoolIdentity
    & $appcmd set apppool $apiPoolName /recycling.periodicRestart.time:00:00:00
    
    # Frontend Pool
    $frontendPoolExists = & $appcmd list apppool $frontendPoolName 2>$null
    if ($frontendPoolExists) {
        Write-Log "Eliminando pool existente: $frontendPoolName" "INFO"
        & $appcmd stop apppool /apppool.name:$frontendPoolName 2>$null
        Start-Sleep -Seconds 2
        & $appcmd delete apppool $frontendPoolName 2>$null
    }
    
    Write-Log "Creando Application Pool: $frontendPoolName" "INFO"
    & $appcmd add apppool /name:$frontendPoolName /managedRuntimeVersion:"" /managedPipelineMode:Integrated
    
    Write-Log "Application Pools creados correctamente" "SUCCESS"
    
    # ============================================
    # CREAR SITIOS WEB
    # ============================================
    Write-Log "Creando sitios web..." "INFO"
    
    # Detener y eliminar sitios existentes
    foreach ($siteName in @($apiSiteName, $frontendSiteName)) {
        $siteExists = & $appcmd list site $siteName 2>$null
        if ($siteExists) {
            Write-Log "Eliminando sitio existente: $siteName" "INFO"
            & $appcmd stop site /site.name:$siteName 2>$null
            Start-Sleep -Seconds 2
            & $appcmd delete site $siteName 2>$null
        }
    }
    
    # Crear sitio API
    Write-Log "Creando sitio: $apiSiteName" "INFO"
    & $appcmd add site /name:$apiSiteName /physicalPath:"$DeployPath\api" /bindings:"http/*:${ApiPortHttp}:$ApiDomainName"
    
    # IMPORTANTE: Asignar el Application Pool al sitio
    Write-Log "Asignando Application Pool $apiPoolName a $apiSiteName" "INFO"
    & $appcmd set app "$apiSiteName/" /applicationPool:$apiPoolName
    
    # Verificar asignación
    $apiAppInfo = & $appcmd list app /site.name:$apiSiteName
    if ($apiAppInfo -match $apiPoolName) {
        Write-Log "Application Pool asignado correctamente a $apiSiteName" "SUCCESS"
    } else {
        Write-Log "ADVERTENCIA: No se pudo verificar la asignacion del pool a $apiSiteName" "WARNING"
    }
    
    # Crear sitio Frontend
    Write-Log "Creando sitio: $frontendSiteName" "INFO"
    & $appcmd add site /name:$frontendSiteName /physicalPath:"$DeployPath\frontend" /bindings:"http/*:${FrontendPortHttp}:$DomainName"
    
    # IMPORTANTE: Asignar el Application Pool al sitio
    Write-Log "Asignando Application Pool $frontendPoolName a $frontendSiteName" "INFO"
    & $appcmd set app "$frontendSiteName/" /applicationPool:$frontendPoolName
    
    # Verificar asignación
    $frontendAppInfo = & $appcmd list app /site.name:$frontendSiteName
    if ($frontendAppInfo -match $frontendPoolName) {
        Write-Log "Application Pool asignado correctamente a $frontendSiteName" "SUCCESS"
    } else {
        Write-Log "ADVERTENCIA: No se pudo verificar la asignacion del pool a $frontendSiteName" "WARNING"
    }
    
    Write-Log "Sitios web creados correctamente" "SUCCESS"
    
    # ============================================
    # CONFIGURAR SSL
    # ============================================
    if ($script:CertThumbprint) {
        Write-Log "Configurando bindings SSL..." "INFO"
        Add-SslBinding -SiteName $apiSiteName -Port $ApiPortHttps -HostName $ApiDomainName
        Add-SslBinding -SiteName $frontendSiteName -Port $FrontendPortHttps -HostName $DomainName
    }
    
    # ============================================
    # CONFIGURAR PERMISOS
    # ============================================
    Write-Log "Configurando permisos del sistema de archivos..." "INFO"
    
    # Permisos para API
    $apiPoolIdentity = "IIS AppPool\$apiPoolName"
    Write-Log "Asignando permisos a: $apiPoolIdentity" "INFO"
    icacls "$DeployPath\api" /grant "${apiPoolIdentity}:(OI)(CI)F" /T /Q 2>$null | Out-Null
    
    # Permisos para Frontend
    $frontendPoolIdentity = "IIS AppPool\$frontendPoolName"
    Write-Log "Asignando permisos a: $frontendPoolIdentity" "INFO"
    icacls "$DeployPath\frontend" /grant "${frontendPoolIdentity}:(OI)(CI)R" /T /Q 2>$null | Out-Null
    
    Write-Log "Permisos configurados correctamente" "SUCCESS"
    
    # ============================================
    # INICIAR SITIOS
    # ============================================
    Write-Log "Iniciando sitios..." "INFO"
    
    foreach ($siteName in @($apiSiteName, $frontendSiteName)) {
        Write-Log "Iniciando sitio: $siteName" "INFO"
        & $appcmd start site /site.name:$siteName 2>$null
    }
    
    Start-Sleep -Seconds 2
    
    # ============================================
    # VERIFICACIÓN FINAL
    # ============================================
    Write-Log "Verificando configuracion..." "INFO"
    
    # Verificar sitios
    $apiSiteStatus = & $appcmd list site $apiSiteName
    $frontendSiteStatus = & $appcmd list site $frontendSiteName
    
    if ($apiSiteStatus -match "state:Started") {
        Write-Log "Sitio API: INICIADO" "SUCCESS"
    } else {
        Write-Log "Sitio API: NO INICIADO" "WARNING"
    }
    
    if ($frontendSiteStatus -match "state:Started") {
        Write-Log "Sitio Frontend: INICIADO" "SUCCESS"
    } else {
        Write-Log "Sitio Frontend: NO INICIADO" "WARNING"
    }
    
    # Verificar pools
    $apiPoolStatus = & $appcmd list apppool $apiPoolName
    $frontendPoolStatus = & $appcmd list apppool $frontendPoolName
    
    if ($apiPoolStatus -match "state:Started") {
        Write-Log "Application Pool API: INICIADO" "SUCCESS"
    } else {
        Write-Log "Application Pool API: NO INICIADO" "WARNING"
    }
    
    if ($frontendPoolStatus -match "state:Started") {
        Write-Log "Application Pool Frontend: INICIADO" "SUCCESS"
    } else {
        Write-Log "Application Pool Frontend: NO INICIADO" "WARNING"
    }
    
    Write-Log "Configuracion de IIS completada" "SUCCESS"
}

function Add-FirewallRules {
    Write-Log "Configurando reglas de firewall..." "INFO"
    
    $rules = @(
        @{Name="BaseWebApp-API-HTTP"; Port=$ApiPortHttp},
        @{Name="BaseWebApp-API-HTTPS"; Port=$ApiPortHttps},
        @{Name="BaseWebApp-Frontend-HTTP"; Port=$FrontendPortHttp},
        @{Name="BaseWebApp-Frontend-HTTPS"; Port=$FrontendPortHttps}
    )
    
    foreach ($rule in $rules) {
        $existing = Get-NetFirewallRule -DisplayName $rule.Name -ErrorAction SilentlyContinue
        if ($existing) {
            Remove-NetFirewallRule -DisplayName $rule.Name
        }
        
        New-NetFirewallRule -DisplayName $rule.Name -Direction Inbound -Action Allow -Protocol TCP -LocalPort $rule.Port | Out-Null
    }
    
    Write-Log "Reglas de firewall configuradas" "SUCCESS"
}

#endregion

#region Main Execution

try {
    Write-Log "========================================" "INFO"
    Write-Log "Iniciando despliegue de BaseWebApp" "INFO"
    Write-Log "========================================" "INFO"
    Write-Log "Fecha: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" "INFO"
    Write-Log "Usuario: $env:USERNAME" "INFO"
    Write-Log "" "INFO"
    
    if (-not (Test-Administrator)) {
        throw "Este script requiere permisos de administrador"
    }
    
    Test-Prerequisites
    Update-HostsFile
    Install-Certificate
    Build-Frontend
    Publish-API
    New-Database
    Deploy-ToIIS
    New-IISSites
    Add-FirewallRules
    
    Write-Log "Reiniciando IIS..." "INFO"
    iisreset /restart | Out-Null
    Start-Sleep -Seconds 5
    
    Write-Log "" "SUCCESS"
    Write-Log "========================================" "SUCCESS"
    Write-Log "DESPLIEGUE COMPLETADO EXITOSAMENTE" "SUCCESS"
    Write-Log "========================================" "SUCCESS"
    Write-Log "" "INFO"
    
    Write-Log "URLs de acceso:" "INFO"
    Write-Log "  Frontend HTTP:  http://${DomainName}" "INFO"
    if ($script:CertThumbprint) {
        Write-Log "  Frontend HTTPS: https://${DomainName}" "INFO"
    }
    Write-Log "  API HTTP:  http://${ApiDomainName}:${ApiPortHttp}/swagger" "INFO"
    if ($script:CertThumbprint) {
        Write-Log "  API HTTPS: https://${ApiDomainName}:${ApiPortHttps}/swagger" "INFO"
    }
    Write-Log "" "INFO"
    
    if ($UseSelfSignedCert) {
        Write-Log "NOTA: Certificado auto-firmado en uso" "WARNING"
        Write-Log "El navegador mostrara advertencias de seguridad" "WARNING"
    }
    
    Write-Log "" "INFO"
    Write-Log "Log guardado en: $script:LogFile" "INFO"
    
} catch {
    Write-Log "========================================" "ERROR"
    Write-Log "ERROR DURANTE EL DESPLIEGUE" "ERROR"
    Write-Log "========================================" "ERROR"
    Write-Log "Error: $_" "ERROR"
    Write-Log "Stack Trace: $($_.ScriptStackTrace)" "ERROR"
    Write-Log "" "ERROR"
    Write-Log "Log guardado en: $script:LogFile" "ERROR"
    
    throw
}

#endregion