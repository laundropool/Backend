using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WasherWebService.Models
{
    public class User
    {
        public string Userid { get; set; }
        public string Username { get; set; }
        public string Useremail { get; set; }
        public string Usermobile { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Userpassword { get; set; }
        public bool Washing { get; set; }

    }
}
