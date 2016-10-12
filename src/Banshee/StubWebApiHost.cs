namespace Banshee
{
   using System;
   using System.Net;
   using System.Net.Sockets;
   using System.Threading.Tasks;
#if NET451
   using Microsoft.Owin;
   using Microsoft.Owin.Hosting;
   using Owin;
#else
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Hosting;
   using Microsoft.AspNetCore.Http;
#endif

   public class StubWebApiHost : IDisposable
   {
      private readonly Uri _baseUri;
      private readonly IDisposable _server;

      private bool _disposed;

      public StubWebApiHost(int port = 0)
      {
         if (port == 0)
         {
            port = GetAvailablePort();
         }

         _baseUri = new Uri($"http://localhost:{port}/");
         _server = CreateServer();
      }

      ~StubWebApiHost()
      {
         Dispose(false);
      }

      public Uri BaseUri => _baseUri;

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      private static int GetAvailablePort()
      {
         var tcp = new TcpListener(IPAddress.Loopback, 0);

         tcp.Start();
         var port = ((IPEndPoint)tcp.LocalEndpoint).Port;
         tcp.Stop();

         return port;
      }

      private IDisposable CreateServer()
      {
#if NET451
         var server = WebApp.Start(_baseUri.ToString(), app => { app.Run(Handler); });
         return server;
#else
         var builder = new WebHostBuilder()
            .UseKestrel()
            .UseUrls(_baseUri.ToString())
            .Configure(app =>
            {
               app.Run(Handler);
            });

         var server = builder.Build();
         server.Start();

         return server;
#endif
      }

#if NET451
      protected virtual Task Handler(IOwinContext context)
      {
         context.Response.StatusCode = (int)HttpStatusCode.NotFound;
         return Task.FromResult(0);
      }
#else
      protected virtual Task Handler(HttpContext context)
      {
         context.Response.StatusCode = (int)HttpStatusCode.NotFound;
         return Task.CompletedTask;
      }
#endif

      protected virtual void Dispose(bool disposing)
      {
         if (_disposed)
         {
            return;
         }

         if (disposing)
         {
            _server.Dispose();
         }

         _disposed = true;
      }
   }
}
