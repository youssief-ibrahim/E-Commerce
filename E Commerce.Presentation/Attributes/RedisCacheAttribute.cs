using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Services_Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerce.Presentation.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int duration;

        public RedisCacheAttribute(int _duration = 5)
        {
            this.duration = _duration;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get Cache Service From DI Container
            var CacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            // Create Cache Key Based On Request Path And Query String
            var Cachekey = CreateCachKey(context.HttpContext.Request);
            // Check If Cached Data Exists
            var CacheValue = await CacheService.GetAsync(Cachekey);
            // If Exists, Return Cached Data and Skip Execution Of EndPoint
            if (CacheValue != null)
            {
                context.Result = new ContentResult()
                {
                    Content = CacheValue,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK,
                };
                return;
            }
            // If Not Exists, Execute The EndPoint and Store The Result In Cache if 200 OK Response
            var executedContext = await next.Invoke();
            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                await CacheService.SetAsync(Cachekey, okObjectResult.Value!, TimeSpan.FromMinutes(duration));
            }

        }
        private string CreateCachKey(HttpRequest request)
        {
            StringBuilder key = new StringBuilder();
            key.Append(request.Path);
                                      // queryparams
            foreach (var item in request.Query.OrderBy(x => x.Key))
                key.Append($"|{item.Key}={item.Value}");

            return key.ToString();
        }
    }
}
