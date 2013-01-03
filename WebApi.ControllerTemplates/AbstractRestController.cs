using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using RestfulRoles;

namespace WebApi.ControllerTemplates
{
    public abstract class AbstractRestController<TResource> : ApiController
    {
        protected virtual void AddCachingHeadersToResponse(HttpResponseMessage response, TResource resource)
        {
            AddETagHeaderToResponse(response, resource);
            AddLastModifiedHeaderToResponse(response, resource);
        }

        protected virtual void AddETagHeaderToResponse(HttpResponseMessage response, TResource resource)
        {
            var etagAware = resource as ETagAware;
            if (etagAware != null && string.IsNullOrEmpty(etagAware.ETag) == false)
            {
                response.Headers.ETag = new EntityTagHeaderValue(etagAware.ETag);
            }
        }

        protected virtual void AddLastModifiedHeaderToResponse(HttpResponseMessage response, TResource resource)
        {
            var lastModifiedAware = resource as LastModifiedAware;
            if (lastModifiedAware != null)
            {
                response.Content.Headers.LastModified = lastModifiedAware.LastModified;
            }
        }
    }
}
