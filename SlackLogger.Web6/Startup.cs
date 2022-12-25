using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SlackLogger.Web._2
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {

            app.UseDeveloperExceptionPage();


            app.Run(async context =>
            {
                if (context.Request.Path.Value.Contains("favicon.ico"))
                    return;

                try
                {
                    throw new Exception("Innerexception");
                }
                catch (Exception e)
                {
                    throw new Exception("Outerexception", e);
                }

                var logger = context.RequestServices.GetService<ILogger<Startup>>();
                logger.LogWarning("Warning");
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
