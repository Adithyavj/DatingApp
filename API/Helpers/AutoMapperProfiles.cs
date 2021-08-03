using System.Linq;
using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // where to map from and where to map to
            // here appuser to membersdto
            CreateMap<AppUser, MemberDto>()
                    .ForMember(dest => dest.PhotoUrl,
                               opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<Photo, PhotoDto>();
        }
    }
}