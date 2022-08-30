using XiaoLi.NET.Startup;
using XiaoLi.NET.Startup.Attributes;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace AutoStartup;

[StartOrder(10)]
public class WebApiStart:IAutoStart
{
    public void ConfigureService(IServiceCollection services)
    {
        // Add services to the container.

        services.AddControllers();


    }


    public void AddSwagger(IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }


    public void SB(IApplicationBuilder app, IHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}