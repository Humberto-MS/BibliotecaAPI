using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace BibliotecaAPI.Services {
    public class GeneradorEnlaces {
        private readonly IAuthorizationService authorization_service;
        private readonly IHttpContextAccessor http_context_accessor;
        private readonly IActionContextAccessor action_context_accessor;

        public GeneradorEnlaces ( 
            IAuthorizationService authorization_service, 
            IHttpContextAccessor http_context_accessor,
            IActionContextAccessor action_context_accessor
        ) {
            this.authorization_service = authorization_service;
            this.http_context_accessor = http_context_accessor;
            this.action_context_accessor = action_context_accessor;
        }

        private IUrlHelper ConstruirURLHelper() {
            var factoria = http_context_accessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper ( action_context_accessor.ActionContext );
        }

        private async Task<bool> EsAdmin() {
            var http_context = http_context_accessor.HttpContext;
            var resultado = await authorization_service.AuthorizeAsync ( http_context.User, "esAdmin" );
            return resultado.Succeeded;
        }

        public async Task GenerarEnlaces ( AutorDTO autor_dto ) {
            var es_admin = await EsAdmin();
            var Url = ConstruirURLHelper();

            autor_dto.Enlaces.Add ( new DatoHATEOAS (
                enlace: Url.Link ( "ObtenerAutor", new { id = autor_dto.Id } ),
                descripcion: "self",
                metodo: "GET"
            ) );

            if ( es_admin ) {
                autor_dto.Enlaces.Add ( new DatoHATEOAS (
                    enlace: Url.Link ( "ActualizarAutor", new { id = autor_dto.Id } ),
                    descripcion: "autor-actualizar",
                    metodo: "PUT"
                ) );
                autor_dto.Enlaces.Add ( new DatoHATEOAS (
                    enlace: Url.Link ( "BorrarAutor", new { id = autor_dto.Id } ),
                    descripcion: "autor-borrar",
                    metodo: "DELETE"
                ) );
            }
        }
    }
}
