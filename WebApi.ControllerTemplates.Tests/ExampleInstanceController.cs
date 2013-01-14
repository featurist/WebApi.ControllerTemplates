using System.Net.Http;
using RestfulRoles;
using WebApi.ControllerTemplates.Roles;

namespace WebApi.ControllerTemplates.Tests
{
    public class ExampleInstanceController<TInstance, TRepo> : InstanceController<TInstance, TRepo> where TRepo : Retriever<TInstance>, Upserter<TInstance>, Serialiser<TInstance>, Deserialiser<TInstance>, Deleter
    {
        public ExampleInstanceController(TRepo repo)
            : base(repo)
        {
        }

        public new HttpResponseMessage Head(string id)
        {
            return base.Head(id);
        }

        public new HttpResponseMessage Delete(string id)
        {
            return base.Delete(id);
        }
    }
}