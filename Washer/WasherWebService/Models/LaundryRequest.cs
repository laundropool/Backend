using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WasherWebService.Models
{
    public class LaundryRequest
    {
        public int RequestId { get; set; }
        public string UserId { get; set; }
        public DateTime WashingTime { get; set; }
        public bool WhitesOnly { get; set; }
        public bool DenimsOrTrousersOnly { get; set; }
        public bool GarmentsOnly { get; set; }
        public bool UnderGarmentsOnly { get; set; }
        public int Weight { get; set; }
        public bool WashingMachine { get; set; }
        public string Status { get; set; }
    }
}
