using Application.Features.Users;
using AutoMapper;
using Data.Entities;
using Data.Enums;

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

            CreateMap<CreateUserCommandDto, CreateUserCommand>()
                .ForMember(
                    d => d.Role,
                    s => s.MapFrom(sc => Roles.User));
        }
    }
}