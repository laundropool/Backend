using System;
using System.Collections.Generic;

namespace WasherDAL.Models
{
    public partial class LaundryRequest
    {
        public LaundryRequest()
        {
            MatchedRequestOwnerRequest = new HashSet<MatchedRequest>();
            MatchedRequestWasherRequest = new HashSet<MatchedRequest>();
            Transaction = new HashSet<Transaction>();
        }

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

        public Users User { get; set; }
        public ICollection<MatchedRequest> MatchedRequestOwnerRequest { get; set; }
        public ICollection<MatchedRequest> MatchedRequestWasherRequest { get; set; }
        public ICollection<Transaction> Transaction { get; set; }
    }
}
