using DataHub;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapHub<AlertsHub>("/alerts");
app.UseHttpsRedirection();

app.Run();

 