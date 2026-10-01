using OpenAI.Chat;
using Prumo.Shared.Comandos;
using System.Text.Json;

namespace Prumo.Api.Services;

public class OpenAIService : IAIService
{
    private readonly ChatClient? _chatClient;
    private readonly ILogger<OpenAIService> _logger;
    private readonly bool _isAvailable;

    public OpenAIService(IConfiguration configuration, ILogger<OpenAIService> logger)
    {
        _logger = logger;

        var apiKey = configuration["OpenAI:ApiKey"];

        if (!string.IsNullOrEmpty(apiKey))
        {
            try
            {
                _chatClient = new ChatClient("gpt-3.5-turbo", apiKey);
                _isAvailable = true;
                _logger.LogInformation("OpenAI service inicializado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao inicializar OpenAI service");
                _isAvailable = false;
            }
        }
        else
        {
            _logger.LogInformation("OpenAI API Key não configurada - serviço de IA não disponível");
            _isAvailable = false;
        }
    }

    public Task<bool> EstaDisponivel()
    {
        return Task.FromResult(_isAvailable && _chatClient != null);
    }

    public async Task<string> GerarPerguntas(GerarPerguntasCommand comando)
    {
        if (!await EstaDisponivel())
            throw new InvalidOperationException("Serviço de IA não está disponível");

        var prompt = $@"
Você é um especialista em criação de pesquisas e questionários.
Crie {comando.QuantidadePerguntas} perguntas para a categoria '{comando.Categoria}' sobre o tema '{comando.TemaPesquisa}'.

Regras:
- As perguntas devem ser claras e objetivas
- Devem ser respondidas em escala de 0 a 10
- Focadas em avaliar competências, comportamentos ou situações
- Evite perguntas muito similares
- Use linguagem profissional mas acessível

Retorne apenas as perguntas, uma por linha, numeradas.
";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("Você é um especialista em criação de pesquisas e questionários profissionais."),
            new UserChatMessage(prompt)
        };

        var response = await _chatClient!.CompleteChatAsync(messages);
        return response.Value.Content[0].Text;
    }

    public async Task<string> MelhorarPesquisa(MelhorarPesquisaCommand comando)
    {
        if (!await EstaDisponivel())
            throw new InvalidOperationException("Serviço de IA não está disponível");

        var pesquisaJson = JsonSerializer.Serialize(comando.PesquisaAtual, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var prompt = $@"
Analise a seguinte pesquisa e forneça sugestões de melhorias:

{pesquisaJson}

Por favor, analise e sugira melhorias em:
1. Clareza das perguntas
2. Equilíbrio entre categorias
3. Perguntas duplicadas ou muito similares
4. Sugestões de novas perguntas importantes
5. Melhoria na redação das perguntas existentes

Seja específico e prático nas suas sugestões.
";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("Você é um consultor especialista em design de pesquisas e questionários."),
            new UserChatMessage(prompt)
        };

        var response = await _chatClient!.CompleteChatAsync(messages);
        return response.Value.Content[0].Text;
    }

    public async Task<string> SugerirCategoriasEPerguntas(string temaPesquisa, int quantidadeGrupos = 5)
    {
        if (!await EstaDisponivel())
            throw new InvalidOperationException("Serviço de IA não está disponível");

        var prompt = $@"
Crie uma estrutura completa de pesquisa sobre '{temaPesquisa}' com {quantidadeGrupos} categorias.

Para cada categoria, forneça:
1. Nome da categoria
2. 4-5 perguntas específicas
3. Uma cor em formato hexadecimal (#RRGGBB)

Formato esperado:
**Categoria: [Nome]** (Cor: #RRGGBB)
1. [Pergunta 1]
2. [Pergunta 2]
3. [Pergunta 3]
4. [Pergunta 4]
5. [Pergunta 5]

As perguntas devem:
- Ser respondidas em escala de 0 a 10
- Avaliar aspectos diferentes do tema
- Ser claras e específicas
- Cobrir o tema de forma abrangente
";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("Você é um especialista em design de pesquisas organizacionais e questionários de avaliação."),
            new UserChatMessage(prompt)
        };

        var response = await _chatClient!.CompleteChatAsync(messages);
        return response.Value.Content[0].Text;
    }
}