using System;
using System.Net;
using System.Net.Http;
using RestfulRoles;
using WebApi.ControllerTemplates.Roles;

namespace WebApi.ControllerTemplates
{
    public class CollectionController<TCollection, TInstance, TRepo> : CollectionController<TCollection, TInstance>
        where TRepo : Inserter<TInstance>, Indexer<TCollection>, Serialiser<TCollection>, Deserialiser<TInstance>, UrlGenerator
    {
        public CollectionController(TRepo repo) : base(repo, repo, repo, repo, repo)
        {
        }
    }

    public class CollectionController<TCollection, TInstance, TRepo, TUrlGenerator, TSerialiser, TDeserialiser> : CollectionController<TCollection, TInstance>
        where TRepo : Inserter<TInstance>, Indexer<TCollection>
        where TSerialiser : Serialiser<TCollection>
        where TDeserialiser : Deserialiser<TInstance>
        where TUrlGenerator : UrlGenerator
    {
        public CollectionController(TRepo repo, TUrlGenerator urlGenerator, TSerialiser serialiser, TDeserialiser deserialiser)
            : base(repo, repo, urlGenerator, serialiser, deserialiser)
        {
        }
    }

    public class CollectionController<TCollection, TInstance, TInserter, TIndexer, TUrlGenerator, TSerialiser, TDeserialiser> : CollectionController<TCollection, TInstance>
        where TInserter : Inserter<TInstance>
        where TIndexer : Indexer<TCollection>
        where TSerialiser : Serialiser<TCollection>
        where TDeserialiser : Deserialiser<TInstance>
        where TUrlGenerator : UrlGenerator
    {
        public CollectionController(TInserter inserter, TIndexer indexer, TUrlGenerator urlGenerator, TSerialiser serialiser, TDeserialiser deserialiser)
            : base(inserter, indexer, urlGenerator, serialiser, deserialiser)
        {
        }
    }

    public abstract class CollectionController<TCollection, TInstance> : ReadOnlyCollectionController<TCollection>
    {
        private readonly Inserter<TInstance> _inserter;
        private readonly Deserialiser<TInstance> _deserialiser;
        private readonly UrlGenerator _urlGenerator;

        protected CollectionController(Inserter<TInstance> inserter, Indexer<TCollection> indexer, UrlGenerator urlGenerator, Serialiser<TCollection> serialiser, Deserialiser<TInstance> deserialiser) : base(indexer, serialiser)
        {
            _inserter = inserter;
            _deserialiser = deserialiser;
            _urlGenerator = urlGenerator;
        }

        public virtual HttpResponseMessage Post()
        {
            var deserialised = _deserialiser.Deserialise(Request);
            var id = _inserter.Insert(deserialised);
            var url = _urlGenerator.GenerateUrl(id);
            return RespondWithLocation(url);
        }

        public virtual HttpResponseMessage Delete()
        {
            return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
        }

        protected virtual HttpResponseMessage RespondWithLocation(string url)
        {
            var response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(url, UriKind.RelativeOrAbsolute);
            return response;
        }
    }
}
