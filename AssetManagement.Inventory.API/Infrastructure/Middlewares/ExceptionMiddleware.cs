using System.Net;
using System.Text.Json;

namespace AssetManagement.Inventory.API.Infrastructure.Middlewares
{
    
    using global::AssetManagement.Inventory.API.Exceptions;
    using Microsoft.IdentityModel.Tokens;
    using System.Net;
    using System.Text.Json;

    namespace AssetManagement.Inventory.API.Middlewares
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
                catch (UnauthorizedAccessException ex)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = 401,
                        error = ex.Message
                    });
                }
                catch (SecurityTokenException)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = 401,
                        error = "Token inválido ou expirado"
                    });
                }
                catch (AppException ex)
                {
                    context.Response.StatusCode = ex.StatusCode;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = ex.StatusCode,
                        error = ex.Message
                    });
                }
                catch (Exception)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = 500,
                        error = "Erro interno no servidor"
                    });
                }
            }

        }
    }

}
