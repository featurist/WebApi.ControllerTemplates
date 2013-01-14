using RestfulRoles;
using WebApi.ControllerTemplates.Roles;

namespace WebApi.ControllerTemplates.Tests
{
    public class ExampleCollectionController<TCollection, TInstance, TRepo> :
        CollectionController<TCollection, TInstance, TRepo>
        where TRepo : Inserter<TInstance>, Indexer<TCollection>, Serialiser<TCollection>, Deserialiser<TInstance>, UrlGenerator
    {
        public ExampleCollectionController(TRepo repo) : base(repo)
        {
        }
    }
}