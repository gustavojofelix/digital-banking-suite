Write-Host "=== Digital Banking Suite: Developer Bootstrap (PowerShell) ==="

Write-Host "`n[1/3] Checking basic tool availability..."
dotnet --version
node --version
npm --version
git --version
docker --version

Write-Host "`n[2/3] Restoring backend and frontend dependencies (if projects exist)..."

if (Test-Path "./src/backend") {
    Write-Host "-> Backend folder found, running dotnet restore..."
    dotnet restore ./src/backend 2>$null
}
else {
    Write-Host "-> Backend folder not found yet. Will be created in later chapters."
}

if (Test-Path "./src/frontend") {
    Write-Host "-> Frontend folder found, running npm install (via Angular/Nx workspace)..."
    Push-Location ./src/frontend
    npm install
    Pop-Location
}
else {
    Write-Host "-> Frontend folder not found yet. Will be created in later chapters."
}

Write-Host "`n[3/3] Starting local infrastructure with Docker Compose (if defined)..."

if (Test-Path "./infra/docker-compose.yml") {
    docker compose -f ./infra/docker-compose.yml up -d
}
else {
    Write-Host "-> No docker-compose.yml found yet. Infrastructure will be added later."
}

Write-Host "`nBootstrap complete. You are ready to follow the next chapters."
