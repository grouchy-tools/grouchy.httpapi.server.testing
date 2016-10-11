namespace Banshee
{
   using System;
   using System.Net.Http;

#if NET45
   using Microsoft.Owin.Testing;
#else
   using Microsoft.AspNetCore.TestHost;
#endif

   public class FakeServer : IDisposable
   {
      private readonly TestServer _testServer;

      private bool _disposed;

      public FakeServer(TestServer testServer)
      {
         _testServer = testServer;
      }

      ~FakeServer()
      {
         Dispose(false);
      }

#if NET45
      public HttpClient CreateClient()
      {
         return _testServer.HttpClient;
      }
#else
      public HttpClient CreateClient()
      {
         return _testServer.CreateClient();
      }
#endif

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
