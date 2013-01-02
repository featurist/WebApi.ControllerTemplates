using System;
using System.Net;
using System.Net.Http;
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

    public abstract class CollectionController<TCollection, TInstance> : AbstractRestController<TCollection>
    {
        private readonly Inserter<TInstance> _inserter;
        private readonly Indexer<TCollection> _indexer;
        private readonly Serialiser<TCollection> _serialiser;
        private readonly Deserialiser<TInstance> _deserialiser;
        private readonly UrlGenerator _urlGenerator;

        protected CollectionController(Inserter<TInstance> inserter, Indexer<TCollection> indexer, Serialiser<TCollection> serialiser, Deserialiser<TInstance> deserialiser, UrlGenerator urlGenerator)
        {
            _inserter = inserter;
            _indexer = indexer;
            _serialiser = serialiser;
            _deserialiser = deserialiser;
            _urlGenerator = urlGenerator;
        }

        public HttpResponseMessage Get()
        {
            return new GetIndexController<TCollection>(_indexer, _serialiser) { Request = Request }.Get();
        }

        public HttpResponseMessage Post()
        {
            var deserialised = _deserialiser.Deserialise(Request);
            var id = _inserter.Insert(deserialised);
            var url = _urlGenerator.GenerateUrl(id);
            var response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(url, UriKind.RelativeOrAbsolute);
            return response;
        }

        public HttpResponseMessage Head()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public HttpResponseMessage Delete()
        {
            return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
        }
    }
}
