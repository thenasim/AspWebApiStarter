using Application.Features.Todos;
using AutoMapper;
using Data.Models;

namespace Application.Mappers
{
    public class TodoItemProfile : Profile
    {
        public TodoItemProfile()
        {
            CreateMap<CreateTodoItemCommand, TodoItem>()
                .ForMember(
                    s => s.Completed,
                    d => d.MapFrom(s => false));
        }
    }
}