using System.Net.Http;

namespace WebApi.ControllerTemplates
{
    public interface Serialiser<in TInstance>
    {
        HttpContent Serialise(TInstance resource);
    }
}