using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Quixotic.LanguageServer.Handlers
{
    internal sealed class InitializeHandler : ILanguageProtocolInitializedHandler
    {
        public Task<Unit> Handle(InitializedParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}
