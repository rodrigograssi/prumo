namespace PesquisaX.Core.ValueObjects;

public class Sistema
{
    public DateTime DataUltimoAcesso { get; private set; }
    public string Token { get; private set; } =  string.Empty;
    
    protected Sistema(){}
    public Sistema(DateTime dataUltimoAcesso, string token)
    {
        DataUltimoAcesso = dataUltimoAcesso;
        Token = token;
    }
}