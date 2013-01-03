using System;
using RestfulRoles;

namespace WebApi.ControllerTemplates.Tests
{
    public class Chart
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class ChartWithLastModifiedDate : Chart, LastModifiedAware
    {
        public DateTimeOffset? LastModified { get; set; }
    }

    public class ChartWithETag : Chart, ETagAware
    {
        public string ETag { get; set; }
    }
}