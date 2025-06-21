using Microsoft.AspNetCore.Mvc;
using Domain.Models.Enums;
using Application.Interfaces;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RateLimitController : ControllerBase
    {
        private readonly IRateLimiterService _rateLimiterService;
        public RateLimitController(IRateLimiterService rateLimiterService)
        {
            _rateLimiterService = rateLimiterService;
        }

        [HttpGet("fixed-window")]
        public async Task<IActionResult> TestFixedWindow()
        {
            var clientId = GetClientId();
            var result = await _rateLimiterService.CheckRateLimitAsync(clientId, RateLimitStrategy.FixedWindow);

            if (!result.IsAllowed)
            {
                return StatusCode(429, result);
            }

            return Ok(new { message = "Fixed window request successful", data = result });
        }

        [HttpGet("sliding-window")]
        public async Task<IActionResult> TestSlidingWindow()
        {
            var clientId = GetClientId();
            var result = await _rateLimiterService.CheckRateLimitAsync(clientId, RateLimitStrategy.SlidingWindow);

            if (!result.IsAllowed)
            {
                return StatusCode(429, result);
            }

            return Ok(new { message = "Sliding window request successful", data = result });
        }

        [HttpGet("token-bucket")]
        public async Task<IActionResult> TestTokenBucket()
        {
            var clientId = GetClientId();
            var result = await _rateLimiterService.CheckRateLimitAsync(clientId, RateLimitStrategy.TokenBucket);

            if (!result.IsAllowed)
            {
                return StatusCode(429, result);
            }

            return Ok(new { message = "Token bucket request successful", data = result });
        }

        private string GetClientId()
        {
            return Request.Headers["X-Client-Id"].FirstOrDefault() ?? "anonymous";
        }
    }
}
