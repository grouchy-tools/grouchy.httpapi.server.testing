//namespace Banshee.Tests
//{
//   using System.Collections.Generic;
//   using System.Net;
//   using System.Net.Http;
//   using System.Runtime.InteropServices;
//   using System.Text;
//   using Xunit;
//   using Banshee;
//#if NET451
//   using Owin;
//#else
//   using Microsoft.AspNetCore.Builder;
//   using Microsoft.AspNetCore.Http;
//#endif

//   public class invoke_post_with_headers
//   {
//      private readonly HttpResponseMessage _response;

//      public invoke_post_with_headers()
//      {
//         var apiHarness = new WebApiTestHost(
//#if !NET451
//            services => { },
//#endif
//            Configure);

//         _response = apiHarness.Post("/ping", "{}", new Dictionary<string, string> { { "post", "true" }, { "xyz", "example.com" } });
//      }

//      [Fact]
//      public void should_return_status_code_201()
//      {
//         Assert.Equal(_response.StatusCode, HttpStatusCode.Created);
//      }

//      [Fact]
//      public void should_return_content_from_headers()
//      {
//         var content = _response.Content.ReadAsStringAsync().Result;

//         Assert.Equal(content, "post=true;xyz=example.com;Host=localhost;Content-Type=application/json; charset=utf-8;");
//      }

//#if NET451
//      private void Configure(IAppBuilder app)
//#else
//      private void Configure(IApplicationBuilder app)
//#endif
//      {
//         app.MapWhen(context => context.Request.Path.ToString() == "/ping" && context.Request.Method == "POST", builder =>
//         {
//            app.Run(async context =>
//            {
//               context.Response.StatusCode = (int)HttpStatusCode.Created;

//               var response = new StringBuilder();
//               foreach (var header in context.Request.Headers)
//               {
//                  response.Append($"{header.Key}={header.Value[0]};");
//               }

//               await context.Response.WriteAsync(response.ToString());
//            });
//         });
//      }
//   }
//}
