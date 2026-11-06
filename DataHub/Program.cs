using DataHub;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHub<AlertsHub>("/alerts");
app.MapHealthChecks("/health");
app.UseHttpsRedirection();

app.Run();

