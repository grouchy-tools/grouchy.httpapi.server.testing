//namespace Banshee.Tests
//{
//   using System.Net;
//   using System.Net.Http;
//   using Xunit;
//   using Banshee;

//#if NET451
//   using Owin;
//#else
//   using Microsoft.AspNetCore.Builder;
//   using Microsoft.AspNetCore.Http;
//#endif

//   public class invoke_test_host
//   {
//      private readonly HttpResponseMessage _response;

//      public invoke_test_host()
//      {
//         var apiHarness = new WebApiTestHost(
//#if !NET451
//            services => { },
//#endif
//            Configure);

//         _response = apiHarness.Get("/ping");
//      }

//      [Fact]
//      public void should_return_status_code_200()
//      {
//         Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
//      }

//      [Fact]
//      public void should_return_content_pong()
//      {
//         var content = _response.Content.ReadAsStringAsync().Result;

//         Assert.Equal("!pong!", content);
//      }

//#if NET451
//      private void Configure(IAppBuilder app)
//#else
//      private void Configure(IApplicationBuilder app)
//#endif
//      {
//         app.Map("/ping", builder =>
//         {
//            app.Run(async context =>
//            {
//               context.Response.StatusCode = (int) HttpStatusCode.OK;
//               await context.Response.WriteAsync("!pong!");
//            });
//         });
//      }
//   }
//}
