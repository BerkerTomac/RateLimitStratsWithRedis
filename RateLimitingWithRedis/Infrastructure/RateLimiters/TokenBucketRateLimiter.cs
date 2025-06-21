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
    public class TokenBucketRateLimiter : IRateLimiter
    {
        private readonly IDatabase _database;
        public string StrategyName => "Token Bucket";

        public TokenBucketRateLimiter(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<RateLimitResult> IsAllowedAsync(string key, RateLimitConfig config)
        {
            var redisKey = $"token_bucket:{key}";
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Get current bucket state
            var bucketData = await _database.HashGetAllAsync(redisKey);
            var bucketDict = bucketData.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());

            var tokens = config.Capacity;
            var lastRefill = now;

            if (bucketDict.ContainsKey("tokens"))
            {
                tokens = int.Parse(bucketDict["tokens"]);
                lastRefill = long.Parse(bucketDict["last_refill"]);
            }

            // Calculate tokens to add
            var timePassed = now - lastRefill;
            var intervalsElapsed = timePassed / (long)config.RefillInterval.TotalSeconds;
            var tokensToAdd = (int)(intervalsElapsed * config.RefillRate);

            tokens = Math.Min(config.Capacity, tokens + tokensToAdd);

            if (tokens > 0)
            {
                tokens--;

                // Update bucket
                await _database.HashSetAsync(redisKey, new HashEntry[]
                {
                new("tokens", tokens),
                new("last_refill", now)
                });
                await _database.KeyExpireAsync(redisKey, TimeSpan.FromHours(1));

                return new RateLimitResult
                {
                    IsAllowed = true,
                    RemainingRequests = tokens,
                    ResetTime = DateTime.UtcNow.Add(config.RefillInterval),
                    Message = "Request allowed",
                    Strategy = StrategyName
                };
            }

            // Update last refill time even if no tokens available
            await _database.HashSetAsync(redisKey, "last_refill", now);

            return new RateLimitResult
            {
                IsAllowed = false,
                RemainingRequests = 0,
                ResetTime = DateTime.UtcNow.Add(config.RefillInterval),
                Message = "No tokens available",
                Strategy = StrategyName
            };
        }
    }
}
