using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Config;
using Application.Features.Models;
using Application.Interfaces;
using AutoMapper;
using Data.Entities;
using Data.Enums;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users
{
    public class CreateUserCommand : IRequest<AuthModel>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }
    }

    public class ValidateCreateUserCommand : AbstractValidator<CreateUserCommand>
    {
        public ValidateCreateUserCommand()
        {
            RuleFor(x => x.Email)
                .EmailAddress();
            
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .MinimumLength(6)
                .MaximumLength(30);
        }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, AuthModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;

        public CreateUserCommandHandler(
            UserManager<ApplicationUser> userManager, 
            IMapper mapper,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtService = jwtService;
        }
        
        public async Task<AuthModel> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
            {
                return new AuthModel()
                {
                    Messages = new List<string>()
                    {
                        $"Email already exists"
                    },
                    IsAuthenticated = false,
                };
            }
            
            // Map command to user and create user
            var mappedUser = _mapper.Map<CreateUserCommand, ApplicationUser>(request);
            
            // Try to create new user
            var createUser = await _userManager.CreateAsync(mappedUser, request.Password);
            if (createUser.Succeeded == false)
            {
                return new AuthModel()
                {
                    Messages = createUser.Errors.Select(x => x.Description).ToList(),
                    IsAuthenticated = false,
                };
            }

            // Add role to user
            await _userManager.AddToRoleAsync(mappedUser, request.Role.ToString());
            
            // Generate token
            var jwtToken = await _jwtService.GenerateToken(mappedUser);

            var roles = await _userManager.GetRolesAsync(mappedUser).ConfigureAwait(false);

            return new AuthModel()
            {
                Messages = new List<string>()
                {
                    "Registration completed successfully"
                },
                IsAuthenticated = true,
                Token = jwtToken,
                UserName = mappedUser.UserName,
                Email = mappedUser.Email,
                Roles = roles.ToList()
            };
        }
    }
}