using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusCommerce.Application.Common
{
    public record PingQuery : IRequest<string>;
    public class PingQueryHandler : IRequestHandler<PingQuery, string>
    {
        public Task<string> Handle(PingQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult("Pong, MediatR đang hoạt động.");
        }
    }
}
