using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Todos
{
    public class GetTodoItemsQuery : IRequest<List<TodoItem>>
    {
        
    };

    public class GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, List<TodoItem>>
    {
        private readonly ApplicationDbContext _context;
        
        public GetTodoItemsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }
            
        public async Task<List<TodoItem>> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
        {
            return await _context.TodoItems.ToListAsync(cancellationToken: cancellationToken);
        }
    }
}