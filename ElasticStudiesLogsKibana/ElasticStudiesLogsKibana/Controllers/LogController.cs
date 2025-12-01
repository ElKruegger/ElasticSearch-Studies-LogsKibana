using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElasticStudiesLogsKibana.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger)
        {
            _logger = logger;
        }

        [HttpGet("generate")]
        public IActionResult GenerateLogs()
        {
            _logger.LogInformation("Requisição de log recebida. Gerando logs de teste.");

            // Simulação de Logs Estruturados
            for (int i = 0; i < 3; i++)
            {
                _logger.LogInformation("Processando transação {TransactionId} para o usuário {UserId}", Guid.NewGuid(), 100 + i);

                if (i == 1)
                {
                    _logger.LogWarning("Transação {TransactionId} demorou mais que o esperado.", Guid.NewGuid());
                }
            }

            // Simulação de Log de Erro com Exceção
            try
            {
                throw new InvalidOperationException("Simulação de falha ao processar pagamento.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro crítico no processamento de pagamento.");
            }

            _logger.LogInformation("Logs de teste gerados com sucesso.");

            return Ok("Logs de teste gerados. Verifique o console e o Elasticsearch (se estiver rodando).");
        }
    }
}
