using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WasherWebService.Models
{
    public class AcceptedRequest
    {
        public int AcceptedRequestId { get; set; }
        public string OwnerId { get; set; }
        public string WasherId { get; set; }
        public int? OwnerRequestId { get; set; }
        public int? WasherRequestId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; }
    }
}
