using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features
{
    public record PingQuery() : IRequest<string>;

    public class PingQueryHandler : IRequestHandler<PingQuery, string>
    {
        public async Task<string> Handle(PingQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult("Pong");
        }
    }
}