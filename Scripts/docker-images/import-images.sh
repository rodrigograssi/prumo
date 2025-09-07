#/bin/bash
echo "=== Importando Imagens Docker do PesquisaX ==="
echo "Build Number: 20250609180128"
echo "Git Commit: 3fa981f1"
echo ""
if [ -f "pesquisax-api-20250609180128.tar" ]; then
    echo "[INFO] Importando pesquisax/api..."
    docker load < pesquisax-api-20250609180128.tar
    echo "[OK] API importada"
else
    echo "[ERROR] Arquivo pesquisax-api-20250609180128.tar nao encontrado"
    exit 1
fi
echo ""
if [ -f "pesquisax-web-20250609180128.tar" ]; then
    echo "[INFO] Importando pesquisax/web..."
    docker load < pesquisax-web-20250609180128.tar
    echo "[OK] Web importado"
else
    echo "[ERROR] Arquivo pesquisax-web-20250609180128.tar nao encontrado"
    exit 1
fi
echo ""
echo "=== Importacao Concluida ==="
echo "Imagens disponiveis:"
docker images | grep pesquisax
echo ""
echo "Proximos passos:"
echo "1. Configure Docker Secrets:"
echo "   ./setup-secrets.sh"
echo "2. Faca o deploy:"
echo "   docker stack deploy -c docker-compose.yml pesquisax"
