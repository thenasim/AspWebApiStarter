using System.Threading.Tasks;
using Application.Features.Todos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TodoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTodos()
        {
            return Ok(await _mediator.Send(new GetTodoItemsQuery()));
        }

        [HttpPost]
        public async Task<IActionResult> NewTodo(CreateTodoItemCommand todoCommand)
        {
            return Ok(await _mediator.Send(todoCommand));
        }
    }
}