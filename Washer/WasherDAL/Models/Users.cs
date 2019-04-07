using System;
using System.Collections.Generic;

namespace WasherDAL.Models
{
    public partial class Users
    {
        public Users()
        {
            LaundryRequest = new HashSet<LaundryRequest>();
            MatchedRequestOwner = new HashSet<MatchedRequest>();
            MatchedRequestWasher = new HashSet<MatchedRequest>();
            Transaction = new HashSet<Transaction>();
        }

        public string Userid { get; set; }
        public string Username { get; set; }
        public string Useremail { get; set; }
        public string Usermobile { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public byte[] Userpassword { get; set; }
        public bool Washing { get; set; }

        public ICollection<LaundryRequest> LaundryRequest { get; set; }
        public ICollection<MatchedRequest> MatchedRequestOwner { get; set; }
        public ICollection<MatchedRequest> MatchedRequestWasher { get; set; }
        public ICollection<Transaction> Transaction { get; set; }
    }
}
