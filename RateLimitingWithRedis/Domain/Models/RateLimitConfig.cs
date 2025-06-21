using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RateLimitConfig
    {
        public int Limit { get; set; }
        public TimeSpan Window { get; set; }
        public int Capacity { get; set; } // For token bucket
        public int RefillRate { get; set; } // For token bucket
        public TimeSpan RefillInterval { get; set; } // For token bucket
    }
}
