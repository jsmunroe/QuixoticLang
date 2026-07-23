using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Quixotic.LanguageServer.Services
{
    public class DocumentManager
    {
        private readonly Dictionary<DocumentUri, string> _documents = [];

        public void Open(DocumentUri uri, string text)
        {
            _documents[uri] = text;
        }

        public void Update(DocumentUri uri, string text)
        {
            _documents[uri] = text;
        }

        public void Close(DocumentUri uri)
        {
            _documents.Remove(uri);
        }

        public string? Get(DocumentUri uri)
        {
            return _documents.TryGetValue(uri, out var text) ? text : null;
        }
    }
}
