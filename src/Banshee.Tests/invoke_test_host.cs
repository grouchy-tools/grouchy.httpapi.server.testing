using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

#if NET451
   using Owin;
#else
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Http;
#endif

namespace Banshee.Tests
{
   public class invoke_test_host
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var apiHarness = new LightweightWebApiHost(
#if !NET451
            services => { },
#endif
            Configure);

         _response = await apiHarness.GetAsync("/ping");
      }

      [Test]
      public void should_return_status_code_200()
      {
         _response.StatusCode.ShouldBe(HttpStatusCode.OK);
      }

      [Test]
      public async Task should_return_content_pong()
      {
         var content = await _response.Content.ReadAsStringAsync();

         content.ShouldBe("!pong!");
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
            if (context.Request.Path.ToString() == "/ping")
            {
               context.Response.StatusCode = (int)HttpStatusCode.OK;
               await context.Response.WriteAsync("!pong!");
            }
         });
      }
   }
}
