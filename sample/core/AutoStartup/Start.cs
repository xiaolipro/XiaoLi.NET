
using XiaoLi.NET.Startup;

namespace AutoStartup;

public class Start:IAutoStart
{
    public void ConfigureServices(IServiceCollection services)
    {
        
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
    }
}