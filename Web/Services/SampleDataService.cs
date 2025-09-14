using PesquisaX.Shared.Comandos;

namespace PesquisaX.Web.Services;

public static class SampleDataService
{
    public static PesquisaCrud CreateTennisEvaluationPesquisa()
    {
        var pesquisa = new PesquisaCrud
        {
            Nome = "Avaliação Completa de Tênis",
            DataCriacao = DateTime.UtcNow
        };

        // Grupo 1: Mental do Tênis
        var mentalGroup = new GrupoQuestaoCrud
        {
            Nome = "Mental do Tênis",
            Categoria = "MENTAL",
            Cor = "#FF6B6B"
        };
        
        mentalGroup.Questoes.AddRange(new[]
        {
            new QuestaoCrud { Texto = "Controle da ansiedade na hora do 'valendo'", EInvertida = false },
            new QuestaoCrud { Texto = "Controle da cabeça para não ter erros não forçados", EInvertida = false },
            new QuestaoCrud { Texto = "Mantém pensamentos positivos durante os jogos", EInvertida = false },
            new QuestaoCrud { Texto = "Consegue se concentrar mesmo com distrações", EInvertida = false }
        });

        // Grupo 2: Conhecimento
        var knowledgeGroup = new GrupoQuestaoCrud
        {
            Nome = "Conhecimento do Jogo",
            Categoria = "CONHECIMENTO",
            Cor = "#4ECDC4"
        };
        
        knowledgeGroup.Questoes.AddRange(new[]
        {
            new QuestaoCrud { Texto = "Conhece as regras básicas do tênis", EInvertida = false },
            new QuestaoCrud { Texto = "Entende estratégias de jogo", EInvertida = false },
            new QuestaoCrud { Texto = "Sabe quando fazer cada tipo de golpe", EInvertida = false }
        });

        // Grupo 3: Técnica
        var technicalGroup = new GrupoQuestaoCrud
        {
            Nome = "Aspectos Técnicos",
            Categoria = "TÉCNICA",
            Cor = "#45B7D1"
        };
        
        technicalGroup.Questoes.AddRange(new[]
        {
            new QuestaoCrud { Texto = "Qualidade do forehand", EInvertida = false },
            new QuestaoCrud { Texto = "Qualidade do backhand", EInvertida = false },
            new QuestaoCrud { Texto = "Efetividade do saque", EInvertida = false },
            new QuestaoCrud { Texto = "Movimentação em quadra", EInvertida = false },
            new QuestaoCrud { Texto = "Número de erros não forçados", EInvertida = true } // Invertida: menos erros = melhor
        });

        // Grupo 4: Competição
        var competitionGroup = new GrupoQuestaoCrud
        {
            Nome = "Competição",
            Categoria = "COMPETIÇÃO",
            Cor = "#96CEB4"
        };
        
        competitionGroup.Questoes.AddRange(new[]
        {
            new QuestaoCrud { Texto = "Performance em jogos oficiais", EInvertida = false },
            new QuestaoCrud { Texto = "Capacidade de lidar com a pressão", EInvertida = false },
            new QuestaoCrud { Texto = "Adaptação ao estilo do adversário", EInvertida = false }
        });

        // Grupo 5: Controle de Bola
        var ballControlGroup = new GrupoQuestaoCrud
        {
            Nome = "Controle de Bola",
            Categoria = "CONTROLE DE BOLA",
            Cor = "#FECA57"
        };
        
        ballControlGroup.Questoes.AddRange(new[]
        {
            new QuestaoCrud { Texto = "Precisão nos golpes direcionados", EInvertida = false },
            new QuestaoCrud { Texto = "Controle de potência", EInvertida = false },
            new QuestaoCrud { Texto = "Variedade de efeitos na bola", EInvertida = false }
        });

        // Grupo 6: Prática
        var practiceGroup = new GrupoQuestaoCrud
        {
            Nome = "Dedicação à Prática",
            Categoria = "PRÁTICA",
            Cor = "#FF9FF3"
        };
        
        practiceGroup.Questoes.AddRange(new[]
        {
            new QuestaoCrud { Texto = "Frequência de treinos", EInvertida = false },
            new QuestaoCrud { Texto = "Qualidade do foco durante treinos", EInvertida = false },
            new QuestaoCrud { Texto = "Busca por aperfeiçoamento técnico", EInvertida = false },
            new QuestaoCrud { Texto = "Faltas aos treinos", EInvertida = true } // Invertida: menos faltas = melhor
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