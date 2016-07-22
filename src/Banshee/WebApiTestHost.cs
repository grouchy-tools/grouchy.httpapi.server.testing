namespace Banshee
{
   using System;
   using System.Net.Http;
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

      public HttpResponseMessage Get(string uri)
      {
         using (var server = CreateTestServer())
         using (var httpClient = server.CreateClient())
         {
            return httpClient.GetAsync(uri).Result;
         }
      }

      public HttpResponseMessage Post(string uri, string body)
      {
         using (var server = CreateTestServer())
         using (var httpClient = server.CreateClient())
         {
            return httpClient.PostAsync(uri, new StringContent(body)).Result;
         }
      }

      private TestServer CreateTestServer()
      {
         var builder = new WebHostBuilder()
            .ConfigureServices(_configureServices)
            .Configure(_configure);

         var testServer = new TestServer(builder);
         return testServer;
      }
   }
}
