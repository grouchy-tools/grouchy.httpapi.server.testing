namespace Banshee.Tests
{
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Xunit;
   using Banshee;
   using Microsoft.AspNetCore.Http;

   public class handle_request_with_fake_web_api
   {
      [Fact]
      public void simple_get_request()
      {
         using (var fake = new FakePingWebApi())
         using (var httpClient = new HttpClient { BaseAddress = fake.BaseUri })
         {
            var response = httpClient.GetAsync("/ping").Result;
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;

            Assert.Equal("pong", content);
         }
      }

      private class FakePingWebApi : FakeWebApi
      {
         protected override async Task Handler(HttpContext context)
         {
            if (context.Request.Method == "GET" && context.Request.Path == "/ping")
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
