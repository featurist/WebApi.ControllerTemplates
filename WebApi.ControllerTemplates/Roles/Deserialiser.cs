using System.Net.Http;

namespace WebApi.ControllerTemplates.Roles
{
    public interface Deserialiser<out TInstance>
    {
        TInstance Deserialise(HttpRequestMessage request);
    }
}