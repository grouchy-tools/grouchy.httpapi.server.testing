using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

#if NET451
   using Owin;
#else
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Http;
#endif

namespace Grouchy.HttpApi.Server.Testing.Tests
{
   public class invoke_get_with_headers
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var apiHarness = new LightweightHttpApiHost(
#if !NET451
            services => { },
#endif
            Configure);

         _response = await apiHarness.GetAsync("/ping", new Dictionary<string, string> { { "abc", "123" }, { "xyz", "example.com" } });
      }

      [Test]
      public void should_return_status_code_200()
      {
         _response.StatusCode.ShouldBe(HttpStatusCode.OK);
      }

      [Test]
      public async Task should_return_content_from_headers()
      {
         var content = await _response.Content.ReadAsStringAsync();

         var foo = content.Split(';');

         foo.ShouldBe(new[] { "abc=123","xyz=example.com","Host=localhost", ""}, ignoreOrder: true);
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
