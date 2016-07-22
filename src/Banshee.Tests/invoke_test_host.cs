namespace Banshee.Tests
{
   using System.Net;
   using System.Net.Http;
   using Xunit;
   using Banshee;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Http;

   public class invoke_test_host
   {
      private readonly HttpResponseMessage _response;

      public invoke_test_host()
      {
         var apiHarness = new WebApiTestHost(
            services => { },
            Configure);

         _response = apiHarness.Get("/ping");
      }

      [Fact]
      public void should_return_status_code_200()
      {
         Assert.Equal(_response.StatusCode, HttpStatusCode.OK);
      }

      [Fact]
      public void should_return_content_pong()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

         Assert.Equal(content, "!pong!");
      }

      private void Configure(IApplicationBuilder app)
      {
         app.Map("/ping", builder =>
         {
            app.Run(async context =>
            {
               context.Response.StatusCode = (int) HttpStatusCode.OK;
               await context.Response.WriteAsync("!pong!");
            });
         });
      }
   }
}
