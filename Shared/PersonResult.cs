namespace PesquisaX.Shared.Modelos;

public class ResultadoPessoa
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NomePessoa { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;
    public List<Resposta> Respostas { get; set; } = new();
    
    public Dictionary<string, double> ObterPercentuaisGrupos(List<GrupoQuestao> grupos)
    {
        var result = new Dictionary<string, double>();
        
        foreach (var grupo in grupos)
        {
            var respostasGrupo = new List<int>();
            
            foreach (var questao in grupo.Questoes)
            {
                var resposta = Respostas.FirstOrDefault(r => r.QuestaoId == questao.Id);
                respostasGrupo.Add(resposta?.ValorBruto ?? 0);
            }
            
            result[grupo.Categoria] = grupo.CalcularPercentual(respostasGrupo);
        }
        
        return result;
    }
}