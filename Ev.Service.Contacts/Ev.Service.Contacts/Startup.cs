using Ev.Service.Contacts.Dto;
using Ev.Service.Contacts.Logs;
using Ev.Service.Contacts.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Ev.Service.Contacts
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddApiExplorer();
            var auditLog = new AuditLog();
            services.AddSingleton<IAuditLog>(auditLog);
            services.AddSingleton<IInfoLog, InfoLog>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contact Service", Version = "v1" });
            });
            services.AddSingleton<IConfiguration>(this.Configuration);
            string contactDbConnectionString = this.Configuration.GetValue<string>("ContactAdmin:ContactDB:ConnectionString");
            if (string.IsNullOrWhiteSpace(contactDbConnectionString))
            {
                throw new System.Configuration.ConfigurationErrorsException($"{nameof(contactDbConnectionString)}, this parameter is null");
            }
            services.AddDbContext<ContactDBContext>(opts => opts.UseSqlServer(contactDbConnectionString));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var response = new ApiResponseDto
                    {
                        Code = ApiResponseCode.InvalidData,
                    };
                    var error = System.Text.Json.JsonSerializer.Serialize(context.ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)));
                    var errorMessage = $"Invalid model received. HTTP method:{context.HttpContext.Request.Method} Error: {error}";
                    response.Errors.Add(new ErrorDto { Error = errorMessage, Code = ApiResponseCode.InvalidData });
                    var result = new BadRequestObjectResult(response);

                    result.ContentTypes.Add(MediaTypeNames.Application.Json);

                    auditLog.GetInstance().Error(errorMessage);
                    return result;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSerilogRequestLogging();
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                logger.LogError(context.Features.Get<IExceptionHandlerPathFeature>().Error, string.Empty);
                var result = JsonConvert.SerializeObject(new { error = "Server error" });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result).ConfigureAwait(false);
            }));
            //app.UseCors("AllowOrigin");

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(); 
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contact API V1");
            });
        }
    }
}
