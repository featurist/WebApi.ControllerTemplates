using System.Net;
using System.Net.Http;
using WebApi.ControllerTemplates.Roles;

namespace WebApi.ControllerTemplates
{
    public class GetInstanceController<TInstance> : AbstractRestController<TInstance>
    {
        private readonly Retriever<TInstance> _retriever;
        private readonly Serialiser<TInstance> _serialiser;

        public GetInstanceController(Retriever<TInstance> retriever, Serialiser<TInstance> serialiser)
        {
            _retriever = retriever;
            _serialiser = serialiser;
        }

        public HttpResponseMessage Get(string id)
        {
            RetrievedOrNotModified<TInstance> result;
            try
            {
                result = _retriever.Retrieve(id, Request.Headers.IfModifiedSince, Request.Headers.FirstIfNoneMatch());
            }
            catch (NotFoundException e)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(e.Message) };
            }
            if (result.WasRetrieved)
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);
                var instance = result.Value;
                response.Content = _serialiser.Serialise(instance);
                AddCachingHeadersToResponse(response, instance);
                return response;
            }
            return new HttpResponseMessage(HttpStatusCode.NotModified);
        }
    }
}