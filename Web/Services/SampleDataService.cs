using PesquisaX.Shared.Modelos;

namespace PesquisaX.Web.Servicos;

public static class SampleDataService
{
    public static Pesquisa CreateTennisEvaluationPesquisa()
    {
        var pesquisa = new Pesquisa
        {
            Nome = "Avaliação Completa de Tênis",
            DataCriacao = DateTime.UtcNow
        };

        // Grupo 1: Mental do Tênis
        var mentalGroup = new GrupoQuestao
        {
            Nome = "Mental do Tênis",
            Categoria = "MENTAL",
            Cor = "#FF6B6B"
        };
        
        mentalGroup.Questoes.AddRange(new[]
        {
            new Questao { Texto = "Controle da ansiedade na hora do 'valendo'", EInvertida = false },
            new Questao { Texto = "Controle da cabeça para não ter erros não forçados", EInvertida = false },
            new Questao { Texto = "Mantém pensamentos positivos durante os jogos", EInvertida = false },
            new Questao { Texto = "Consegue se concentrar mesmo com distrações", EInvertida = false }
        });

        // Grupo 2: Conhecimento
        var knowledgeGroup = new GrupoQuestao
        {
            Nome = "Conhecimento do Jogo",
            Categoria = "CONHECIMENTO",
            Cor = "#4ECDC4"
        };
        
        knowledgeGroup.Questoes.AddRange(new[]
        {
            new Questao { Texto = "Conhece as regras básicas do tênis", EInvertida = false },
            new Questao { Texto = "Entende estratégias de jogo", EInvertida = false },
            new Questao { Texto = "Sabe quando fazer cada tipo de golpe", EInvertida = false }
        });

        // Grupo 3: Técnica
        var technicalGroup = new GrupoQuestao
        {
            Nome = "Aspectos Técnicos",
            Categoria = "TÉCNICA",
            Cor = "#45B7D1"
        };
        
        technicalGroup.Questoes.AddRange(new[]
        {
            new Questao { Texto = "Qualidade do forehand", EInvertida = false },
            new Questao { Texto = "Qualidade do backhand", EInvertida = false },
            new Questao { Texto = "Efetividade do saque", EInvertida = false },
            new Questao { Texto = "Movimentação em quadra", EInvertida = false },
            new Questao { Texto = "Número de erros não forçados", EInvertida = true } // Invertida: menos erros = melhor
        });

        // Grupo 4: Competição
        var competitionGroup = new GrupoQuestao
        {
            Nome = "Competição",
            Categoria = "COMPETIÇÃO",
            Cor = "#96CEB4"
        };
        
        competitionGroup.Questoes.AddRange(new[]
        {
            new Questao { Texto = "Performance em jogos oficiais", EInvertida = false },
            new Questao { Texto = "Capacidade de lidar com a pressão", EInvertida = false },
            new Questao { Texto = "Adaptação ao estilo do adversário", EInvertida = false }
        });

        // Grupo 5: Controle de Bola
        var ballControlGroup = new GrupoQuestao
        {
            Nome = "Controle de Bola",
            Categoria = "CONTROLE DE BOLA",
            Cor = "#FECA57"
        };
        
        ballControlGroup.Questoes.AddRange(new[]
        {
            new Questao { Texto = "Precisão nos golpes direcionados", EInvertida = false },
            new Questao { Texto = "Controle de potência", EInvertida = false },
            new Questao { Texto = "Variedade de efeitos na bola", EInvertida = false }
        });

        // Grupo 6: Prática
        var practiceGroup = new GrupoQuestao
        {
            Nome = "Dedicação à Prática",
            Categoria = "PRÁTICA",
            Cor = "#FF9FF3"
        };
        
        practiceGroup.Questoes.AddRange(new[]
        {
            new Questao { Texto = "Frequência de treinos", EInvertida = false },
            new Questao { Texto = "Qualidade do foco durante treinos", EInvertida = false },
            new Questao { Texto = "Busca por aperfeiçoamento técnico", EInvertida = false },
            new Questao { Texto = "Faltas aos treinos", EInvertida = true } // Invertida: menos faltas = melhor
        });

        pesquisa.GruposQuestoes.AddRange(new[] 
        { 
            mentalGroup, 
            knowledgeGroup, 
            technicalGroup, 
            competitionGroup, 
            ballControlGroup, 
            practiceGroup 
        });

        return pesquisa;
    }
}