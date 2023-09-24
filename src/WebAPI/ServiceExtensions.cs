using Application.Common.Interfaces.Infrastructure.DI;
using Application.Common.Models;
using Infrastructure.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Reflection;
using WebAPI.ExceptionHandlers;
using WebAPI.Filters;

namespace WebAPI
{
    internal static class ServiceExtensions
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            //builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            return services.RegisterServices(configuration)
                    .AddCORS()
                    .AddControllers()
                    .AddSwagger(configuration)
                    .AddHttpClients();
        }

        public static void EnsureDatabaseSetup(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }

        static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IExceptionHandler, ValidationExceptionHandler>();
            services.AddScoped<IExceptionHandler, InvalidModelStateExceptionHandler>();
            services.AddScoped<IExceptionHandler, AppExceptionHandler>();

            services.RegisterScrutorServices();
            return services;
        }

        /// <summary>
        /// Register services automatically with Scrutor
        /// </summary>
        static IServiceCollection RegisterScrutorServices(this IServiceCollection services, Assembly assembly = null)
        {
            var executingAssembly = assembly ?? Assembly.GetExecutingAssembly();

            services.Scan(scan => scan
                .FromAssemblyDependencies(executingAssembly)
                .AddClasses(classes => classes.AssignableTo<ITransientDependency>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            services.Scan(scan => scan
                .FromAssemblyDependencies(executingAssembly)
                .AddClasses(classes => classes.AssignableTo<ISingletonDependency>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

            services.Scan(scan => scan
                  .FromAssemblyDependencies(executingAssembly)
                  .AddClasses(classes => classes.AssignableTo<IScopedDependency>())
                  .AsImplementedInterfaces()
                  .WithScopedLifetime());
            return services;
        }

        static IServiceCollection AddControllers(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            }).AddControllers(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            return services;
        }

        static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient();
            return services;
        }

        static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services
            .AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
            })
            .AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            })
            .AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "API v1", Version = "v1" });
                options.UseInlineDefinitionsForEnums();
                options.DescribeAllParametersInCamelCase();

                var assembly = Assembly.GetExecutingAssembly();
                var assemblyPath = Path.GetDirectoryName(assembly.Location);
                try
                {
                    foreach (var filePath in Directory.GetFiles(assemblyPath, $"{assembly.GetName().Name}.xml"))
                        options.IncludeXmlComments(filePath, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            })
            .AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        static IServiceCollection AddCORS(this IServiceCollection services)
        {
            return services.AddCors(opt =>
            {
                opt.AddPolicy(name: "AppConstants_CORS_PolicyName", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

    }
}
