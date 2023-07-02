using Aspose.Email.Clients;
using itedu_assitant.Controllers.Methods;
using itedu_assitant.DB;
using itedu_assitant.forsave.Methods;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Reflection;

namespace itedu_assitant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


            builder.Services.AddAuthentication();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<dbcontext>();
            

            // set values of logger
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ToDo API",
                    Description = "An ASP.NET Core Web API for managing ToDo items",
                    License = new OpenApiLicense
                    {
                        Name = "itedu.kz",
                        Url = new Uri("http://itedu.kz")
                    }});
               });

            //builder.Services.AddSingleton<IConnectionMultiplexer>(new rediscontext().connectionmpx);
            //builder.Services.AddHostedService<BackgroundTaskManage>();

            var app = builder.Build();


            // add logger

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Home}/{action=Meeting}/{id?}");

            app.UseRouting().UseMiddleware<Middleware>().UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                });


            app.MapControllers();
            app.Run();
        }
    }
}