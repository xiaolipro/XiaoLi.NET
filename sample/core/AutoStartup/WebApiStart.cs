using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using XiaoLi.NET.Startup;
using XiaoLi.NET.Startup.Attributes;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace AutoStartup;

[StartOrder(10)]
public class WebApiStart:IAutoStart
{
    public void ConfigureService(IServiceCollection services)
    {
        // Add services to the container.

        services.AddControllers()
            
            // .AddNewtonsoftJson(options =>
            // {
            //     options.SerializerSettings.Converters.Add(new DateTimeConverter2());
            // })
            .AddJsonOptions(config =>
            {
                //此设定解决JsonResult中文被编码的问题
                //config.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            
                config.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            })
            ;
    }

    public class DateTimeConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.TryParse(reader.GetString(), out var dateTime) ? dateTime : default(DateTime);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
    public class DateTimeConverter2 : Newtonsoft.Json.JsonConverter<DateTime>
    {
        public override void WriteJson(JsonWriter writer, DateTime value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString("yyy-MM-dd HH:mm"));
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue,
            Newtonsoft.Json.JsonSerializer serializer)
        {
            return DateTime.TryParse(reader.Value.ToString(), out DateTime dateTime) ? dateTime : default(DateTime);
        }
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