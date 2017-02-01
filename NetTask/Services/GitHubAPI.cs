using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using NetTask.Helpers;

namespace NetTask.Services
{
    public class GitHubAPI : IHttpHandler, IDisposable
    {
        public GitHubAPI() { }
        public GitHubAPI(HttpMessageHandler handler)
       {
            _httpMessageHandler = handler;
        }

        private HttpMessageHandler _httpMessageHandler;
        private HttpClient _client { get; set; }
        private int _rateLimit;
        private int _rateLimitRemaining;

        public int RateLimitReaming
        {
            get { return _rateLimitRemaining; }
            private set { _rateLimitRemaining = value; }
        }

        public int RateLimit
        {
            get { return _rateLimit; }
            private set { _rateLimit = value; }
        }

        public HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = _httpMessageHandler ==null? new HttpClient(): new HttpClient(_httpMessageHandler);
                    _client.BaseAddress = new Uri(@"https://api.github.com");
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _client.DefaultRequestHeaders.UserAgent.ParseAdd("NetTask");
                }
                return _client;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string Uri)
        {
                var result = await Client.GetAsync(Uri);
                CheckRateLimit(result.Headers);
                return result;
        }


        public async Task<dynamic> GetFromResponse(HttpResponseMessage response, Type _type)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            var serializer = new DataContractJsonSerializer(_type);
            return serializer.ReadObject(stream);
        }

        public async Task<HttpResponseMessage> PostAsync(string Uri, dynamic obj)
        {
            HttpContent content = null;
            if (obj != null)
            {
                string json = JsonConvert.SerializeObject(obj);
                content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }
            var result = await Client.PostAsync(Uri, content);
            CheckRateLimit(result.Headers);
            return result;
        }


        public void SetToken(string tok)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tok);
        }

        public bool CheckRateLimit(HttpResponseHeaders headers)
        {
            if (headers != null)
            {
                IEnumerable<string> rateLimit;
                IEnumerable<string> rateRemaining;
                headers.TryGetValues("X-RateLimit-Limit", out rateLimit);
                headers.TryGetValues("X-RateLimit-Remaining", out rateRemaining);

                if (rateLimit != null && rateLimit.Count() > 0) RateLimit = Convert.ToInt32(rateLimit.FirstOrDefault());
                if (rateRemaining != null && rateRemaining.Count() > 0) RateLimitReaming = Convert.ToInt32(rateRemaining.FirstOrDefault());

                if (rateRemaining != null && RateLimitReaming <= 0)
                    throw new CustomException($"API rate limit {RateLimit} has been reached.", null);
            }

            return RateLimitReaming > 0;
        }

        public HttpResponseMessage Get(string url)
        {
            try
            {
                var retToken = GetAsync(url);
                return retToken.Result;
            }
            catch(AggregateException aEx)
            {
                if (aEx.InnerException != null && aEx.InnerException.GetType() == typeof(CustomException))
                    throw aEx.InnerException;
                else
                    throw aEx;
            }

        }

        public HttpResponseMessage Post(string url, dynamic obj)
        {
            return PostAsync(url, obj).Result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null) _client.Dispose();
            }
        }
    }
}
