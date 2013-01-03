using System;
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

        [Test]
        public void GetRespondsWithSerialisedRetrievedInstance()
        {
            var repo = new ChartRepo {{"123", new Chart {Id = "123"}}};
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Get("123").Content.ReadAsStringAsync().Result.ShouldEqual("Chart 123");
        }

        [Test]
        public void GetRespondsWithLastModifiedWhenTheContentSupportsIt()
        {
            DateTimeOffset modified = DateTime.Today;
            var repo = new ChartRepo { { "123", new ChartWithLastModifiedDate { Id = "123", LastModified = modified } } };
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Get("123").Content.Headers.LastModified.ShouldEqual(modified);
        }

        [Test]
        public void GetRespondsWith200WhenContentExists()
        {
            var repo = new ChartRepo { { "123", new Chart { Id = "123" } } };
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Get("123").StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void GetRespondsWithETagWhenContentIsETagAware()
        {
            var repo = new ChartRepo { { "123", new ChartWithETag { Id = "123", ETag = ETag } } };
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Get("123").Headers.ETag.Tag.ShouldEqual(ETag);
        }

        [Test]
        public void HeadRespondsWith200WhenContentExists()
        {
            var repo = new ChartRepo { { "123", new Chart { Id = "123" } } };
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Head("123").StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void GetRespondsWith404WhenContentDoesNotExist()
        {
            var repo = new ChartRepo();
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Get("666").StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Test]
        public void GetRespondsWith304WhenIfModifiedSinceEqualsLastModified()
        {
            var modified = DateTimeOffset.Now;
            var repo = new ChartRepo { { "432", new ChartWithLastModifiedDate { Id = "432", LastModified = modified } } };
            var request = new HttpRequestMessage();
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = request };
            request.Headers.IfModifiedSince = modified;
            controller.Get("432").StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWith304WhenIfModifiedSinceIsLaterThanLastModified()
        {
            var modified = DateTimeOffset.Now;
            var repo = new ChartRepo { { "432", new ChartWithLastModifiedDate { Id = "432", LastModified = modified } } };
            var request = new HttpRequestMessage();
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = request };
            request.Headers.IfModifiedSince = modified.AddSeconds(1);
            controller.Get("432").StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWith200WhenIfModifiedSinceIsEarlierThanLastModified()
        {
            var modified = DateTimeOffset.Now;
            var repo = new ChartRepo { { "432", new ChartWithLastModifiedDate { Id = "432", LastModified = modified } } };
            var request = new HttpRequestMessage();
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = request };
            request.Headers.IfModifiedSince = modified.AddSeconds(-1);
            controller.Get("432").StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void GetRespondsWithContentWhenIfModifiedSinceIsEarlierThanLastModified()
        {
            var modified = DateTimeOffset.Now;
            var repo = new ChartRepo { { "876", new ChartWithLastModifiedDate { Id = "876", LastModified = modified } } };
            var request = new HttpRequestMessage();
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = request };
            request.Headers.IfModifiedSince = modified.AddSeconds(-1);
            controller.Get("876").Content.ReadAsStringAsync().Result.ShouldEqual("Chart 876");
        }

        [Test]
        public void GetRespondsWith304WhenIfNoneMatchEqualsInstanceETag()
        {
            var repo = new ChartRepo { { "012", new ChartWithETag { Id = "012", ETag = ETag } } };
            var request = new HttpRequestMessage();
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = request };
            request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(ETag));
            controller.Get("012").StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void GetRespondsWith200WhenIfNoneMatchDoesNotEqualInstanceETag()
        {
            var repo = new ChartRepo { { "210", new ChartWithETag { Id = "210", ETag = ETag } } };
            var request = new HttpRequestMessage();
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = request };
            request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(ETag.Replace("9", "6")));
            controller.Get("210").StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void HeadRespondsWith404WhenContentDoesNotExist()
        {
            var repo = new ChartRepo();
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Head("777").StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Test]
        public void PutInsertsDeserialisedInstance()
        {
            var repo = new ChartRepo();
            var controller = new InstanceController<Chart, ChartRepo>(repo)
            {
                Request =  new HttpRequestMessage { Content = new StringContent ("Selective Reflation") }
            };
            controller.Put("123");
            repo["123"].Title.ShouldEqual("Selective Reflation");
        }

        [Test]
        public void PutRespondsWith201WhenInstanceWasCreated()
        {
            var repo = new ChartRepo();
            var controller = new InstanceController<Chart, ChartRepo>(repo)
            {
                Request = new HttpRequestMessage { Content = new StringContent("Quantitative Easing") }
            };
            var response = controller.Put("321");
            response.StatusCode.ShouldEqual(HttpStatusCode.Created);
        }

        [Test]
        public void PutRespondsWith200WhenInstanceWasUpdated()
        {
            var repo = new ChartRepo { { "765", new Chart { Title = "Japanese Inflation" } } };
            var controller = new InstanceController<Chart, ChartRepo>(repo)
            {
                Request = new HttpRequestMessage { Content = new StringContent("Japanese Inflation") }
            };
            var response = controller.Put("765");
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void PutRespondsWith409WhenUpsertConflicts()
        {
            var repo = new ChartRepo { EnableConflicts = true };
            var controller = new InstanceController<Chart, ChartRepo>(repo)
            {
                Request = new HttpRequestMessage { Content = new StringContent("Japanese Inflation") }
            };
            var response = controller.Put("765");
            response.StatusCode.ShouldEqual(HttpStatusCode.Conflict);
        }

        [Test]
        public void PutUpdatesDeserialisedInstance()
        {
            var repo = new ChartRepo { { "567", new Chart { Title = "Original" } } };
            var controller = new InstanceController<Chart, ChartRepo>(repo)
                                 {
                                     Request = new HttpRequestMessage { Content = new StringContent ("Updated") }
                                 };
            controller.Put("567");
            repo["567"].Title.ShouldEqual("Updated");
        }

        [Test]
        public void DeleteDeletesInstanceById()
        {
            var repo = new ChartRepo { { "678", new Chart { Title = "Deletable" } } };
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Delete("678");
            repo.Count.ShouldEqual(0);
        }

        [Test]
        public void DeleteRespondsWith204()
        {
            var repo = new ChartRepo { { "678", new Chart { Title = "Deletable" } } };
            var controller = new InstanceController<Chart, ChartRepo>(repo) { Request = new HttpRequestMessage() };
            controller.Delete("678").StatusCode.ShouldEqual(HttpStatusCode.NoContent);
        }
    }
}
