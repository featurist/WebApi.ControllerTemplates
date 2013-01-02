using System.Linq;
using System.Net.Http.Headers;

namespace WebApi.ControllerTemplates
{
    public static class HttpRequestMessageExtensions
    {
        public static string FirstIfNoneMatch(this HttpRequestHeaders headers)
        {
            var header = headers.IfNoneMatch.FirstOrDefault();
            return header != null ? header.Tag : null;
        }
    }
}
