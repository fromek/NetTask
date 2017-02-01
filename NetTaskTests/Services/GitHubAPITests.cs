using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTask.Helpers;
using NetTask.Services;
using NetTaskTests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetTask.Services.Tests
{
    [TestClass()]
    public class GitHubAPITests
    {

        [TestMethod()]
        public void GetAsyncTest()
        {
            var fakeResponseHandler = new FakeResponseHandler();
            HttpResponseMessage repMessage = new HttpResponseMessage(HttpStatusCode.OK);
            repMessage.Headers.Add("X-RateLimit-Remaining", "10");
            fakeResponseHandler.AddFakeResponse(new Uri("http://NetTask.net/test"), repMessage);

            var httpClient = new GitHubAPI(fakeResponseHandler);

            var response1 = httpClient.Get("http://NetTask.net/test1");
            var response2 = httpClient.Get("http://NetTask.net/test");

            Assert.AreEqual(response1.StatusCode, HttpStatusCode.NotFound);
            Assert.AreEqual(response2.StatusCode, HttpStatusCode.OK);

        }

        [TestMethod()]
        [ExpectedException(typeof(CustomException))]
        public void RateLimitTest()
        {
            try
            {
                var fakeResponseHandler = new FakeResponseHandler();
                HttpResponseMessage repMessage = new HttpResponseMessage(HttpStatusCode.OK);
                repMessage.Headers.Add("X-RateLimit-Remaining", "0");
                fakeResponseHandler.AddFakeResponse(new Uri("http://NetTask.net/test"), repMessage);

                var httpClient = new GitHubAPI(fakeResponseHandler);
                var response2 = httpClient.Get("http://NetTask.net/test");
            }catch(Exception ex) when (ex.GetType() != typeof(CustomException))
            {
            }

            Assert.Fail();
        }
    }
}