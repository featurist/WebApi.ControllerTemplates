using System;

namespace WebApi.ControllerTemplates
{
    public interface LastModifiedAware
    {
        DateTimeOffset? LastModified { get; }
    }
}
