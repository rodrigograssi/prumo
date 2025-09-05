# PesquisaX

Sistema completo para criação e aplicação de pesquisas personalizadas com visualização em radar charts. Desenvolvido em ASP.NET Core Web API + Blazor WebAssembly com persistência em PostgreSQL.

## 🚀 Funcionalidades

- ✅ **Criação de Pesquisas**: Interface intuitiva para criar pesquisas com questões agrupadas por categorias
- ✅ **Aplicação de Pesquisas**: Sistema de avaliação com escala de 0 a 10
- ✅ **Visualização em Radar Charts**: Gráficos lado a lado (Atual vs Ideal 100%) 
- ✅ **Persistência em PostgreSQL**: Dados armazenados com segurança em banco relacional
- ✅ **API REST Completa**: Endpoints documentados com Swagger/OpenAPI
- ✅ **Interface Responsiva**: Blazor WebAssembly com MudBlazor

## 🛠️ Stack Tecnológico

- **Backend**: ASP.NET Core Web API 9.0
- **Frontend**: Blazor WebAssembly 9.0
- **Database**: PostgreSQL com Entity Framework Core 9.0
- **UI Framework**: MudBlazor 8.12.0
- **Charts**: Chart.js via JavaScript Interop
- **ORM**: Entity Framework Core com Npgsql

## 🏗️ Arquitetura

```
PesquisaX/
├── Api/                           # ASP.NET Core Web API
│   ├── Controllers/
│   │   └── PesquisasController.cs # CRUD completo de pesquisas
│   ├── Program.cs                 # Configuração da API
│   └── appsettings.json          # Connection string e configurações
├── Web/                          # Blazor WebAssembly
│   ├── Components/
│   │   ├── Charts/RadarChart.razor
│   │   ├── Forms/ (SurveyForm, QuestionGroupForm, QuestionForm)
│   │   └── Layout/MainLayout.razor
│   ├── Services/ (ISurveyService, LocalStorageService)
│   └── Pages/ (Home, SurveyList, CreateSurvey, ApplySurvey, ShowResult)
├── Infra/                        # Entity Framework + Repositórios
│   ├── Contexto/
│   │   └── PesquisaXContexto.cs  # DbContext configurado
│   └── Repositorios/
│       └── RepositorioPesquisa.cs # Repository pattern
├── Shared/                       # Modelos compartilhados (DTOs)
│   ├── Pesquisa.cs              # Modelo principal
│   ├── GrupoQuestao.cs          # Grupos de questões
│   ├── Questao.cs               # Questões individuais
│   ├── ResultadoPessoa.cs       # Resultados de avaliações
│   └── Resposta.cs              # Respostas individuais
└── PesquisaX.sln                # Solução completa
```

## 📊 Modelo de Dados

### Entidades Principais

- **Pesquisa**: Contém nome, data de criação, grupos de questões e resultados
- **GrupoQuestao**: Agrupa questões por categoria com cálculo automático de peso (1/quantidade)
- **Questao**: Possui texto e flag `EInvertida` para pontuação reversa (10 - valor)
- **ResultadoPessoa**: Armazena avaliações individuais com percentuais calculados por grupo
- **Resposta**: Relaciona questão com valores bruto e processado

### Sistema de Pontuação

- Cada questão em um grupo tem peso igual (1/quantidade de questões)
- Questões invertidas: `valorFinal = 10 - valorBruto`
- Percentual do grupo: `(soma dos valores processados * peso * 10)%`

## 🚦 Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)

## ⚡ Instalação e Execução

### 1. Clone o repositório
```bash
git clone git@github.com:rodrigograssi/pesquisax.git
cd pesquisax
```

### 2. Configure o PostgreSQL
Certifique-se de que o PostgreSQL está rodando com as seguintes configurações:
- **Host**: localhost
- **Database**: pesquisax  
- **Username**: postgres
- **Password**: postgres
- **Port**: 5432

### 3. Execute a API
```bash
cd Api
dotnet run
```
A API estará disponível em: https://localhost:7001

### 4. Execute o Frontend (em outro terminal)
```bash
cd Web
dotnet run
```
O frontend estará disponível em: https://localhost:5001

### 5. Acesse a aplicação
- **Frontend**: https://localhost:5001
- **API Swagger**: https://localhost:7001

## 🔧 Comandos Úteis

### Desenvolvimento
```bash
# Compilar todos os projetos
dotnet build

# Executar apenas a API
dotnet run --project Api

# Executar apenas o Web
dotnet run --project Web
```

### Database
```bash
# As migrations são aplicadas automaticamente na inicialização
# Connection string padrão no appsettings.json
```

## 📡 API Endpoints

Base URL: `https://localhost:7001/api`

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/pesquisas` | Listar todas as pesquisas |
| GET | `/pesquisas/{id}` | Obter pesquisa por ID |
| POST | `/pesquisas` | Criar nova pesquisa |
| PUT | `/pesquisas/{id}` | Atualizar pesquisa |
| DELETE | `/pesquisas/{id}` | Excluir pesquisa |
| POST | `/pesquisas/{id}/resultados` | Salvar resultado |
| GET | `/pesquisas/{id}/resultados` | Obter resultados |

## 🎯 Fluxo de Uso

1. **Criar Pesquisa**: Defina questões agrupadas por categorias
2. **Aplicar Pesquisa**: Responda questões com escala 0-10
3. **Visualizar Resultados**: Gráficos radar comparativos
4. **Gerenciar Dados**: CRUD completo via interface ou API

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch: `git checkout -b feature/nova-funcionalidade`
3. Commit: `git commit -m 'Add: nova funcionalidade'`
4. Push: `git push origin feature/nova-funcionalidade`
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

---

Desenvolvido com ❤️ por [Rodrigo Grassi](https://github.com/rodrigograssi)