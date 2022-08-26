using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Abp;
using Abp.UI;

using Microsoft.AspNetCore.Http;
using NUglify.JavaScript;

namespace eryaz.Web.Host.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception exception)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int) HttpStatusCode.BadRequest;

                var result = JsonSerializer.Serialize(new { message = exception?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}

