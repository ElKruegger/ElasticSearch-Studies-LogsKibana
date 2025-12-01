using Serilog;
using Serilog.Events;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configuração do Serilog usando a configuração do builder
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.WithProperty("ApplicationName", "ElasticStudiesLogsKibana")
        .CreateLogger();

    Log.Information("Iniciando o host da API Web...");

    // Usa Serilog para logging
    builder.Host.UseSerilog();

    // Adiciona serviços ao container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configura o pipeline de requisições HTTP.
    // IMPORTANTE: A ordem dos middlewares é crítica!

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Adiciona o middleware do Serilog para logs de requisição
    // Deve vir depois do Swagger para evitar logs duplicados de requisições do Swagger UI
    app.UseSerilogRequestLogging(options =>
    {
        // Personaliza o nível de log para requisições bem-sucedidas
        options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });

    // UseHttpsRedirection pode causar requisições duplicadas em desenvolvimento
    // Comentado para evitar problemas durante desenvolvimento
    // app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "O Host da API parou inesperadamente.");
}
finally
{
    Log.CloseAndFlush();
}
