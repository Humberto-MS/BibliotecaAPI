using BibliotecaAPI.DTOs;

namespace BibliotecaAPI.Utilities {
    public static class IQueryableExtensions {
        public static IQueryable<T> Paginar<T> ( this IQueryable<T> queryable, PaginacionDTO paginacion_dto ) {
            return queryable
                .Skip ( ( paginacion_dto.Pagina - 1 ) * paginacion_dto.RecordsPorPagina )
                .Take ( paginacion_dto.RecordsPorPagina );
        }
    }
}
