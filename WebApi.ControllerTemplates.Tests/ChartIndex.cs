using System;

namespace WebApi.ControllerTemplates.Tests
{
    public class ChartIndex
    {
        public int Count;
    }

    public class ChartIndexWithETag : ChartIndex, ETagAware
    {
        public string ETag { get; set; }
    }

    public class ChartIndexWithLastModified : ChartIndex, LastModifiedAware
    {
        public DateTimeOffset? LastModified { get; set; }
    }
}