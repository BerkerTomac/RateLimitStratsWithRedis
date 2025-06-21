using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.RateLimiters;
using Application.Services;
using Application.Interfaces;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;


namespace Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRateLimitingServices(this IServiceCollection services, string redisConnectionString)
        {
            // Redis connection
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });
            // Rate limiters
            services.AddScoped<FixedWindowRateLimiter>();
            services.AddScoped<SlidingWindowRateLimiter>();
            services.AddScoped<TokenBucketRateLimiter>();

            // Services
            services.AddScoped<IRateLimiterService, RateLimiterService>();

            return services;
        }
    }
}
