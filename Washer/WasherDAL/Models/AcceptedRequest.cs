using System;
using System.Collections.Generic;

namespace WasherDAL.Models
{
    public partial class AcceptedRequest
    {
        public int AcceptedRequestId { get; set; }
        public string OwnerId { get; set; }
        public string WasherId { get; set; }
        public int? OwnerRequestId { get; set; }
        public int? WasherRequestId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; }

        public MatchedRequest MatchedRequest { get; set; }
    }
}
