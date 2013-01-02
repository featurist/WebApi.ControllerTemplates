using System;

namespace WebApi.ControllerTemplates
{
    public interface Indexer<TCollection>
    {
        RetrievedOrNotModified<TCollection> Index(DateTimeOffset? ifModifiedSince, string ifNoneMatch);
    }
}