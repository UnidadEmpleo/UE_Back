#Requires -RunAsAdministrator

<#
.SYNOPSIS
    Instala .NET 9 Hosting Bundle para IIS
#>

$ErrorActionPreference = "Continue"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Instalador de .NET 9 Hosting Bundle" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar administrador
$currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
$isAdmin = $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "ERROR: Requiere permisos de administrador" -ForegroundColor Red
    exit 1
}

# Buscar instalador en carpeta de descargas
$downloadPaths = @(
    "$env:USERPROFILE\Downloads\dotnet-hosting-*.exe",
    "$env:USERPROFILE\Downloads\windowsdesktop-runtime-*.exe",
    "C:\Temp\dotnet-hosting-*.exe"
)

Write-Host "Buscando instalador en carpeta de descargas..." -ForegroundColor Yellow
$installer = $null

foreach ($pattern in $downloadPaths) {
    $found = Get-ChildItem -Path $pattern -ErrorAction SilentlyContinue | 
             Sort-Object LastWriteTime -Descending | 
             Select-Object -First 1
    
    if ($found) {
        $installer = $found.FullName
        Write-Host "  Encontrado: $installer" -ForegroundColor Green
        break
    }
}

if (-not $installer) {
    Write-Host ""
    Write-Host "No se encontro instalador descargado." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "INSTRUCCIONES:" -ForegroundColor Cyan
    Write-Host "1. Se abrira la pagina de descarga de .NET 9" -ForegroundColor White
    Write-Host "2. Busca la seccion: 'ASP.NET Core Runtime 9.0.x'" -ForegroundColor White
    Write-Host "3. Descarga: 'Hosting Bundle' (Windows x64)" -ForegroundColor White
    Write-Host "4. Una vez descargado, vuelve a ejecutar este script" -ForegroundColor White
    Write-Host ""
    
    $response = Read-Host "Presiona Enter para abrir la pagina de descarga"
    
    # Abrir pagina oficial
    Start-Process "https://dotnet.microsoft.com/en-us/download/dotnet/9.0"
    
    Write-Host ""
    Write-Host "Esperando descarga..." -ForegroundColor Yellow
    Write-Host "Cuando termines de descargar, ejecuta de nuevo:" -ForegroundColor Cyan
    Write-Host "  .\Install-HostingBundle.ps1" -ForegroundColor White
    Write-Host ""
    
    exit 0
}

# Mostrar informacion del instalador
Write-Host ""
Write-Host "Instalador encontrado:" -ForegroundColor Cyan
Write-Host "  Archivo: $installer" -ForegroundColor White
Write-Host "  Tamano: $([math]::Round((Get-Item $installer).Length / 1MB, 2)) MB" -ForegroundColor White
Write-Host "  Fecha: $((Get-Item $installer).LastWriteTime)" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "Deseas instalar? (S/N)"
if ($confirm -notlike 'S*' -and $confirm -notlike 'Y*') {
    Write-Host "Instalacion cancelada" -ForegroundColor Yellow
    exit 0
}

# Detener IIS
Write-Host ""
Write-Host "Deteniendo IIS..." -ForegroundColor Yellow
try {
    net stop was /y 2>$null | Out-Null
    iisreset /stop 2>$null | Out-Null
    Start-Sleep -Seconds 3
    Write-Host "  IIS detenido" -ForegroundColor Green
} catch {
    Write-Host "  Advertencia: No se pudo detener IIS" -ForegroundColor Yellow
}

# Instalar
Write-Host ""
Write-Host "Instalando Hosting Bundle..." -ForegroundColor Yellow
Write-Host "  Esto puede tardar 5-10 minutos..." -ForegroundColor Gray
Write-Host "  Por favor espera..." -ForegroundColor Gray
Write-Host ""

$logPath = Join-Path $env:TEMP "dotnet-hosting-install.log"

try {
    $arguments = "/install /quiet /norestart /log `"$logPath`""
    
    $process = Start-Process -FilePath $installer `
                            -ArgumentList $arguments `
                            -Wait `
                            -PassThru `
                            -NoNewWindow
    
    Write-Host "Instalacion finalizada" -ForegroundColor Green
    Write-Host "  Codigo de salida: $($process.ExitCode)" -ForegroundColor White
    
    if ($process.ExitCode -eq 0) {
        Write-Host "  Estado: Exitosa" -ForegroundColor Green
    } elseif ($process.ExitCode -eq 3010) {
        Write-Host "  Estado: Exitosa (requiere reinicio de Windows)" -ForegroundColor Yellow
    } elseif ($process.ExitCode -eq 1638) {
        Write-Host "  Estado: Version mas reciente ya instalada" -ForegroundColor Yellow
    } else {
        Write-Host "  Ver log: $logPath" -ForegroundColor Gray
    }
    
    $needsRestart = $process.ExitCode -eq 3010
    
} catch {
    Write-Host "  ERROR: $_" -ForegroundColor Red
    Write-Host "  Ver log: $logPath" -ForegroundColor Gray
    exit 1
}

# Reiniciar IIS
Write-Host ""
Write-Host "Reiniciando IIS..." -ForegroundColor Yellow
try {
    iisreset /start | Out-Null
    Start-Sleep -Seconds 5
    Write-Host "  IIS reiniciado" -ForegroundColor Green
} catch {
    Write-Host "  Advertencia: Error al reiniciar IIS" -ForegroundColor Yellow
}

# Verificar instalacion
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "VERIFICACION DE INSTALACION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Write-Host ""
Write-Host "1. Modulo AspNetCoreModuleV2:" -ForegroundColor Yellow

$appcmd = "$env:SystemRoot\System32\inetsrv\appcmd.exe"
$modules = & $appcmd list modules 2>&1

if ($modules -match "AspNetCoreModuleV2") {
    Write-Host "   INSTALADO CORRECTAMENTE" -ForegroundColor Green
} else {
    Write-Host "   NO DETECTADO" -ForegroundColor Red
    if ($needsRestart) {
        Write-Host "   Reinicia Windows para completar instalacion" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "2. Runtimes ASP.NET Core:" -ForegroundColor Yellow
$runtimes = dotnet --list-runtimes 2>$null | Select-String "Microsoft.AspNetCore.App 9"
if ($runtimes) {
    $runtimes | ForEach-Object { Write-Host "   $_" -ForegroundColor Green }
} else {
    Write-Host "   No se encontro .NET 9" -ForegroundColor Red
}

Write-Host ""
Write-Host "3. Archivos del modulo:" -ForegroundColor Yellow
$moduleFiles = @(
    "C:\Program Files\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll",
    "C:\Windows\System32\inetsrv\aspnetcorev2.dll"
)

$filesFound = 0
foreach ($file in $moduleFiles) {
    if (Test-Path $file) {
        Write-Host "   Encontrado: $file" -ForegroundColor Green
        $filesFound++
    }
}

if ($filesFound -eq 0) {
    Write-Host "   No se encontraron archivos del modulo" -ForegroundColor Red
}

# Resumen final
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "INSTALACION COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

if ($needsRestart) {
    Write-Host "IMPORTANTE: Reinicia Windows para completar" -ForegroundColor Yellow
    Write-Host "  Restart-Computer -Force" -ForegroundColor Cyan
    Write-Host ""
}

Write-Host "Proximo paso:" -ForegroundColor Yellow
Write-Host "  .\Deploy-BaseWebApp.ps1 -UseSelfSignedCert" -ForegroundColor Cyan
Write-Host ""