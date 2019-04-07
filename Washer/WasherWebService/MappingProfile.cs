using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WasherDAL.Models;
//using WasherDAL.Models;

namespace WasherWebService
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<MatchedRequest, Models.MatchedRequest>();
            CreateMap<Models.MatchedRequest, MatchedRequest>();
            CreateMap<LaundryRequest, Models.LaundryRequest>();
            CreateMap<Models.LaundryRequest, LaundryRequest>();
        }
    }
}
