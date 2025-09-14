using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using PesquisaX.Infra.Contexto;

namespace PesquisaX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly PesquisaXContexto _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(PesquisaXContexto context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Health check básico da API
    /// </summary>
    [HttpGet("status")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        });
    }

    /// <summary>
    /// Testa a conexão com o banco de dados PostgreSQL
    /// </summary>
    [HttpGet("database")]
    public async Task<IActionResult> DatabaseHealth()
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Tenta conectar e executar uma query simples
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                _logger.LogWarning("Não foi possível conectar ao banco de dados");
                return StatusCode(503, new
                {
                    status = "unhealthy",
                    error = "Cannot connect to database",
                    timestamp = DateTime.UtcNow,
                    responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
                });
            }

            // Testa uma query simples
            var result = await _context.Database.ExecuteSqlRawAsync("SELECT 1");
            
            // Informações do banco
            var connectionString = _context.Database.GetConnectionString();
            var databaseName = _context.Database.GetDbConnection().Database;
            
            stopwatch.Stop();
            
            _logger.LogInformation("Database health check successful in {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
            
            return Ok(new
            {
                status = "healthy",
                database = new
                {
                    name = databaseName,
                    provider = "PostgreSQL",
                    canConnect = true,
                    connectionString = MaskConnectionString(connectionString),
                    responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
                },
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Database health check failed after {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
            
            return StatusCode(503, new
            {
                status = "unhealthy",
                error = ex.Message,
                type = ex.GetType().Name,
                responseTime = $"{stopwatch.ElapsedMilliseconds}ms",
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Health check detalhado da aplicação
    /// </summary>
    [HttpGet("detailed")]
    public async Task<IActionResult> DetailedHealth()
    {
        var checks = new List<object>();

        // Check da API
        checks.Add(new
        {
            name = "api",
            status = "healthy",
            description = "API is running"
        });

        // Check do banco de dados
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            checks.Add(new
            {
                name = "database",
                status = canConnect ? "healthy" : "unhealthy",
                description = canConnect ? "Database connection successful" : "Database connection failed"
            });
        }
        catch (Exception ex)
        {
            checks.Add(new
            {
                name = "database",
                status = "unhealthy",
                description = $"Database error: {ex.Message}"
            });
        }

        // Check de memória
        var process = Process.GetCurrentProcess();
        var memoryUsage = process.WorkingSet64 / (1024 * 1024); // MB
        
        checks.Add(new
        {
            name = "memory",
            status = memoryUsage < 500 ? "healthy" : "warning", // Warning se > 500MB
            description = $"Memory usage: {memoryUsage}MB",
            details = new { memoryUsageMB = memoryUsage }
        });

        var overallStatus = checks.Any(c => c.GetType().GetProperty("status")?.GetValue(c)?.ToString() == "unhealthy") 
            ? "unhealthy" : "healthy";

        return Ok(new
        {
            status = overallStatus,
            checks = checks,
            timestamp = DateTime.UtcNow,
            uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime,
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Mascara informações sensíveis da connection string
    /// </summary>
    private static string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "Not available";
            
        // Remove password da connection string para não vazar em logs
        var parts = connectionString.Split(';');
        var maskedParts = parts.Select(part =>
        {
            if (part.ToLower().Contains("password"))
                return "Password=***";
            return part;
        });
        
        return string.Join(";", maskedParts);
    }
}