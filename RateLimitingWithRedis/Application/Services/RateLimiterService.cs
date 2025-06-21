using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Models.Enums;
using Application.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services
{
    public class RateLimiterService : IRateLimiterService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<RateLimitStrategy, RateLimitConfig> _defaultConfigs;
        public RateLimiterService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _defaultConfigs = new Dictionary<RateLimitStrategy, RateLimitConfig>
            {
                [RateLimitStrategy.FixedWindow] = new RateLimitConfig
                {
                    Limit = 5,
                    Window = TimeSpan.FromMinutes(1)
                },
                [RateLimitStrategy.SlidingWindow] = new RateLimitConfig
                {
                    Limit = 5,
                    Window = TimeSpan.FromMinutes(1)
                },
                [RateLimitStrategy.TokenBucket] = new RateLimitConfig
                {
                    Capacity = 10,
                    RefillRate = 2,
                    RefillInterval = TimeSpan.FromSeconds(30)
                }
            };
        }

        public async Task<RateLimitResult> CheckRateLimitAsync(string clientId, RateLimitStrategy strategy)
        {
            var config = _defaultConfigs[strategy];
            return await CheckRateLimitAsync(clientId, strategy, config);
        }

        public async Task<RateLimitResult> CheckRateLimitAsync(string clientId, RateLimitStrategy strategy, RateLimitConfig config)
        {
            var rateLimiter = GetRateLimiter(strategy);
            return await rateLimiter.IsAllowedAsync(clientId, config);
        }

        private IRateLimiter GetRateLimiter(RateLimitStrategy strategy)
        {
            return strategy switch
            {
                RateLimitStrategy.FixedWindow => _serviceProvider.GetRequiredService<Infrastructure.RateLimiters.FixedWindowRateLimiter>(),
                RateLimitStrategy.SlidingWindow => _serviceProvider.GetRequiredService<Infrastructure.RateLimiters.SlidingWindowRateLimiter>(),
                RateLimitStrategy.TokenBucket => _serviceProvider.GetRequiredService<Infrastructure.RateLimiters.TokenBucketRateLimiter>(),
                _ => throw new ArgumentException($"Unknown rate limit strategy: {strategy}")
            };
        }
    }
}
