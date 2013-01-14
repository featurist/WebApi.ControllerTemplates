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

    public class InstanceController<TInstance, TRepo, TSerialiser, TDeserialiser> : InstanceController<TInstance>
        where TRepo : Retriever<TInstance>, Upserter<TInstance>, Deleter
        where TSerialiser : Serialiser<TInstance>
        where TDeserialiser : Deserialiser<TInstance>
    {
        public InstanceController(TRepo repo, TSerialiser serialiser, TDeserialiser deserialiser)
            : base(repo, repo, repo, serialiser, deserialiser)
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
        public InstanceController(TRetriever retriever, TUpserter upserter, TDeleter deleter, TSerialiser serialiser, TDeserialiser deserialiser)
            : base(retriever, upserter, deleter, serialiser, deserialiser)
        {
        }
    }

    public abstract class InstanceController<TInstance> : ReadOnlyInstanceController<TInstance>
    {
        private readonly Upserter<TInstance> _upserter;
        private readonly Deserialiser<TInstance> _deserialiser;
        private readonly Deleter _deleter;

        protected InstanceController(Retriever<TInstance> retriever, Upserter<TInstance> upserter, Deleter deleter, Serialiser<TInstance> serialiser, Deserialiser<TInstance> deserialiser)
            : base(retriever, serialiser)
        {
            _upserter = upserter;
            _deserialiser = deserialiser;
            _deleter = deleter;
        }

        public virtual HttpResponseMessage Put(string id)
        {
            var instance = _deserialiser.Deserialise(Request);
            return PutDeserialised(id, instance);
        }

        public virtual HttpResponseMessage Delete(string id)
        {
            _deleter.Delete(id);
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        protected virtual HttpResponseMessage PutDeserialised(string id, TInstance instance)
        {
            try
            {
                var upsertResult = _upserter.Upsert(id, instance);
                return Request.CreateResponse(upsertResult.ToHttpStatusCode());
            }
            catch (ConflictException e)
            {
                var response = Request.CreateResponse(HttpStatusCode.Conflict);
                response.Content = new StringContent(e.Message);
                return response;
            }
        }
    }
}
