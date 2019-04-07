using System;
using System.Collections.Generic;

namespace WasherDAL.Models
{
    public partial class MatchedRequest
    {
        public MatchedRequest()
        {
            AcceptedRequest = new HashSet<AcceptedRequest>();
        }

        public int MatchedRequestId { get; set; }
        public string OwnerId { get; set; }
        public string WasherId { get; set; }
        public int? OwnerRequestId { get; set; }
        public int? WasherRequestId { get; set; }
        public string Status { get; set; }
        public decimal Distance { get; set; }
        public string RequestSentBy { get; set; }

        public Users Owner { get; set; }
        public LaundryRequest OwnerRequest { get; set; }
        public Users Washer { get; set; }
        public LaundryRequest WasherRequest { get; set; }
        public ICollection<AcceptedRequest> AcceptedRequest { get; set; }
    }
}
