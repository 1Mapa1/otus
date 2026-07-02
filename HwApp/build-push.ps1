# Сборка и push Docker-образов HwApp (8 MS × Api + Migration = 16 образов).
#
# Примеры:
#   .\build-push.ps1                    # build + push, тег 10.0
#   .\build-push.ps1 -Action build      # только сборка
#   .\build-push.ps1 -Action push       # только push (образы уже собраны)
#   .\build-push.ps1 -Tag 10.1          # другой тег
#   .\build-push.ps1 -Service catalog    # один сервис (slug: auth, catalog, order, ...)
#
# Перед push: docker login

#Requires -Version 5.1
[CmdletBinding()]
param(
    [string]$Registry = "maslovdeveloper",
    [string]$Tag = "10.0",
    [string]$Platform = "linux/amd64",
    [ValidateSet("build", "push", "all")]
    [string]$Action = "all",
    [string[]]$Service = @()
)

$ErrorActionPreference = "Stop"

$ServiceDefinitions = @(
    @{ Folder = "AuthService"; Slug = "auth" }
    @{ Folder = "CustomerService"; Slug = "customer" }
    @{ Folder = "CatalogService"; Slug = "catalog" }
    @{ Folder = "BillingService"; Slug = "billing" }
    @{ Folder = "WarehouseService"; Slug = "warehouse" }
    @{ Folder = "DeliveryService"; Slug = "delivery" }
    @{ Folder = "NotificationService"; Slug = "notification" }
    @{ Folder = "OrderService"; Slug = "order" }
)

function Invoke-Docker {
    param([string[]]$Arguments)

    Write-Host ">> docker $($Arguments -join ' ')" -ForegroundColor Cyan
    & docker @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "docker failed with exit code $LASTEXITCODE"
    }
}

$selected = if ($Service.Count -gt 0) {
    $ServiceDefinitions | Where-Object { $_.Slug -in $Service }
} else {
    $ServiceDefinitions
}

if (-not $selected) {
    throw "No services matched -Service filter. Slugs: auth, customer, catalog, billing, warehouse, delivery, notification, order"
}

Write-Host "Registry: $Registry  Tag: $Tag  Platform: $Platform  Action: $Action"
Write-Host "Services: $($selected.Slug -join ', ')"
Write-Host ""

foreach ($definition in $selected) {
    $context = Join-Path $PSScriptRoot $definition.Folder
    if (-not (Test-Path $context)) {
        throw "Service folder not found: $context"
    }

    $apiImage = "${Registry}/hwapp-$($definition.Slug)-service:${Tag}"
    $migrationImage = "${Registry}/hwapp-$($definition.Slug)-migration:${Tag}"
    $apiDockerfile = Join-Path $context "Dockerfile.Api"
    $migrationDockerfile = Join-Path $context "Dockerfile.Migration"

    foreach ($dockerfile in @($apiDockerfile, $migrationDockerfile)) {
        if (-not (Test-Path $dockerfile)) {
            throw "Dockerfile not found: $dockerfile"
        }
    }

    Write-Host "=== $($definition.Folder) ===" -ForegroundColor Green

    if ($Action -eq "build" -or $Action -eq "all") {
        Invoke-Docker @(
            "build", "--platform", $Platform,
            "-f", $apiDockerfile,
            "-t", $apiImage,
            $context
        )
        Invoke-Docker @(
            "build", "--platform", $Platform,
            "-f", $migrationDockerfile,
            "-t", $migrationImage,
            $context
        )
    }

    if ($Action -eq "push" -or $Action -eq "all") {
        Invoke-Docker @("push", $apiImage)
        Invoke-Docker @("push", $migrationImage)
    }
}

Write-Host ""
Write-Host "Done. $($selected.Count * 2) image(s) per action scope." -ForegroundColor Green
