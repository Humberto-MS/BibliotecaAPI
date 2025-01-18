using Microsoft.AspNetCore.Builder;

namespace BibliotecaAPI.Middlewares {
    public static class LoguearRespuestaHttpMiddlewareExtensions {
        public static IApplicationBuilder UseLoguearRespuestaHttp ( this IApplicationBuilder app ) {
            return app.UseMiddleware<LoguearRespuestaHttpMiddleware>();
        }
    }

    public class LoguearRespuestaHttpMiddleware {
        private readonly RequestDelegate next;

        public LoguearRespuestaHttpMiddleware ( RequestDelegate siguiente ) {
            this.next = siguiente;
        }

        public async Task InvokeAsync ( HttpContext contexto ) {
            using ( var ms = new MemoryStream () ) {
                var logger = contexto.RequestServices.GetRequiredService<ILogger<Program>> ();
                logger.LogInformation ( $"Petición: {contexto.Request.Method} {contexto.Request.Path}" );
                await next.Invoke ( contexto ); 
                logger.LogInformation ( $"Respuesta: {contexto.Response.StatusCode}" );
            }
        }
    }
}
