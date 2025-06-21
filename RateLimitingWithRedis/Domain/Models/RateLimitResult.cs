using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RateLimitResult
    {
        public bool IsAllowed { get; set; }
        public int RemainingRequests { get; set; }
        public DateTime ResetTime { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Strategy { get; set; } = string.Empty;
    }
}
