using Application.Features.Todos;
using AutoMapper;
using Data.Entities;

namespace Application.Mappers
{
    public class TodoItemProfile : Profile
    {
        public TodoItemProfile()
        {
            CreateMap<CreateTodoItemCommand, TodoItem>()
                .ForMember(
                    d => d.Completed,
                    s => s.MapFrom(sc => false));
        }
    }
}