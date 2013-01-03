using System.Net;
using System.Net.Http;
using RestfulRoles;
using WebApi.ControllerTemplates.Roles;

namespace WebApi.ControllerTemplates
{
    public class InstanceController<TInstance, TRepo> : InstanceController<TInstance>
        where TRepo : Retriever<TInstance>, Upserter<TInstance>, Serialiser<TInstance>, Deserialiser<TInstance>, Deleter
    {
        public InstanceController(TRepo repo) : base(repo, repo, repo, repo, repo)
        {
        }
    }
    
    public class InstanceController<TInstance, TRetriever, TUpserter, TSerialiser, TDeserialiser, TDeleter> : InstanceController<TInstance>
        where TRetriever : Retriever<TInstance>
        where TUpserter : Upserter<TInstance>
        where TSerialiser : Serialiser<TInstance>
        where TDeserialiser : Deserialiser<TInstance>
        where TDeleter : Deleter
    {
        public InstanceController(TRetriever retriever, TUpserter upserter, TSerialiser serialiser, TDeserialiser deserialiser, TDeleter deleter)
            : base(retriever, upserter, serialiser, deserialiser, deleter)
        {
        }
    }

    public abstract class InstanceController<TInstance> : AbstractRestController<TInstance>
    {
        private readonly Retriever<TInstance> _retriever;
        private readonly Upserter<TInstance> _upserter;
        private readonly Serialiser<TInstance> _serialiser;
        private readonly Deserialiser<TInstance> _deserialiser;
        private readonly Deleter _deleter;

        protected InstanceController(Retriever<TInstance> retriever, Upserter<TInstance> upserter, Serialiser<TInstance> serialiser, Deserialiser<TInstance> deserialiser, Deleter deleter)
        {
            _retriever = retriever;
            _upserter = upserter;
            _serialiser = serialiser;
            _deserialiser = deserialiser;
            _deleter = deleter;
        }

        public virtual HttpResponseMessage Get(string id)
        {
            return new GetInstanceController<TInstance>(_retriever, _serialiser) { Request = Request }.Get(id);
        }

        public virtual HttpResponseMessage Put(string id)
        {
            var instance = _deserialiser.Deserialise(Request);
            var upsertResult = _upserter.Upsert(id, instance);
            var response = Request.CreateResponse(upsertResult.ToHttpStatusCode());
            return response;
        }

        public virtual HttpResponseMessage Delete(string id)
        {
            _deleter.Delete(id);
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        public virtual HttpResponseMessage Head(string id)
        {
            return Get(id);
        }
    }
}
