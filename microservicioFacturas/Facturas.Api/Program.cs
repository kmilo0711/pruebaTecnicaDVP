using Facturas.Application;
using Facturas.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("OracleDb") 
    ?? throw new InvalidOperationException("Connection string 'OracleDb' not found.");
var clientesServiceUrl = builder.Configuration["ClientesServiceUrl"] 
    ?? throw new InvalidOperationException("ClientesServiceUrl not found in configuration.");
var auditoriaServiceUrl = builder.Configuration["AuditoriaServiceUrl"] 
    ?? throw new InvalidOperationException("AuditoriaServiceUrl not found in configuration.");

builder.Services.AddScoped<IFacturaRepository>(provider => 
    new OracleFacturaRepository(connectionString));
builder.Services.AddScoped<FacturaService>();

builder.Services.AddHttpClient<HttpClienteGateway>();
builder.Services.AddScoped<IClienteGateway>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    return new HttpClienteGateway(httpClient, clientesServiceUrl);
});

builder.Services.AddHttpClient<HttpAuditoriaClient>();
builder.Services.AddScoped<HttpAuditoriaClient>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    return new HttpAuditoriaClient(httpClient, auditoriaServiceUrl);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
