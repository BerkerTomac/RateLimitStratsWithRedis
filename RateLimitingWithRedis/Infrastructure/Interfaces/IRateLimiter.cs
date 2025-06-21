using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;


namespace Infrastructure.Interfaces
{
    public interface IRateLimiter
    {
        Task<RateLimitResult> IsAllowedAsync(string key, RateLimitConfig config);
        string StrategyName { get; }
    }
}
