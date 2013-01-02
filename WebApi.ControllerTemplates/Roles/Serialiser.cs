using System.Net.Http;

namespace WebApi.ControllerTemplates.Roles
{
    public interface Serialiser<in TInstance>
    {
        HttpContent Serialise(TInstance resource);
    }
}