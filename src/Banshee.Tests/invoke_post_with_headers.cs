﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Banshee;
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
   public class invoke_post_with_headers
   {
      private readonly HttpResponseMessage _response;

      public invoke_post_with_headers()
      {
         var apiHarness = new LightweightWebApiHost(
#if !NET451
            services => { },
#endif
            Configure);

         _response = apiHarness.Post("/ping", "{}", new Dictionary<string, string> { { "post", "true" }, { "xyz", "example.com" } });
      }

      [Test]
      public void should_return_status_code_201()
      {
         _response.StatusCode.ShouldBe(HttpStatusCode.Created);
      }

      [Test]
      public async Task should_return_content_from_headers()
      {
         var content = await _response.Content.ReadAsStringAsync();

         var foo = content.Split(';');
         
         foo.ShouldBe(new[] {"post=true","xyz=example.com","Host=localhost","Content-Type=application/json"," charset=utf-8", ""}, ignoreOrder: true);
      }

#if NET451
      private void Configure(IAppBuilder app)
#else
      private void Configure(IApplicationBuilder app)
#endif
      {
         // Would be better using app.MapWhen, but fails in net451
         app.Run(async context =>
         {
            if (context.Request.Method == "POST" && context.Request.Path.ToString() == "/ping")
            {
               context.Response.StatusCode = (int) HttpStatusCode.Created;

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
