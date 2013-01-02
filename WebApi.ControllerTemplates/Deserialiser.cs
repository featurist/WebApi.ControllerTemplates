using System.Net.Http;

namespace WebApi.ControllerTemplates
{
    public interface Deserialiser<out TInstance>
    {
        TInstance Deserialise(HttpRequestMessage request);
    }
}