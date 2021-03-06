using System.Net;
using RestfulRoles;

namespace WebApi.ControllerTemplates
{
    internal static class UpsertResultExtensions
    {
        public static HttpStatusCode ToHttpStatusCode(this UpsertResult result)
        {
            return result.WasCreate ? HttpStatusCode.Created : HttpStatusCode.OK;
        }
    }
}