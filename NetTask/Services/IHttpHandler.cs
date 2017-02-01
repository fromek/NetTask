using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetTask.Services
{
    interface IHttpHandler
    {
        HttpResponseMessage Get(string url);
        HttpResponseMessage Post(string Uri, dynamic obj);
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostAsync(string Uri, dynamic obj);
        Task<dynamic> GetFromResponse(HttpResponseMessage response, Type _type);
    }
}
