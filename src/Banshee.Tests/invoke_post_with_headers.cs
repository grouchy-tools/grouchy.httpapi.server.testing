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

   public class invoke_post_with_headers
   {
      private readonly HttpResponseMessage _response;

      public invoke_post_with_headers()
      {
         var apiHarness = new WebApiTestHost(
            services => { },
            Configure);

         _response = apiHarness.Post("/ping", "{}", new Dictionary<string, string> { { "post", "true" }, { "xyz", "example.com" } });
      }

      [Fact]
      public void should_return_status_code_201()
      {
         Assert.Equal(_response.StatusCode, HttpStatusCode.Created);
      }

      [Fact]
      public void should_return_content_from_headers()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

         Assert.Equal(content, "post=true;xyz=example.com;Host=localhost;Content-Type=application/json; charset=utf-8;");
      }

      private void Configure(IApplicationBuilder app)
      {
         app.MapWhen(context => context.Request.Path == "/ping" && context.Request.Method == "POST", builder =>
         {
            app.Run(async context =>
            {
               context.Response.StatusCode = (int)HttpStatusCode.Created;

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
