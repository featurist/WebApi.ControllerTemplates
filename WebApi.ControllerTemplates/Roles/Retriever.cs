using System;

namespace WebApi.ControllerTemplates.Roles
{
    public interface Retriever<TInstance>
    {
        RetrievedOrNotModified<TInstance> Retrieve(string id, DateTimeOffset? ifModifiedSince, string ifNoneMatch);
    }
}