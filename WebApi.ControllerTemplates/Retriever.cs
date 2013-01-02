using System;

namespace WebApi.ControllerTemplates
{
    public interface Retriever<TInstance>
    {
        RetrievedOrNotModified<TInstance> Retrieve(string id, DateTimeOffset? ifModifiedSince, string ifNoneMatch);
    }
}