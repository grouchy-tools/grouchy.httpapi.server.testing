namespace Banshee
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;
   using System.Text;
#if NET45
   using Microsoft.Owin.Testing;
   using Owin;
#else
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Hosting;
   using Microsoft.AspNetCore.TestHost;
   using Microsoft.Extensions.DependencyInjection;
#endif

   public class WebApiTestHost
   {
#if NET45
      private readonly Action<IAppBuilder> _configure;

      public WebApiTestHost(Action<IAppBuilder> configure)
      {
         _configure = configure;
      }
#else
      private readonly Action<IServiceCollection> _configureServices;
      private readonly Action<IApplicationBuilder> _configure;

      public WebApiTestHost(
         Action<IServiceCollection> configureServices,
         Action<IApplicationBuilder> configure)
      {
         _configureServices = configureServices;
         _configure = configure;
      }
#endif

      public HttpResponseMessage Get(string uri, IDictionary<string, string> headers = null)
      {
         using (var fakeServer = CreateFakeServer())
         {
            return Get(fakeServer, uri, headers);
         }
      }

      public HttpResponseMessage Get(FakeServer fakeServer, string uri, IDictionary<string, string> headers = null)
      {
         using (var httpClient = fakeServer.CreateClient())
         {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            AddHeadersToRequest(headers, request);

            return httpClient.SendAsync(request).Result;
         }
      }

      public HttpResponseMessage Head(string uri, IDictionary<string, string> headers = null)
      {
         using (var fakeServer = CreateFakeServer())
         {
            return Head(fakeServer, uri, headers);
         }
      }

      public HttpResponseMessage Head(FakeServer fakeServer, string uri, IDictionary<string, string> headers = null)
      {
         using (var httpClient = fakeServer.CreateClient())
         {
            var request = new HttpRequestMessage(HttpMethod.Head, uri);

            AddHeadersToRequest(headers, request);

            return httpClient.SendAsync(request).Result;
         }
      }

      public HttpResponseMessage Post(string uri, string body, IDictionary<string, string> headers = null, string bodyType = "application/json")
      {
         using (var fakeServer = CreateFakeServer())
         {
            return Post(fakeServer, uri, body, headers, bodyType);
         }
      }

      public HttpResponseMessage Post(FakeServer fakeServer, string uri, string body, IDictionary<string, string> headers = null, string bodyType = "application/json")
      {
         using (var client = fakeServer.CreateClient())
         {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
               Content = new StringContent(body, Encoding.UTF8, bodyType)
            };

            AddHeadersToRequest(headers, request);

            return client.SendAsync(request).Result;
         }
      }

      public FakeServer CreateFakeServer()
      {
#if NET45
         var fakeServer = new FakeServer(TestServer.Create(_configure));
         return fakeServer;
#else
         var builder = new WebHostBuilder()
            .ConfigureServices(_configureServices)
            .Configure(_configure);

         var fakeServer = new FakeServer(new TestServer(builder));
         return fakeServer;
#endif
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
