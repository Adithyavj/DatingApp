using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    ///
    /// Custom Middleware for handling exceptions
    ///
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        // ctor takes in logger(logging error info in terminal), env - (to check if we are in dev/prod environment)
        // requestdelegate is a middleware
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        // this happens in the context of an http request
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // shows exception in the terminal
                _logger.LogError(ex, ex.Message);
                // writing this exception to the response/ to the client
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500 error

                // check environment and handle error and get the rep to response variable
                var response = _env.IsDevelopment()
                            ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                            : new ApiException(context.Response.StatusCode, "Internal server error");

                // we will be passing the response as a JSON, so we need to serialize it and return it in camelcase
                // passing in option to make it in camelcase
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                // serializing the response json and providing the options
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}