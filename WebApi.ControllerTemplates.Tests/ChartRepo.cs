using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using RestfulRoles;
using WebApi.ControllerTemplates.Roles;

namespace WebApi.ControllerTemplates.Tests
{
    public class ChartRepo : Dictionary<string, Chart>,
        Retriever<Chart>,
        Upserter<Chart>,
        Serialiser<Chart>,
        Deserialiser<Chart>,
        Deleter,
        Inserter<Chart>,
        Serialiser<ChartIndex>,
        UrlGenerator,
        Indexer<ChartIndex>,
        Indexer<ChartIndexWithETag>,
        Indexer<ChartIndexWithLastModified>
    {
        public RetrieveResult<Chart> Retrieve(string id, DateTimeOffset? ifModifiedSince, string ifNoneMatch)
        {
            if (!ContainsKey(id))
            {
                throw new NotFoundException("There was no chart with id=" + id);
            }
            var chart = this[id];
            var eTagAware = chart as ETagAware;
            if (eTagAware != null && eTagAware.ETag == ifNoneMatch)
            {
                return RetrieveResult<Chart>.NotModified;
            }
            
            var lastModifiedAware = chart as LastModifiedAware;
            if (lastModifiedAware != null && ifModifiedSince.HasValue &&
                lastModifiedAware.LastModified <= ifModifiedSince.Value)
            {
                return RetrieveResult<Chart>.NotModified;
            }

            return RetrieveResult<Chart>.Retrieved(chart);
        }

        public UpsertResult Upsert(string id, Chart chart)
        {
            if (EnableConflicts) throw new UpsertConflictException("There was a conflict upserting chart " + id);

            if (ContainsKey(id))
            {
                this[id] = chart;
                return new UpsertResult { WasCreate = false };
            }
            this[id] = chart;
            return new UpsertResult { WasCreate = true };
        }

        public HttpContent Serialise(Chart chart)
        {
            return new StringContent("Chart " + chart.Id);
        }

        public Chart Deserialise(HttpRequestMessage request)
        {
            return new Chart { Title = request.Content.ReadAsStringAsync().Result };
        }

        public void Delete(string id)
        {
            Remove(id);
        }

        public string Insert(Chart chart)
        {
            var id = Guid.NewGuid().ToString();
            Upsert(id, chart);
            return id;
        }

        public HttpContent Serialise(ChartIndex chartIndex)
        {
            return new StringContent(chartIndex.Count + " charts");
        }

        public string GenerateUrl(string id)
        {
            return "/charts/" + id;
        }

        public RetrieveResult<ChartIndex> Index(DateTimeOffset? ifModifiedSince, string ifNoneMatch)
        {
            return new RetrieveResult<ChartIndex> { Value = new ChartIndex { Count = Count }, WasRetrieved = true };
        }

        RetrieveResult<ChartIndexWithETag> Indexer<ChartIndexWithETag>.Index(DateTimeOffset? ifModifiedSince, string ifNoneMatch)
        {
            return ifNoneMatch == IndexETag ? RetrieveResult<ChartIndexWithETag>.NotModified
                : new RetrieveResult<ChartIndexWithETag> { Value = new ChartIndexWithETag { Count = Count, ETag = IndexETag }, WasRetrieved = true };
        }

        RetrieveResult<ChartIndexWithLastModified> Indexer<ChartIndexWithLastModified>.Index(DateTimeOffset? ifModifiedSince, string ifNoneMatch)
        {
            if (IndexLastModified.HasValue && ifModifiedSince.HasValue && IndexLastModified <= ifModifiedSince.Value)
            {
                return RetrieveResult<ChartIndexWithLastModified>.NotModified;
            }
            return new RetrieveResult<ChartIndexWithLastModified> { Value = new ChartIndexWithLastModified { Count = Count, LastModified = IndexLastModified }, WasRetrieved = true };
        }

        public string IndexETag { get; set; }

        public DateTimeOffset? IndexLastModified { get; set; }

        public bool EnableConflicts { get; set; }

        public void AddCharts(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var id = i.ToString(CultureInfo.InvariantCulture);
                Add(id, new Chart { Id = id, Title = "Chart " + i });
            }
        }
    }
}