using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Data.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Todos
{
    public class CreateTodoItemCommand : IRequest<int>
    {
        public string Title { get; set; }
    }

    public class ValidateCreateTodoItemCommand : AbstractValidator<CreateTodoItemCommand>
    {
        public ValidateCreateTodoItemCommand()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(128);
        }
    }

    public class CreateTodoItemHandler : IRequestHandler<CreateTodoItemCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateTodoItemHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var newTodoItem = _mapper.Map<CreateTodoItemCommand, TodoItem>(request);

            await _context.TodoItems.AddAsync(newTodoItem, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newTodoItem.Id;
        }
    }
}