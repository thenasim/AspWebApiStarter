using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Models;
using Application.Interfaces;
using AutoMapper;
using Data.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users
{
    public class LoginUserQuery : IRequest<AuthModel>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ValidateLoginUserQuery : AbstractValidator<LoginUserQuery>
    {
        public ValidateLoginUserQuery()
        {
            RuleFor(x => x.Email)
                .EmailAddress();
            
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .MinimumLength(6)
                .MaximumLength(30);
        }
    }

    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, AuthModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;

        public LoginUserQueryHandler(
            UserManager<ApplicationUser> userManager, 
            IMapper mapper,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtService = jwtService;
        }
        
        public async Task<AuthModel> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            // Check if user exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is null)
            {
                return new AuthModel()
                {
                    Messages = new List<string>()
                    {
                        $"Invalid credentials"
                    },
                    IsAuthenticated = false,
                };
            }

            var checkPassword = await _userManager.CheckPasswordAsync(existingUser, request.Password);
            if (checkPassword == false)
            {
                return new AuthModel()
                {
                    Messages = new List<string>()
                    {
                        $"Invalid credentials"
                    },
                    IsAuthenticated = false,
                };
            }
            
            
            var jwtToken = await _jwtService.GenerateToken(existingUser);

            var roles = await _userManager.GetRolesAsync(existingUser).ConfigureAwait(false);

            return new AuthModel()
            {
                Messages = new List<string>()
                {
                    "Login successfully"
                },
                IsAuthenticated = true,
                Token = jwtToken,
                UserName = existingUser.UserName,
                Email = existingUser.Email,
                Roles = roles.ToList()
            };
        }
    }
}