#!/usr/bin/env bash
set -e

echo "=== Digital Banking Suite: Developer Bootstrap (bash) ==="

echo
echo "[1/3] Checking basic tool availability..."
dotnet --version || echo "dotnet not found"
node --version || echo "node not found"
npm --version || echo "npm not found"
git --version || echo "git not found"
docker --version || echo "docker not found"

echo
echo "[2/3] Restoring backend and frontend dependencies (if projects exist)..."

if [ -d "./src/backend" ]; then
  echo "-> Backend folder found, running dotnet restore..."
  dotnet restore ./src/backend || true
else
  echo "-> Backend folder not found yet. Will be created in later chapters."
fi

if [ -d "./src/frontend" ]; then
  echo "-> Frontend folder found, running npm install (via Angular/Nx workspace)..."
  cd ./src/frontend
  npm install || true
  cd - > /dev/null
else
  echo "-> Frontend folder not found yet. Will be created in later chapters."
fi

echo
echo "[3/3] Starting local infrastructure with Docker Compose (if defined)..."

if [ -f "./infra/docker-compose.yml" ]; then
  docker compose -f ./infra/docker-compose.yml up -d
else
  echo "-> No docker-compose.yml found yet. Infrastructure will be added later."
fi

echo
echo "Bootstrap complete. You are ready to follow the next chapters."
