using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#if NET451
   using Microsoft.Owin.Testing;
   using Owin;
#else
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Hosting;
   using Microsoft.AspNetCore.TestHost;
   using Microsoft.Extensions.DependencyInjection;
#endif

namespace Grouchy.HttpApi.Server.Testing
{
   public class LightweightHttpApiHost
   {
#if NET451
      private readonly Action<IAppBuilder> _configure;

      public LightweightWebApiHost(Action<IAppBuilder> configure)
      {
         _configure = configure;
      }
#else
      private readonly Action<IServiceCollection> _configureServices;
      private readonly Action<IApplicationBuilder> _configure;

      public LightweightHttpApiHost(
         Action<IServiceCollection> configureServices,
         Action<IApplicationBuilder> configure)
      {
         _configureServices = configureServices;
         _configure = configure;
      }
#endif

      public async Task<HttpResponseMessage> GetAsync(string uri, IDictionary<string, string> headers = null)
      {
         using (var fakeServer = CreateServer())
         {
            return await GetAsync(fakeServer, uri, headers);
         }
      }

      public async Task<HttpResponseMessage> GetAsync(Server server, string uri, IDictionary<string, string> headers = null)
      {
         using (var httpClient = server.CreateClient())
         {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            AddHeadersToRequest(headers, request);

            return await httpClient.SendAsync(request);
         }
      }

      public async Task<HttpResponseMessage> HeadAsync(string uri, IDictionary<string, string> headers = null)
      {
         using (var fakeServer = CreateServer())
         {
            return await HeadAsync(fakeServer, uri, headers);
         }
      }

      public async Task<HttpResponseMessage> HeadAsync(Server server, string uri, IDictionary<string, string> headers = null)
      {
         using (var httpClient = server.CreateClient())
         {
            var request = new HttpRequestMessage(HttpMethod.Head, uri);

            AddHeadersToRequest(headers, request);

            return await httpClient.SendAsync(request);
         }
      }

      public async Task<HttpResponseMessage> PostAsync(string uri, string body, IDictionary<string, string> headers = null, string bodyType = "application/json")
      {
         using (var fakeServer = CreateServer())
         {
            return await PostAsync(fakeServer, uri, body, headers, bodyType);
         }
      }

      public async Task<HttpResponseMessage> PostAsync(Server server, string uri, string body, IDictionary<string, string> headers = null, string bodyType = "application/json")
      {
         using (var client = server.CreateClient())
         {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
               Content = new StringContent(body, Encoding.UTF8, bodyType)
            };

            AddHeadersToRequest(headers, request);

            return await client.SendAsync(request);
         }
      }

      public Server CreateServer()
      {
#if NET451
         var server = new Server(TestServer.Create(_configure));
         return server;
#else
         var builder = new WebHostBuilder()
            .ConfigureServices(_configureServices)
            .Configure(_configure);

         var server = new Server(new TestServer(builder));
         return server;
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

      public class Server : IDisposable
      {
         private readonly TestServer _testServer;

         private bool _disposed;

         internal Server(TestServer testServer)
         {
            _testServer = testServer;
         }

         ~Server()
         {
            Dispose(false);
         }

         public HttpClient CreateClient()
         {
#if NET451
            return _testServer.HttpClient;
#else
            return _testServer.CreateClient();
#endif
         }

         public void Dispose()
         {
            Dispose(true);
            GC.SuppressFinalize(this);
         }

         protected virtual void Dispose(bool disposing)
         {
            if (_disposed)
            {
               return;
            }

            if (disposing)
            {
               _testServer.Dispose();
            }

            _disposed = true;
         }
      }
   }
}
