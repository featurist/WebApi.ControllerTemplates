using System.Net;
using System.Net.Http;
using RestfulRoles;
using WebApi.ControllerTemplates.Roles;

namespace WebApi.ControllerTemplates
{
    public class ReadOnlyInstanceController<TInstance> : AbstractRestController<TInstance>
    {
        private readonly Retriever<TInstance> _retriever;
        private readonly Serialiser<TInstance> _serialiser;

        public ReadOnlyInstanceController(Retriever<TInstance> retriever, Serialiser<TInstance> serialiser)
        {
            _retriever = retriever;
            _serialiser = serialiser;
        }

        public virtual HttpResponseMessage Get(string id)
        {
            RetrieveResult<TInstance> result;
            try
            {
                result = _retriever.Retrieve(id, Request.Headers.IfModifiedSince, Request.Headers.FirstIfNoneMatch());
            }
            catch (NotFoundException e)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(e.Message) };
            }
            return result.WasRetrieved ? RespondWithInstance(result.Value) : new HttpResponseMessage(HttpStatusCode.NotModified);
        }

        public virtual HttpResponseMessage Head(string id)
        {
            return Get(id);
        }

        protected virtual HttpResponseMessage RespondWithInstance(TInstance instance)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = _serialiser.Serialise(instance);
            AddCachingHeadersToResponse(response, instance);
            return response;
        }
    }
}