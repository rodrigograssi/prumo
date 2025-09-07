# Segurança dos Endpoints da API

## Endpoints Públicos (Sem Autenticação)

### AuthController
- `POST /api/auth/login` - Realizar login
- `POST /api/auth/registro` - Registrar novo usuário
- `GET /api/auth/has-users` - Verificar se existem usuários no sistema

## Endpoints Protegidos (Requerem JWT Token)

### PesquisasController - [Authorize]
- `GET /api/pesquisas` - Listar todas as pesquisas
- `GET /api/pesquisas/{id}` - Obter pesquisa por ID
- `POST /api/pesquisas` - Criar nova pesquisa
- `PUT /api/pesquisas/{id}` - Atualizar pesquisa existente
- `DELETE /api/pesquisas/{id}` - Excluir pesquisa
- `POST /api/pesquisas/{id}/resultados` - Salvar resultado de pessoa
- `GET /api/pesquisas/{id}/resultados` - Obter resultados da pesquisa
- `GET /api/pesquisas/{id}/resultados/{pessoaId}` - Obter resultado específico

### UsuariosController - [Authorize]
- `GET /api/usuarios` - Listar todos os usuários
- `GET /api/usuarios/{id}` - Obter usuário por ID
- `POST /api/usuarios` - Criar novo usuário
- `PUT /api/usuarios/{id}` - Atualizar usuário
- `DELETE /api/usuarios/{id}` - Excluir usuário

## Como Usar

### 1. Autenticação
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "usuario@exemplo.com",
  "senha": "suasenha"
}
```

### 2. Usando o Token
```http
GET /api/pesquisas
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Configuração JWT

- **Algoritmo**: HMAC SHA256
- **Duração**: 7 dias
- **Claims incluídos**:
  - `NameIdentifier`: ID do usuário
  - `Name`: Nome do usuário
  - `Email`: Email do usuário

## Logs de Segurança

Todos os endpoints protegidos registram:
- Usuário que fez a requisição
- Ação executada
- Timestamp da operação
- Erros e tentativas de acesso não autorizado

## Swagger UI

A documentação interativa da API inclui autenticação JWT:
1. Acesse `/swagger`
2. Clique em "Authorize"
3. Insira: `Bearer {seu-token-jwt}`
4. Teste os endpoints protegidos