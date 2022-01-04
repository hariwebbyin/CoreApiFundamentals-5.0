using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper; // it has Profile
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>()
                .ForMember(cmpmdl => cmpmdl.VenueName, optn => optn.MapFrom(cmpentity => cmpentity.Location.VenueName)).ReverseMap();
            //first part is gonna make the default mapping but we can add an exception using ForMember, here 
            //ForMember takes 2 args - 1: the model which we wanna map to, and the option (what we want to do with that model field.
            //MapFrom is returing use camp entity's location object's VenueName field here.

            this.CreateMap<Talk, TalkModel>().ReverseMap()
                .ForMember(cmp => cmp.Camp, opt =>opt.Ignore()); //since we're dong it after ReverseMap, it will be applicable only for the mapping
                                                                 //from TalkModel to Talk.

            this.CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}
