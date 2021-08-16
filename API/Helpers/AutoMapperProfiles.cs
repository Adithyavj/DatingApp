using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
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
                               opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))  // to map PhotoUrl
                    .ForMember(dest => dest.Age,
                               opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge())); // to map Age
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>(); // for updating data
            CreateMap<RegisterDto, AppUser>();
        }
    }
}