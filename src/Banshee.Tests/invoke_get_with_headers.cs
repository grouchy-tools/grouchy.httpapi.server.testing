namespace Banshee.Tests
{
   using System.Collections.Generic;
   using System.Net;
   using System.Net.Http;
   using System.Runtime.InteropServices;
   using System.Text;
   using Xunit;
   using Banshee;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Http;

   public class invoke_get_with_headers
   {
      private readonly HttpResponseMessage _response;

      public invoke_get_with_headers()
      {
         var apiHarness = new WebApiTestHost(
            services => { },
            Configure);

         _response = apiHarness.Get("/ping", new Dictionary<string, string> { { "abc", "123" }, { "xyz", "example.com" } });
      }

      [Fact]
      public void should_return_status_code_200()
      {
         Assert.Equal(_response.StatusCode, HttpStatusCode.OK);
      }

      [Fact]
      public void should_return_content_from_headers()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

         Assert.Equal(content, "abc=123;xyz=example.com;Host=localhost;");
      }

      private void Configure(IApplicationBuilder app)
      {
         app.Map("/ping", builder =>
         {
            app.Run(async context =>
            {
               context.Response.StatusCode = (int)HttpStatusCode.OK;

               var response = new StringBuilder();
               foreach (var header in context.Request.Headers)
               {
                  response.Append($"{header.Key}={header.Value[0]};");
               }

               await context.Response.WriteAsync(response.ToString());
            });
         });
      }
   }
}
