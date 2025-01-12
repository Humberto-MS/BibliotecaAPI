using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Utilities {
    public static class HttpContextExtensions {
        public async static Task InsertarParametrosPaginacionEnCabecera<T> ( this HttpContext http_context, IQueryable<T> queryable ) {
            ArgumentNullException.ThrowIfNull ( http_context );
            double cantidad = await queryable.CountAsync();
            http_context.Response.Headers.Append ( "cantidadTotalRegistros", cantidad.ToString() );
        }
    }
}
