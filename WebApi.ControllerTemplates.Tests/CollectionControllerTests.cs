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

        private ExampleCollectionController<ChartIndex, Chart, ChartRepo> CreateController()
        {
            return new ExampleCollectionController<ChartIndex, Chart, ChartRepo>(_repo) { Request = new HttpRequestMessage() };
        }

        private ExampleCollectionController<ChartIndexWithETag, Chart, ChartRepo> CreateControllerWithETagSupport()
        {
            return new ExampleCollectionController<ChartIndexWithETag, Chart, ChartRepo>(_repo) { Request = new HttpRequestMessage() };
        }

        private ExampleCollectionController<ChartIndexWithLastModified, Chart, ChartRepo> CreateControllerWithLastModifiedSupport()
        {
            return new ExampleCollectionController<ChartIndexWithLastModified, Chart, ChartRepo>(_repo) { Request = new HttpRequestMessage() };
        }

        private ChartRepo _repo;
        
        [SetUp]
        public void CreateContext()
        {
            _repo = new ChartRepo();
        }

        [Test]
        public void GetRespondsWithDeserialisedIndex()
        {
            _repo.AddCharts(2);
            var controller = CreateController();
            controller.Get().Content.ReadAsStringAsync().Result.ShouldEqual("2 charts");
        }

        [Test]
        public void GetRespondsWith200()
        {
            var controller = CreateController();
            controller.Get().StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void GetRespondsWithETagWhenIndexSupportsIt()
        {
            _repo.AddCharts(2);
            _repo.IndexETag = ETag;
            var controller = CreateControllerWithETagSupport();
            controller.Get().Headers.ETag.Tag.ShouldEqual(ETag);
        }

        [Test]
        public void GetRespondsWith304WhenETagAndIfNoneMatchAreEqual()
        {
            _repo.AddCharts(2);
            _repo.IndexETag = ETag;
            var controller = CreateControllerWithETagSupport();
            controller.Request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(ETag));
            controller.Get().StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWith304WhenLastModifiedAndIfModifiedSinceAreEqual()
        {
            _repo.AddCharts(2);
            _repo.IndexLastModified = DateTimeOffset.Now;
            var request = new HttpRequestMessage();
            var controller = CreateControllerWithETagSupport();
            request.Headers.IfModifiedSince = _repo.IndexLastModified;
            controller.Get().StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWith200WhenLastModifiedIsLaterThanIfModifiedSince()
        {
            _repo.AddCharts(2);
            _repo.IndexLastModified = DateTimeOffset.Now;
            var controller = CreateControllerWithLastModifiedSupport();
            controller.Request.Headers.IfModifiedSince = _repo.IndexLastModified.Value.AddSeconds(-1);
            controller.Get().StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void GetRespondsWith304WhenLastModifiedIsEarlierThanIfModifiedSince()
        {
            _repo.AddCharts(2);
            _repo.IndexLastModified = DateTimeOffset.Now;
            var controller = CreateControllerWithLastModifiedSupport();
            controller.Request.Headers.IfModifiedSince = _repo.IndexLastModified.Value.AddSeconds(1);
            controller.Get().StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWithLastModifiedWhenIndexSupportsIt()
        {
            _repo.AddCharts(2);
            _repo.IndexLastModified = DateTimeOffset.Now;
            var controller = CreateControllerWithLastModifiedSupport();
            controller.Get().Content.Headers.LastModified.ShouldEqual(_repo.IndexLastModified);
        }

        [Test]
        public void HeadRespondsWith200()
        {
            CreateController().Head().StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void DeleteRespondsWith406()
        {
            CreateController().Delete().StatusCode.ShouldEqual(HttpStatusCode.NotAcceptable);
        }

        [Test]
        public void PostInsertsDeserialisedInstance()
        {
            var controller = CreateController();
            controller.Request.Content = new StringContent("crunk");
            controller.Post();
            var chart = _repo.Values.Single();
            chart.Title.ShouldEqual("crunk");
        }

        [Test]
        public void PostRespondsWith201()
        {
            var controller = CreateController();
            controller.Request.Content = new StringContent("difribulate");
            controller.Post().StatusCode.ShouldEqual(HttpStatusCode.Created);
        }

        [Test]
        public void PostRespondsWithLocation()
        {
            var controller = CreateController();
            controller.Request.Content = new StringContent("blimey");
            var response = controller.Post();
            response.Headers.Location.ToString().ShouldEqual("/charts/" + _repo.Keys.First());
        }
    }
}
