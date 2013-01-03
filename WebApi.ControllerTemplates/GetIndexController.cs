using System.Net;
using System.Net.Http;
using RestfulRoles;
using WebApi.ControllerTemplates.Roles;

namespace WebApi.ControllerTemplates
{
    public class GetIndexController<TCollection> : AbstractRestController<TCollection>
    {
        private readonly Indexer<TCollection> _indexer;
        private readonly Serialiser<TCollection> _serialiser;

        public GetIndexController(Indexer<TCollection> indexer, Serialiser<TCollection> serialiser)
        {
            _indexer = indexer;
            _serialiser = serialiser;
        }

        public virtual HttpResponseMessage Get()
        {
            var index = _indexer.Index(Request.Headers.IfModifiedSince, Request.Headers.FirstIfNoneMatch());
            if (index.WasRetrieved)
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = _serialiser.Serialise(index.Value);
                AddCachingHeadersToResponse(response, index.Value);
                return response;
            }
            return new HttpResponseMessage(HttpStatusCode.NotModified);
        }
    }
}