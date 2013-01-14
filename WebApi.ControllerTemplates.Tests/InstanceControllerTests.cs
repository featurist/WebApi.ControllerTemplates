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
    public class InstanceControllerTests
    {
        const string ETag = "\"686897696a7c876b7e\"";

        private InstanceController<Chart, ChartRepo> CreateController()
        {
            return new InstanceController<Chart, ChartRepo>(_repo) { Request = _request };
        }

        private ChartRepo _repo;
        private HttpRequestMessage _request;

        [SetUp]
        public void CreateRepo()
        {
            _repo = new ChartRepo();
            _request = new HttpRequestMessage();
        }

        [Test]
        public void GetRespondsWithSerialisedRetrievedInstance()
        {
            _repo.AddCharts(1);
            var controller = CreateController();
            controller.Get(_repo.Keys.First()).Content.ReadAsStringAsync().Result.ShouldEqual(_repo.Values.First().Title);
        }

        [Test]
        public void GetRespondsWithLastModifiedWhenTheContentSupportsIt()
        {
            DateTimeOffset modified = DateTime.Today;
            _repo.Add("123", new ChartWithLastModifiedDate { Id = "123", LastModified = modified });
            var controller = CreateController();
            controller.Get("123").Content.Headers.LastModified.ShouldEqual(modified);
        }

        [Test]
        public void GetRespondsWith200WhenContentExists()
        {
            _repo.AddCharts(1);
            var controller = CreateController();
            controller.Get(_repo.Keys.First()).StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void GetRespondsWithETagWhenContentIsETagAware()
        {
            _repo.Add("123", new ChartWithETag { Id = "123", ETag = ETag });
            var controller = CreateController();
            controller.Get("123").Headers.ETag.Tag.ShouldEqual(ETag);
        }

        [Test]
        public void HeadRespondsWith200WhenContentExists()
        {
            _repo.AddCharts(1);
            var controller = CreateController();
            controller.Head(_repo.Keys.First()).StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void GetRespondsWith404WhenContentDoesNotExist()
        {
            var controller = CreateController();
            controller.Get("666").StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Test]
        public void GetRespondsWith304WhenIfModifiedSinceEqualsLastModified()
        {
            var modified = DateTimeOffset.Now;
            _repo.Add("432", new ChartWithLastModifiedDate { Id = "432", LastModified = modified });
            var controller = CreateController();
            _request.Headers.IfModifiedSince = modified;
            controller.Get("432").StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWith304WhenIfModifiedSinceIsLaterThanLastModified()
        {
            var modified = DateTimeOffset.Now;
            _repo.Add("654", new ChartWithLastModifiedDate { Id = "654", LastModified = modified });
            var controller = CreateController();
            _request.Headers.IfModifiedSince = modified.AddSeconds(1);
            controller.Get("654").StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWith200WhenIfModifiedSinceIsEarlierThanLastModified()
        {
            var modified = DateTimeOffset.Now;
            _repo.Add("098", new ChartWithLastModifiedDate { Id = "098", LastModified = modified });
            var controller = CreateController();
            _request.Headers.IfModifiedSince = modified.AddSeconds(-1);
            controller.Get("098").StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void GetRespondsWithContentWhenIfModifiedSinceIsEarlierThanLastModified()
        {
            var modified = DateTimeOffset.Now;
            _repo.Add("876", new ChartWithLastModifiedDate { Id = "876", LastModified = modified });
            var controller = CreateController();
            _request.Headers.IfModifiedSince = modified.AddSeconds(-1);
            controller.Get("876").Content.ReadAsStringAsync().Result.ShouldEqual("Chart 876");
        }

        [Test]
        public void GetRespondsWith304WhenIfNoneMatchEqualsInstanceETag()
        {
            _repo.Add("012", new ChartWithETag { Id = "012", ETag = ETag });
            var controller = CreateController();
            _request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(ETag));
            controller.Get("012").StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWith200WhenIfNoneMatchDoesNotEqualInstanceETag()
        {
            _repo.Add("210", new ChartWithETag { Id = "210", ETag = ETag });
            var controller = CreateController();
            _request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(ETag.Replace("9", "6")));
            controller.Get("210").StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void HeadRespondsWith404WhenContentDoesNotExist()
        {
            CreateController().Head("777").StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Test]
        public void PutInsertsDeserialisedInstance()
        {
            var controller = CreateController();
            _request.Content = new StringContent("shamone");
            controller.Put("123");
            _repo["123"].Title.ShouldEqual("shamone");
        }

        [Test]
        public void PutRespondsWith201WhenInstanceWasCreated()
        {
            var controller = CreateController();
            _request.Content = new StringContent("oosh");
            var response = controller.Put("321");
            response.StatusCode.ShouldEqual(HttpStatusCode.Created);
        }

        [Test]
        public void PutRespondsWith200WhenInstanceWasUpdated()
        {
            _repo.Add("765", new Chart { Title = "bingo" });
            var controller = CreateController();
            _request.Content = new StringContent("bingo");
            var response = controller.Put("765");
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void PutRespondsWith409WhenUpsertConflicts()
        {
            _repo.EnableConflicts = true;
            var controller = CreateController();
            _request.Content = new StringContent("oosh");
            var response = controller.Put("765");
            response.StatusCode.ShouldEqual(HttpStatusCode.Conflict);
        }

        [Test]
        public void PutUpdatesDeserialisedInstance()
        {
            _repo.Add("567", new Chart { Title = "Original" });
            var controller = CreateController();
            _request.Content = new StringContent("Updated");
            controller.Put("567");
            _repo["567"].Title.ShouldEqual("Updated");
        }

        [Test]
        public void DeleteDeletesInstanceById()
        {
            _repo.AddCharts(1);
            var controller = CreateController();
            controller.Delete(_repo.Keys.First());
            _repo.Count.ShouldEqual(0);
        }

        [Test]
        public void DeleteRespondsWith204()
        {
            _repo.AddCharts(1);
            var controller = CreateController();
            controller.Delete(_repo.Keys.First()).StatusCode.ShouldEqual(HttpStatusCode.NoContent);
        }
    }
}
