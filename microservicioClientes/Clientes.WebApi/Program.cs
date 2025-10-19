using Clientes.Application;
using Clientes.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("OracleDb") 
    ?? throw new InvalidOperationException("Connection string 'OracleDb' not found.");
var auditoriaServiceUrl = builder.Configuration["AuditoriaServiceUrl"] 
    ?? throw new InvalidOperationException("AuditoriaServiceUrl not found in configuration.");

builder.Services.AddScoped<IClienteRepository>(provider => 
    new OracleClienteRepository(connectionString));
builder.Services.AddScoped<ClienteService>();

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
