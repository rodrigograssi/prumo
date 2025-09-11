using ControleDeIp.FrontEnd.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PesquisaX.Web;
using PesquisaX.Web.Servicos;
using PesquisaX.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registrar o handler de autenticação
builder.Services.AddTransient<AuthenticationHandler>();

// Configurar HttpClient para API com handler de autenticação automática
builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/";
    
    // Log para debug (remover depois)
    Console.WriteLine($"[DEBUG] Configurando HttpClient com BaseAddress: {apiBaseUrl}");
    Console.WriteLine($"[DEBUG] Ambiente: {builder.HostEnvironment.Environment}");
    
    var authHandler = sp.GetRequiredService<AuthenticationHandler>();
    authHandler.InnerHandler = new HttpClientHandler();
    
    var httpClient = new HttpClient(authHandler);
    httpClient.BaseAddress = new Uri(apiBaseUrl);
    
    return httpClient;
});

builder.Services.AddMudServices();

// Configurar autorização
builder.Services.AddAuthorizationCore();

// Usar serviço HTTP para consumir API
// builder.Services.AddScoped<IServicoPesquisa, HttpPesquisaService>();
builder.Services.AddScoped<IServicoTema, ServicoTema>();
// builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services.AddScoped<IUsuarioServico, UsuarioServico>();

builder.Services.AddTransient<ApiController>();
builder.Services.AddTransient(typeof(ApiController<>));
builder.Services.AddTransient(typeof(ApiUsuarioController));
builder.Services.AddTransient<AutenticacaoController>();
builder.Services.AddTransient<AuthenticationStateProvider>(provider => provider.GetRequiredService<AutenticacaoController>());


var app = builder.Build();

await app.RunAsync();
