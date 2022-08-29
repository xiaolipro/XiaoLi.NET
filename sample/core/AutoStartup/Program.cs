using XiaoLi.NET.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.InitWebApp();

var app = builder.Build();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();