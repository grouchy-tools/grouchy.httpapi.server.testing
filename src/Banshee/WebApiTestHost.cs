namespace Banshee
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;
   using System.Text;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Hosting;
   using Microsoft.AspNetCore.TestHost;
   using Microsoft.Extensions.DependencyInjection;

   public class WebApiTestHost
   {
      private readonly Action<IServiceCollection> _configureServices;
      private readonly Action<IApplicationBuilder> _configure;

      public WebApiTestHost(Action<IServiceCollection> configureServices, Action<IApplicationBuilder> configure)
      {
         _configureServices = configureServices;
         _configure = configure;
      }

      public HttpResponseMessage Get(string uri, IDictionary<string, string> headers = null)
      {
         using (var testServer = CreateTestServer())
         {
            return Get(testServer, uri, headers);
         }
      }

      public HttpResponseMessage Get(TestServer testServer, string uri, IDictionary<string, string> headers = null)
      {
         using (var httpClient = testServer.CreateClient())
         {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            AddHeadersToRequest(headers, request);

            return httpClient.SendAsync(request).Result;
         }
      }

      public HttpResponseMessage Head(string uri, IDictionary<string, string> headers = null)
      {
         using (var testServer = CreateTestServer())
         {
            return Head(testServer, uri, headers);
         }
      }

      public HttpResponseMessage Head(TestServer testServer, string uri, IDictionary<string, string> headers = null)
      {
         using (var httpClient = testServer.CreateClient())
         {
            var request = new HttpRequestMessage(HttpMethod.Head, uri);

            AddHeadersToRequest(headers, request);

            return httpClient.SendAsync(request).Result;
         }
      }

      public HttpResponseMessage Post(string uri, string body, IDictionary<string, string> headers = null, string bodyType = "application/json")
      {
         using (var testServer = CreateTestServer())
         {
            return Post(testServer, uri, body, headers, bodyType);
         }
      }

      public HttpResponseMessage Post(TestServer testServer, string uri, string body, IDictionary<string, string> headers = null, string bodyType = "application/json")
      {
         using (var client = testServer.CreateClient())
         {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
               Content = new StringContent(body, Encoding.UTF8, bodyType)
            };

            AddHeadersToRequest(headers, request);

            return client.SendAsync(request).Result;
         }
      }

      public TestServer CreateTestServer()
      {
         var builder = new WebHostBuilder()
            .ConfigureServices(_configureServices)
            .Configure(_configure);

         var testServer = new TestServer(builder);
         return testServer;
      }

      private static void AddHeadersToRequest(IDictionary<string, string> headers, HttpRequestMessage request)
      {
         if (headers == null)
         {
            return;
         }

         foreach (var header in headers)
         {
            request.Headers.Add(header.Key, header.Value);
         }
      }
   }
}
