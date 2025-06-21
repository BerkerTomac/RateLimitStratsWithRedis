using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Infrastructure.Interfaces;
using Domain.Models;

namespace Infrastructure.RateLimiters
{
    public class FixedWindowRateLimiter : IRateLimiter
    {
        private readonly IDatabase _database;
        public string StrategyName => "Fixed Window";

        public FixedWindowRateLimiter(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<RateLimitResult> IsAllowedAsync(string key, RateLimitConfig config)
        {
            var windowStart = DateTimeOffset.UtcNow.Ticks / config.Window.Ticks * config.Window.Ticks;
            var redisKey = $"fixed_window:{key}:{windowStart}";

            var current = await _database.StringIncrementAsync(redisKey);

            if (current == 1)
            {
                await _database.KeyExpireAsync(redisKey, config.Window);
            }

            var isAllowed = current <= config.Limit;
            var remaining = Math.Max(0, config.Limit - (int)current);
            var resetTime = new DateTime(windowStart).Add(config.Window);

            return new RateLimitResult
            {
                IsAllowed = isAllowed,
                RemainingRequests = remaining,
                ResetTime = resetTime,
                Message = isAllowed ? "Request allowed" : "Rate limit exceeded",
                Strategy = StrategyName
            };
        }
    }
}
