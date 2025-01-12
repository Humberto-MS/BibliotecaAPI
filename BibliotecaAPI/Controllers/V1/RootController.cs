using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers.V1 {

    [ApiController]
    [Route ( "api" )]
    [Authorize ( AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme )]
    public class RootController: ControllerBase {
        private readonly IAuthorizationService authorization_service;

        public RootController ( IAuthorizationService authorization_service ) {
            this.authorization_service = authorization_service;
        }

        [HttpGet ( Name = "ObtenerRoot" )]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get () {
            var datos_hateoas = new List<DatoHATEOAS> ();
            var es_admin = await authorization_service.AuthorizeAsync ( User, "esAdmin" );

            datos_hateoas.Add ( new DatoHATEOAS (
                enlace: Url.Link ( "ObtenerRoot", new { } ),
                descripcion: "self",
                metodo: "GET"
            ) );

            datos_hateoas.Add ( new DatoHATEOAS (
                enlace: Url.Link ( "ObtenerAutores", new { } ),
                descripcion: "autores",
                metodo: "GET"
            ) );

            datos_hateoas.Add ( new DatoHATEOAS (
                enlace: Url.Link ( "ObtenerLibros", new { } ),
                descripcion: "libros",
                metodo: "GET"
            ) );

            if ( es_admin.Succeeded ) {
                datos_hateoas.Add ( new DatoHATEOAS (
                    enlace: Url.Link ( "CrearAutor", new { } ),
                    descripcion: "autor-crear",
                    metodo: "POST"
                ) );

                datos_hateoas.Add ( new DatoHATEOAS (
                    enlace: Url.Link ( "CrearLibro", new { } ),
                    descripcion: "libro-crear",
                    metodo: "POST"
                ) );
            }

            return datos_hateoas;
        }
    }
}
