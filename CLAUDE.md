# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Sistema completo para criação e aplicação de pesquisas personalizadas com visualização em radar charts. Migrado para arquitetura API + Blazor WebAssembly com persistência em PostgreSQL.

## Commands

### Build and Development
```bash
# API
cd Api && dotnet run                 # Executar API (https://localhost:7001)
cd Api && dotnet build              # Compilar API

# Web (Blazor)
cd Web && dotnet run                # Executar aplicação web
cd Web && dotnet build             # Compilar aplicação web

# Solução completa
dotnet build                        # Compilar todos os projetos
dotnet run --project Api           # Executar API
dotnet run --project Web           # Executar Web
```

### Database
```bash
# PostgreSQL deve estar rodando localmente
# Connection string padrão: Host=localhost;Database=pesquisax;Username=postgres;Password=postgres;Port=5432
```

## Architecture

### Technology Stack
- **Backend**: ASP.NET Core Web API 9.0
- **Frontend**: Blazor WebAssembly 9.0
- **Database**: PostgreSQL com Entity Framework Core 9.0
- **UI Framework**: MudBlazor 8.12.0
- **Charts**: Chart.js (via JavaScript Interop)
- **API Documentation**: Swagger/OpenAPI
- **ORM**: Entity Framework Core com Npgsql

### Project Structure
```
PesquisaX/
├── Api/                           # ASP.NET Core Web API
│   ├── Controllers/
│   │   └── PesquisasController.cs # CRUD completo de pesquisas
│   ├── Program.cs                 # Configuração da API
│   └── appsettings.json          # Connection string e configurações
├── Web/                          # Blazor WebAssembly (código original adaptado)
│   ├── Components/
│   │   ├── Charts/RadarChart.razor
│   │   ├── Forms/ (SurveyForm, QuestionGroupForm, QuestionForm)
│   │   └── Layout/MainLayout.razor
│   ├── Services/ (ISurveyService, LocalStorageService adaptados)
│   ├── Pages/ (Home, SurveyList, CreateSurvey, ApplySurvey, ShowResult)
│   └── wwwroot/
├── Infra/                        # Entity Framework + Repositórios
│   ├── Contexto/
│   │   └── PesquisaXContexto.cs  # DbContext configurado
│   └── Repositorios/
│       ├── IRepositorioBase.cs    # Interface base
│       ├── RepositorioBase.cs     # Implementação base
│       ├── IRepositorioPesquisa.cs
│       └── RepositorioPesquisa.cs # Repositório específico
├── Shared/                       # Modelos compartilhados (DTOs)
│   ├── Pesquisa.cs              # Modelo principal (Survey → Pesquisa)
│   ├── GrupoQuestao.cs          # QuestionGroup → GrupoQuestao
│   ├── Questao.cs               # Question → Questao
│   ├── ResultadoPessoa.cs       # PersonResult → ResultadoPessoa
│   └── Resposta.cs              # Answer → Resposta
└── PesquisaX.sln                # Solução com todos os projetos
```

### Key Models (em Português)

**Pesquisa**: Contém nome, data de criação, grupos de questões e resultados
**GrupoQuestao**: Agrupa questões por categoria com cálculo automático de peso (1/quantidade)
**Questao**: Possui texto e flag EInvertida para pontuação reversa (10 - valor)
**ResultadoPessoa**: Armazena avaliações individuais com percentuais calculados por grupo
**Resposta**: Relaciona questão com valores bruto e processado

### Database Schema

Tabelas em snake_case:
- `pesquisas` (id, nome, data_criacao)
- `grupos_questoes` (id, nome, categoria, cor, pesquisa_id)
- `questoes` (id, texto, e_invertida, grupo_questao_id)
- `resultados_pessoas` (id, nome_pessoa, email, data_avaliacao, pesquisa_id)
- `respostas` (questao_id, valor_bruto, valor_processado, resultado_pessoa_id)

### API Endpoints

Base URL: `https://localhost:7001/api`

- `GET /pesquisas` - Listar todas as pesquisas
- `GET /pesquisas/{id}` - Obter pesquisa por ID (com navegação completa)
- `POST /pesquisas` - Criar nova pesquisa
- `PUT /pesquisas/{id}` - Atualizar pesquisa
- `DELETE /pesquisas/{id}` - Excluir pesquisa
- `POST /pesquisas/{id}/resultados` - Salvar resultado de pessoa
- `GET /pesquisas/{id}/resultados` - Obter resultados da pesquisa
- `GET /pesquisas/{id}/resultados/{pessoaId}` - Obter resultado específico

Swagger UI disponível em: `https://localhost:7001/`

### Core Business Logic

**Sistema de Pontuação**:
- Cada questão em um grupo tem peso igual (1/quantidade)
- Questões invertidas: valorFinal = 10 - valorBruto
- Percentual do grupo: (soma dos valores processados * peso * 10)%

**Fluxo de Dados**:
1. Criar pesquisas com questões agrupadas
2. Aplicar pesquisas com escala 0-10
3. Calcular percentuais por grupo usando peso ponderado
4. Gerar radar charts lado a lado (Atual vs Ideal 100%)

### Development Notes

- **Naming Convention**: Todos os nomes em português brasileiro (classes, métodos, variáveis, tabelas, colunas)
- **Repository Pattern**: Abstração completa de acesso a dados
- **Dependency Injection**: Configurado no Program.cs da API
- **CORS**: Habilitado para comunicação API ↔ Blazor
- **Entity Framework**: Code-First com configuração Fluent API
- **Logging**: Configurado para desenvolvimento e produção
- **Error Handling**: Try-catch com logs estruturados nos controllers

### Migration Strategy

Para migrar dados existentes do LocalStorage:
1. Implementar endpoint de importação na API
2. Adaptar serviços do Blazor para HTTP calls
3. Manter ISurveyService como abstração
4. Criar serviço de migração automática

### Known Issues

1. **Connection String**: Ajustar no appsettings.json conforme ambiente
2. **CORS Origins**: Atualizar URLs conforme deploy
3. **Charts**: Verificar compatibilidade após migração dos serviços
4. **Theme Persistence**: Migrar de localStorage para API se necessário

### Testing

- API testável via Swagger UI
- Postman collection recomendada para testes automatizados
- Unit tests pendentes para repositórios e controllers