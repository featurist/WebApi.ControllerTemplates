using System;
using System.Collections.Generic;
using System.Net.Http;
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
        public RetrievedOrNotModified<Chart> Retrieve(string id, DateTimeOffset? ifModifiedSince, string ifNoneMatch)
        {
            if (!ContainsKey(id))
            {
                throw new NotFoundException("There was no chart with id=" + id);
            }
            var chart = this[id];
            var eTagAware = chart as ETagAware;
            if (eTagAware != null && eTagAware.ETag == ifNoneMatch)
            {
                return RetrievedOrNotModified<Chart>.NotModified;
            }
            
            var lastModifiedAware = chart as LastModifiedAware;
            if (lastModifiedAware != null && ifModifiedSince.HasValue &&
                lastModifiedAware.LastModified <= ifModifiedSince.Value)
            {
                return RetrievedOrNotModified<Chart>.NotModified;
            }
            
            return RetrievedOrNotModified<Chart>.Retrieved(chart);
        }

        public UpsertResult Upsert(string id, Chart chart)
        {
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

        public RetrievedOrNotModified<ChartIndex> Index(DateTimeOffset? ifModifiedSince, string ifNoneMatch)
        {
            return new RetrievedOrNotModified<ChartIndex> { Value = new ChartIndex { Count = Count }, WasRetrieved = true };
        }

        RetrievedOrNotModified<ChartIndexWithETag> Indexer<ChartIndexWithETag>.Index(DateTimeOffset? ifModifiedSince, string ifNoneMatch)
        {
            return ifNoneMatch == IndexETag ? RetrievedOrNotModified<ChartIndexWithETag>.NotModified
                : new RetrievedOrNotModified<ChartIndexWithETag> { Value = new ChartIndexWithETag { Count = Count, ETag = IndexETag }, WasRetrieved = true };
        }

        RetrievedOrNotModified<ChartIndexWithLastModified> Indexer<ChartIndexWithLastModified>.Index(DateTimeOffset? ifModifiedSince, string ifNoneMatch)
        {
            if (IndexLastModified.HasValue && ifModifiedSince.HasValue && IndexLastModified <= ifModifiedSince.Value)
            {
                return RetrievedOrNotModified<ChartIndexWithLastModified>.NotModified;
            }
            return new RetrievedOrNotModified<ChartIndexWithLastModified> { Value = new ChartIndexWithLastModified { Count = Count, LastModified = IndexLastModified }, WasRetrieved = true };
        }

        public string IndexETag { get; set; }

        public DateTimeOffset? IndexLastModified { get; set; }
    }
}