using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WasherWebService.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public string UserId { get; set; }
        public int Laundrocash { get; set; }
        public string TransactionType { get; set; }
        public string Message { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public int? RequestId { get; set; }
    }
}
