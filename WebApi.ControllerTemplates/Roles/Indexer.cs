using System;

namespace WebApi.ControllerTemplates.Roles
{
    public interface Indexer<TCollection>
    {
        RetrievedOrNotModified<TCollection> Index(DateTimeOffset? ifModifiedSince, string ifNoneMatch);
    }
}