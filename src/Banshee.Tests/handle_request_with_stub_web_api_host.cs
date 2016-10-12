namespace Banshee.Tests
{
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Banshee;
   using NUnit.Framework;
#if NET451
   using Microsoft.Owin;
#else
   using Microsoft.AspNetCore.Http;
#endif

   public class handle_request_with_stub_web_api_host
   {
      [Test]
      public void simple_get_request()
      {
         using (var webApi = new PingWebApi())
         using (var httpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var response = httpClient.GetAsync("/ping").Result;
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;

            Assert.AreEqual("pong", content);
         }
      }

      private class PingWebApi : StubWebApiHost
      {
#if NET451
         protected override async Task Handler(IOwinContext context)
#else
         protected override async Task Handler(HttpContext context)
#endif
         {
            if (context.Request.Method == "GET" && context.Request.Path.ToString() == "/ping")
            {
               context.Response.StatusCode = (int)HttpStatusCode.OK;
               await context.Response.WriteAsync("pong");
            }
            else
            {
               await base.Handler(context);
            }
         }
      }
   }
}
