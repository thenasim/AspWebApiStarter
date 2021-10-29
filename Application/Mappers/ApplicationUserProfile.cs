using Application.Features.Users;
using AutoMapper;
using Data.Entities;

namespace Application.Mappers
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<CreateUserCommand, ApplicationUser>()
                .ForMember(
                    d => d.UserName,
                    s => s.MapFrom(sc => sc.Email));
        }
    }
}