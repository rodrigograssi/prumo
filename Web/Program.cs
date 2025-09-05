using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PesquisaX.Web;
using PesquisaX.Web.Servicos;
using PesquisaX.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient para API
builder.Services.AddScoped(sp => 
{
    var httpClient = new HttpClient();
    // URL da API - porta configurada no launchSettings.json da API
    httpClient.BaseAddress = new Uri("https://localhost:7088/");
    return httpClient;
});

builder.Services.AddMudServices();

// Usar serviço HTTP para consumir API
builder.Services.AddScoped<IServicoPesquisa, HttpPesquisaService>();
builder.Services.AddScoped<IServicoTema, ServicoTema>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

await builder.Build().RunAsync();
