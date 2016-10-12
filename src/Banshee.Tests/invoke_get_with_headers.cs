namespace Banshee.Tests
{
   using System.Collections.Generic;
   using System.Net;
   using System.Net.Http;
   using System.Text;
   using Banshee;
   using NUnit.Framework;
   using Shouldly;
#if NET451
   using Owin;
#else
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Http;
#endif

   public class invoke_get_with_headers
   {
      private readonly HttpResponseMessage _response;

      public invoke_get_with_headers()
      {
         var apiHarness = new LightweightWebApiHost(
#if !NET451
            services => { },
#endif
            Configure);

         _response = apiHarness.Get("/ping", new Dictionary<string, string> { { "abc", "123" }, { "xyz", "example.com" } });
      }

      [Test]
      public void should_return_status_code_200()
      {
         _response.StatusCode.ShouldBe(HttpStatusCode.OK);
      }

      [Test]
      public void should_return_content_from_headers()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

         content.ShouldBe("abc=123;xyz=example.com;Host=localhost;");
      }

#if NET451
      private void Configure(IAppBuilder app)
#else
      private void Configure(IApplicationBuilder app)
#endif
      {
         // Would be better using app.Map, but fails in net451
         app.Run(async context =>
         {
            if (context.Request.Method == "GET" && context.Request.Path.ToString() == "/ping")
            {
               context.Response.StatusCode = (int) HttpStatusCode.OK;

               var response = new StringBuilder();
               foreach (var header in context.Request.Headers)
               {
                  response.Append($"{header.Key}={header.Value[0]};");
               }

               await context.Response.WriteAsync(response.ToString());
            }
         });
      }
   }
}
