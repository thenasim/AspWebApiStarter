using System.Threading.Tasks;
using Application.Config;
using Application.Features.Users;
using AutoMapper;
using Data.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly CookieConfig _cookieConfig;

        public AuthController(IMediator mediator, IMapper mapper, CookieConfig cookieConfig)
        {
            _mediator = mediator;
            _mapper = mapper;
            _cookieConfig = cookieConfig;
        }

        [HttpGet("secret")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Secret()
        {
            return Ok(await Task.FromResult("Secret page"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserQuery loginUserQuery)
        {
            var result = await _mediator.Send(loginUserQuery);
            if (result.IsAuthenticated == false)
            {
                return BadRequest(result);
            }
            
            var cookieOptions = new CookieOptions() {
                HttpOnly = true,
            }; 
            Response.Cookies.Append(_cookieConfig.JwtKey, result.Token, cookieOptions);

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsUser([FromBody] CreateUserCommandDto userCommandDto)
        {
            var userCommand = _mapper.Map<CreateUserCommandDto, CreateUserCommand>(userCommandDto);
            
            // ? For role User only
            userCommand.Role = Roles.User;
            
            var result = await _mediator.Send(userCommand);
            if (result.IsAuthenticated == false)
            {
                return BadRequest(result);
            }
            
            var cookieOptions = new CookieOptions() {
                HttpOnly = true,
            }; 
            Response.Cookies.Append(_cookieConfig.JwtKey, result.Token, cookieOptions);

            return Ok(result);
        }
    }
}
