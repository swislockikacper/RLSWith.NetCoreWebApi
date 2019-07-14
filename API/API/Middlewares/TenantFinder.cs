using API.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Middlewares
{
    public class TenantFinder
    {
        private readonly RequestDelegate next;
        private readonly DatabaseContext dbContext;

        public TenantFinder(RequestDelegate next, DatabaseContext dbContext)
        {
            this.next = next;
            this.dbContext = dbContext;
        }

        public async Task Invoke(HttpContext context)
        {
            var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();

            if (string.IsNullOrEmpty(apiKey))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid API key");
                return;
            }

            Guid apiKeyGuid;

            if (!Guid.TryParse(apiKey, out apiKeyGuid))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid API key");
                return;
            }

            var tenant = dbContext.Tenants
                .Where(t => t.ApiKey == apiKeyGuid)
                .FirstOrDefault();

            if (tenant == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API key");
                return;
            }
            else
            {
                context.Items["TENANT"] = tenant;
            }

            await next.Invoke(context);
        }
    }
}
