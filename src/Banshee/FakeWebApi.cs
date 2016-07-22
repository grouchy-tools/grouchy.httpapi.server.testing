namespace Banshee
{
   using System;
   using System.Net;
   using System.Net.Sockets;
   using System.Threading.Tasks;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Hosting;
   using Microsoft.AspNetCore.Http;

   public class FakeWebApi : IDisposable
   {
      private readonly Uri _baseUri;
      private readonly IWebHost _testServer;

      private bool _disposed;

      public FakeWebApi(int port = 0)
      {
         if (port == 0)
         {
            port = GetAvailablePort();
         }

         _baseUri = new Uri($"http://localhost:{port}/");
         _testServer = CreateTestServer();
         _testServer.Start();
      }

      ~FakeWebApi()
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

      private IWebHost CreateTestServer()
      {
         var builder = new WebHostBuilder()
            .UseKestrel()
            .UseUrls(_baseUri.ToString())
            .Configure(app =>
            {
               app.Run(Handler);
            });

         var testServer = builder.Build();
         return testServer;
      }

      protected virtual Task Handler(HttpContext context)
      {
         context.Response.StatusCode = (int)HttpStatusCode.NotFound;
         return Task.CompletedTask;
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
