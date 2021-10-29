using System.Threading.Tasks;
using Application.Features.Users;
using Data.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost("/register")]
        public async Task<IActionResult> RegisterAsUser(CreateUserCommand userCommand)
        {
            userCommand.Role = Roles.User;
            
            var result = await _mediator.Send(userCommand);
            if (result.IsAuthenticated == false)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}