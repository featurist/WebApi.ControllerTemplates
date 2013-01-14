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

    public class CollectionController<TCollection, TInstance, TInserter, TIndexer, TSerialiser, TDeserialiser, TUrlGenerator> : CollectionController<TCollection, TInstance>
        where TInserter : Inserter<TInstance>
        where TIndexer : Indexer<TCollection>
        where TSerialiser : Serialiser<TCollection>
        where TDeserialiser : Deserialiser<TInstance>
        where TUrlGenerator : UrlGenerator
    {
        public CollectionController(TInserter inserter, TIndexer indexer, TSerialiser serialiser, TDeserialiser deserialiser, TUrlGenerator urlGenerator)
            : base(inserter, indexer, serialiser, deserialiser, urlGenerator)
        {
        }
    }

    public abstract class CollectionController<TCollection, TInstance> : ReadOnlyCollectionController<TCollection>
    {
        private readonly Inserter<TInstance> _inserter;
        private readonly Deserialiser<TInstance> _deserialiser;
        private readonly UrlGenerator _urlGenerator;

        protected CollectionController(Inserter<TInstance> inserter, Indexer<TCollection> indexer, Serialiser<TCollection> serialiser, Deserialiser<TInstance> deserialiser, UrlGenerator urlGenerator) : base(indexer, serialiser)
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
            var response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(url, UriKind.RelativeOrAbsolute);
            return response;
        }

        public virtual HttpResponseMessage Delete()
        {
            return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
        }
    }
}
