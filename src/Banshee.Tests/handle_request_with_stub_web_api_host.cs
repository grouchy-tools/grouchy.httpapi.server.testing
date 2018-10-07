using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

#if NET451
   using Microsoft.Owin;
#else
   using Microsoft.AspNetCore.Http;
#endif

namespace Banshee.Tests
{
   public class handle_request_with_stub_web_api_host
   {
      [Test]
      public async Task simple_get_request()
      {
         using (var webApi = new PingWebApi())
         using (var httpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var response = await httpClient.GetAsync("/ping");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            content.ShouldBe("pong");
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
