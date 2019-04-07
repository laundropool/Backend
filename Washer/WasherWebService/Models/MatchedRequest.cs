using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WasherWebService.Models
{
    public class MatchedRequest
    {
        public int MatchedRequestId { get; set; }
        public string OwnerId { get; set; }
        public string WasherId { get; set; }
        public int OwnerRequestId { get; set; }
        public int WasherRequestId { get; set; }
        public string Status { get; set; }
        public decimal Distance { get; set; }
        public string RequestSentBy { get; set; }

    }
}
