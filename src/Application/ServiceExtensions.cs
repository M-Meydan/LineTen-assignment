using Application.Common.Interfaces.Infrastructure.DI;
using Application.Configuration.FluentValidation;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Application
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(assembly)
                    .AddValidatorsFromAssembly(assembly)
                    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly))
                    .RegisterServices();

            ValidatorOptions.Global.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;

            return services;
        }

        /// <summary>
        /// Register Application services manually
        /// </summary>
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
