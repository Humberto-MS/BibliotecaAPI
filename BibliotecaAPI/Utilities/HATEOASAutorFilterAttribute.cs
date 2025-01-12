using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entities;
using BibliotecaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BibliotecaAPI.Utilities {
    public class HATEOASAutorFilterAttribute: HATEOASFiltroAttribute {
        private readonly GeneradorEnlaces generador_enlaces;

        public HATEOASAutorFilterAttribute ( GeneradorEnlaces generador_enlaces ) {
            this.generador_enlaces = generador_enlaces;
        }

        public override async Task OnResultExecutionAsync ( ResultExecutingContext context, ResultExecutionDelegate next ) {
            var debe_incluir = DebeIncluirHATEOAS ( context );
            
            if ( !debe_incluir ) {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;
            var autor_dto = resultado.Value as AutorDTO;

            if ( autor_dto is null ) {
                var autores_dto = resultado.Value as List<AutorDTO> ?? 
                    throw new ArgumentException ( "Se esperaba una instancia de AutorDTO o List<AutorDTO>" );

                autores_dto.ForEach ( async autor => await generador_enlaces.GenerarEnlaces ( autor ) );
                resultado.Value = autores_dto;
            } else {
                await generador_enlaces.GenerarEnlaces ( autor_dto );
            }

            await next();
        }
    }
}
