using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PesquisaX.Core.Repositorios;
using System.Text;
using System.Text.Json;
using PesquisaX.Core.Interfaces;
using PesquisaX.Infra.Contexto;
using PesquisaX.Infra.Handlers;
using PesquisaX.Infra.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Função para ler Docker Secrets ou variáveis de ambiente
string GetSecret(string secretName, string? defaultValue = null)
{
    var secretPath = $"/run/secrets/{secretName}";
    if (File.Exists(secretPath))
    {
        return File.ReadAllText(secretPath).Trim();
    }
    
    var envValue = Environment.GetEnvironmentVariable(secretName.ToUpper());
    if (!string.IsNullOrEmpty(envValue))
    {
        return envValue;
    }
    
    return defaultValue ?? throw new InvalidOperationException($"Secret '{secretName}' não encontrado");
}

// Configuração do banco de dados com suporte a Docker Secrets
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Host=localhost;Database=pesquisax;Username=pesquisax_user;Password=pesquisax_2024!;Port=5432;Timezone=UTC";

Console.WriteLine($"[DEBUG] ConnectionString inicial: {connectionString}");
Console.WriteLine($"[DEBUG] Ambiente: {builder.Environment.EnvironmentName}");

// Substituir placeholder da senha se estiver em produção
if (builder.Environment.IsProduction())
{
    Console.WriteLine("[DEBUG] Entrando no modo Production");
    try
    {
        var dbPassword = GetSecret("postgres_password");
        Console.WriteLine($"[DEBUG] Secret postgres_password encontrado: {!string.IsNullOrEmpty(dbPassword)}");
        
        connectionString = connectionString.Replace("Password_File=/run/secrets/postgres_password", $"Password={dbPassword}");
        
        // Se não tem placeholder, construir connection string completa
        if (!connectionString.Contains("Password="))
        {
            Console.WriteLine("[DEBUG] Connection string não contém Password=, reconstruindo...");
            connectionString = $"Host=database;Database=pesquisax;Username=pesquisax;Password={dbPassword};Port=5432;Include Error Detail=false";
        }
        Console.WriteLine($"[DEBUG] ConnectionString final: {connectionString}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DEBUG] Erro ao ler secret: {ex.Message}");
        builder.Services.AddLogging().BuildServiceProvider().GetService<ILogger<Program>>()?.LogError(ex, "Erro ao ler secret do PostgreSQL");
    }
}

builder.Services.AddDbContext<PesquisaXContexto>(options =>
    options.UseNpgsql(connectionString));

// Configuração dos repositórios
builder.Services.AddScoped<IPesquisaRepositorio, PesquisaRepositorio>();
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();
builder.Services.AddScoped<IPesquisaHandler, PesquisaHandler>();

// Configuração da API
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configuração do JWT com suporte a Docker Secrets
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "PesquisaXSecretKeyPadrao123456789!@#$%^&*()";

// Usar Docker Secret para JWT se estiver em produção
if (builder.Environment.IsProduction())
{
    try
    {
        secretKey = GetSecret("jwt_secret");
    }
    catch (Exception ex)
    {
        builder.Services.AddLogging().BuildServiceProvider().GetService<ILogger<Program>>()?.LogError(ex, "Erro ao ler JWT secret, usando configuração padrão");
    }
}

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PesquisaX API",
        Version = "v1",
        Description = "API para gerenciamento de pesquisas e avaliações com visualização em radar charts"
    });
    
    // Configuração do JWT no Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    
    // Incluir comentários XML se existirem
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        // policy.WithOrigins(
        //         // Desenvolvimento local
        //         "https://localhost:7009", "http://localhost:5209", "https://localhost:5001", "http://localhost:5000", 
        //            //Swagger
        //           "https://localhost:7088",  "http://localhost:5173",
        //
        //           // Produção
        //           "https://pesquisax.thaishiroki.com.br", "https://www.pesquisax.thaishiroki.com.br"
        //       )
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
              //.AllowCredentials();
    });
    
});

// Configuração de logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Pipeline de middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Habilitar Swagger também em Production para testes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PesquisaX API V1");
    c.RoutePrefix = "swagger"; // Swagger disponível em /swagger
});

app.UseHttpsRedirection();
app.UseCors("AllowBlazorApp");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Health Check endpoint para Docker Swarm
app.MapGet("/health", async (PesquisaXContexto context) =>
{
    try
    {
        // Verificar conexão com banco de dados
        await context.Database.CanConnectAsync();
        
        return Results.Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Database = "Connected"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Health Check Failed",
            detail: ex.Message,
            statusCode: 503
        );
    }
})
.WithName("HealthCheck")
.WithOpenApi()
.AllowAnonymous();

app.MapControllers();

// Aplicar migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PesquisaXContexto>();
    try
    {
        context.Database.Migrate();
        app.Logger.LogInformation("Migrations aplicadas com sucesso");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Erro ao aplicar migrations: {Message}", ex.Message);
    }
}


app.Logger.LogInformation("PesquisaX API iniciada em {Environment}", app.Environment.EnvironmentName);

app.Run();