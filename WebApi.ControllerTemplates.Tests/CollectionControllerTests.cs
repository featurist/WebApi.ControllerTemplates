using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Should;

namespace WebApi.ControllerTemplates.Tests
{
    [TestFixture]
    public class CollectionControllerTests
    {
        const string ETag = "\"684897696a7c876b7a\"";

        [Test]
        public void GetRespondsWithDeserialisedIndex()
        {
            var repo = new ChartRepo { { "234", new Chart() }, { "345", new Chart() } };
            var controller = new CollectionController<ChartIndex, Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Get().Content.ReadAsStringAsync().Result.ShouldEqual("2 charts");
        }

        [Test]
        public void GetRespondsWith200()
        {
            var repo = new ChartRepo();
            var controller = new CollectionController<ChartIndex, Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            var response = controller.Get();
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void GetRespondsWithETagWhenIndexSupportsIt()
        {
            var repo = new ChartRepo { { "234", new Chart() }, { "345", new Chart() } };
            repo.IndexETag = ETag;
            var controller = new CollectionController<ChartIndexWithETag, Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Get().Headers.ETag.Tag.ShouldEqual(ETag);
        }

        [Test]
        public void GetRespondsWith304WhenETagAndIfNoneMatchAreEqual()
        {
            var repo = new ChartRepo { { "234", new Chart() }, { "345", new Chart() } };
            repo.IndexETag = ETag;
            var request = new HttpRequestMessage();
            var controller = new CollectionController<ChartIndexWithETag, Chart, ChartRepo>(repo) { Request = request };
            request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(ETag));
            controller.Get().StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWith304WhenLastModifiedAndIfModifiedSinceAreEqual()
        {
            var repo = new ChartRepo { { "234", new Chart() }, { "345", new Chart() } };
            repo.IndexLastModified = DateTimeOffset.Now;
            var request = new HttpRequestMessage();
            var controller = new CollectionController<ChartIndexWithETag, Chart, ChartRepo>(repo) { Request = request };
            request.Headers.IfModifiedSince = repo.IndexLastModified;
            controller.Get().StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWith200WhenLastModifiedIsLaterThanIfModifiedSince()
        {
            var repo = new ChartRepo { { "234", new Chart() }, { "345", new Chart() } };
            repo.IndexLastModified = DateTimeOffset.Now;
            var request = new HttpRequestMessage();
            var controller = new CollectionController<ChartIndexWithLastModified, Chart, ChartRepo>(repo) { Request = request };
            request.Headers.IfModifiedSince = repo.IndexLastModified.Value.AddSeconds(-1);
            controller.Get().StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void GetRespondsWith304WhenLastModifiedIsEarlierThanIfModifiedSince()
        {
            var repo = new ChartRepo { { "234", new Chart() }, { "345", new Chart() } };
            repo.IndexLastModified = DateTimeOffset.Now;
            var request = new HttpRequestMessage();
            var controller = new CollectionController<ChartIndexWithLastModified, Chart, ChartRepo>(repo) { Request = request };
            request.Headers.IfModifiedSince = repo.IndexLastModified.Value.AddSeconds(1);
            controller.Get().StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWithLastModifiedWhenIndexSupportsIt()
        {
            var repo = new ChartRepo { { "234", new Chart() }, { "345", new Chart() } };
            repo.IndexLastModified = DateTimeOffset.Now;
            var controller = new CollectionController<ChartIndexWithLastModified, Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Get().Content.Headers.LastModified.ShouldEqual(repo.IndexLastModified);
        }

        [Test]
        public void HeadRespondsWith200()
        {
            var repo = new ChartRepo();
            var controller = new CollectionController<ChartIndex, Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Head().StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void DeleteRespondsWith406()
        {
            var repo = new ChartRepo();
            var controller = new CollectionController<ChartIndex, Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Delete().StatusCode.ShouldEqual(HttpStatusCode.NotAcceptable);
        }

        [Test]
        public void PostInsertsDeserialisedInstance()
        {
            var repo = new ChartRepo();
            var controller = new CollectionController<ChartIndex, Chart, ChartRepo>(repo)
                                 {
                                     Request = new HttpRequestMessage { Content = new StringContent ("Selective Reflation") }
                                 };
            controller.Post();
            var chart = repo.Values.Single();
            chart.Title.ShouldEqual("Selective Reflation");
        }

        [Test]
        public void PostRespondsWith201()
        {
            var repo = new ChartRepo();
            var controller = new CollectionController<ChartIndex, Chart, ChartRepo>(repo)
            {
                Request = new HttpRequestMessage { Content = new StringContent("Selective Reflation") }
            };
            controller.Post().StatusCode.ShouldEqual(HttpStatusCode.Created);
        }

        [Test]
        public void PostRespondsWithLocation()
        {
            var repo = new ChartRepo();
            var controller = new CollectionController<ChartIndex, Chart, ChartRepo>(repo)
            {
                Request = new HttpRequestMessage { Content = new StringContent("Selective Reflation") }
            };
            var response = controller.Post();
            response.Headers.Location.ToString().ShouldEqual("/charts/" + repo.Keys.First());
        }
    }
}
