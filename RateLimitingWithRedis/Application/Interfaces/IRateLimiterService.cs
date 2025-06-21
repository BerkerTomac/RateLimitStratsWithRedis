using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Models.Enums;

namespace Application.Interfaces
{
    public interface IRateLimiterService
    {
        Task<RateLimitResult> CheckRateLimitAsync(string clientId, RateLimitStrategy strategy);
        Task<RateLimitResult> CheckRateLimitAsync(string clientId, RateLimitStrategy strategy, RateLimitConfig config);
    }
}
