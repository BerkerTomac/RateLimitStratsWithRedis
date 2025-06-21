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
    public class SlidingWindowRateLimiter : IRateLimiter
    {
        private readonly IDatabase _database;
        public string StrategyName => "Sliding Window";

        public SlidingWindowRateLimiter(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<RateLimitResult> IsAllowedAsync(string key, RateLimitConfig config)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var windowStart = now - (long)config.Window.TotalMilliseconds;
            var redisKey = $"sliding_window:{key}";

            // Remove old entries
            await _database.SortedSetRemoveRangeByScoreAsync(redisKey, 0, windowStart);

            // Count current requests in window
            var current = await _database.SortedSetLengthAsync(redisKey);

            if (current < config.Limit)
            {
                // Add current request
                await _database.SortedSetAddAsync(redisKey, Guid.NewGuid().ToString(), now);
                await _database.KeyExpireAsync(redisKey, config.Window);

                return new RateLimitResult
                {
                    IsAllowed = true,
                    RemainingRequests = config.Limit - (int)current - 1,
                    ResetTime = DateTime.UtcNow.Add(config.Window),
                    Message = "Request allowed",
                    Strategy = StrategyName
                };
            }

            return new RateLimitResult
            {
                IsAllowed = false,
                RemainingRequests = 0,
                ResetTime = DateTime.UtcNow.Add(config.Window),
                Message = "Rate limit exceeded",
                Strategy = StrategyName
            };
        }
    }
}
