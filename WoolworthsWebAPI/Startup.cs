using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Polly;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using WoolworthsWebAPI.Filters;
using WoolworthsWebAPI.Repositories;
using WoolworthsWebAPI.Services;
using WoolworthsWebAPI.Services.Interfaces;

namespace WoolworthsWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Adding custom filter for validations.
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            //Add toggle feature functionality.
            services.AddFeatureManagement();

            //registering fluentvalidation validators.
            services.AddControllers()
                    .AddFluentValidation(validator =>
                    {
                        validator.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                        validator.ImplicitlyValidateChildProperties = true;
                        validator.RegisterValidatorsFromAssemblyContaining<Startup>();
                    });


            //Adding Dependency Injections.
            services.AddScoped<IShoppingService, ShoppingService>();
            services.AddScoped<IServiceAPIRepository, ServiceAPIRepository>();
            services.AddHttpClient("WooliesX", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetValue<string>("ApplicationData:Resources:BaseUrl"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            }).SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(3, t => TimeSpan.FromSeconds(2)));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WoolworthsWebAPI", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Add Middleware to handle exceptions.
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //Add Swagger to Middleware.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WoolworthsWebAPI");
                c.RoutePrefix = "api";

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
