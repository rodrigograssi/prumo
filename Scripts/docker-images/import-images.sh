#/bin/bash
echo "=== Importando Imagens Docker do Prumo ==="
echo "Build Number: 20250609180128"
echo "Git Commit: 3fa981f1"
echo ""
if [ -f "prumo-api-20250609180128.tar" ]; then
    echo "[INFO] Importando prumo/api..."
    docker load < prumo-api-20250609180128.tar
    echo "[OK] API importada"
else
    echo "[ERROR] Arquivo prumo-api-20250609180128.tar nao encontrado"
    exit 1
fi
echo ""
if [ -f "prumo-web-20250609180128.tar" ]; then
    echo "[INFO] Importando prumo/web..."
    docker load < prumo-web-20250609180128.tar
    echo "[OK] Web importado"
else
    echo "[ERROR] Arquivo prumo-web-20250609180128.tar nao encontrado"
    exit 1
fi
echo ""
echo "=== Importacao Concluida ==="
echo "Imagens disponiveis:"
docker images | grep prumo
echo ""
echo "Proximos passos:"
echo "1. Configure Docker Secrets:"
echo "   ./setup-secrets.sh"
echo "2. Faca o deploy:"
echo "   docker stack deploy -c docker-compose.yml prumo"
