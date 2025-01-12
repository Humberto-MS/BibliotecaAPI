using AutoMapper;
using BibliotecaAPI.Data;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entities;
using BibliotecaAPI.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers.V2 {
    [ApiController]
    [Route ( "api/autores" )]
    [CabeceraEstaPresente ( "api-version", "2" )]
    [Authorize ( AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin" )]
    public class AutoresController: ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController ( ApplicationDbContext context, IMapper mapper ) {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet ( Name = "ObtenerAutores" )]
        [AllowAnonymous]
        [ServiceFilter ( typeof ( HATEOASAutorFilterAttribute ) )]
        public async Task<ActionResult<List<AutorDTO>>> Get () {
            var autores = await context.Autores.ToListAsync ();
            autores.ForEach ( autor => autor.Nombre = autor.Nombre.ToUpper() );
            return mapper.Map<List<AutorDTO>> ( autores );
        }

        [HttpGet ( "{id:int}", Name = "ObtenerAutor" )] // api/autores/id
        [AllowAnonymous]
        [ServiceFilter ( typeof ( HATEOASAutorFilterAttribute ) )]
        public async Task<ActionResult<AutorDTOConLibros>> Get ( int id ) {
            var autor = await context.Autores
                .Include ( x => x.AutoresLibros )
                .ThenInclude ( x => x.Libro )
                .FirstOrDefaultAsync ( x => x.Id == id );

            if ( autor is null ) return NotFound ();
            var dto = mapper.Map<AutorDTOConLibros> ( autor );
            return dto;
        }

        [HttpGet ( "{nombre:alpha}", Name = "ObtenerAutorPorNombre" )] // api/autores/nombre
        public async Task<IEnumerable<AutorDTO>> Get ( [FromRoute] string nombre ) {
            var autores = await context.Autores.Where ( x => x.Nombre.Contains ( nombre ) ).ToListAsync ();
            return mapper.Map<List<AutorDTO>> ( autores );
        }

        [HttpPost ( Name = "CrearAutor" )]
        public async Task<ActionResult> Post ( [FromBody] AutorCreacionDTO autor_creacion_dto ) {
            var existe_autor_con_el_mismo_nombre = await context.Autores.AnyAsync ( x =>
                x.Nombre == autor_creacion_dto.Nombre );

            if ( existe_autor_con_el_mismo_nombre )
                return BadRequest ( $"Ya existe un autor con el nombre {autor_creacion_dto.Nombre}" );

            var autor = mapper.Map<Autor> ( autor_creacion_dto );

            context.Add ( autor );
            await context.SaveChangesAsync ();
            var autor_dto = mapper.Map<AutorDTO> ( autor );
            return CreatedAtRoute ( "ObtenerAutor", new { id = autor.Id }, autor_dto );
        }

        [HttpPut ( "{id:int}", Name = "ActualizarAutor" )]
        public async Task<ActionResult> Put ( int id, AutorCreacionDTO autor_creacion_dto ) {
            var existe = await context.Autores.AnyAsync ( x => x.Id == id );
            if ( !existe ) return NotFound ();
            var autor = mapper.Map<Autor> ( autor_creacion_dto );
            autor.Id = id;
            context.Update ( autor );
            await context.SaveChangesAsync ();
            return NoContent ();
        }

        [HttpDelete ( "{id:int}", Name = "BorrarAutor" )]
        public async Task<ActionResult> Delete ( int id ) {
            var registros_borrados = await context.Autores.Where ( x => x.Id == id ).ExecuteDeleteAsync ();
            if ( registros_borrados == 0 ) return NotFound ();
            return NoContent ();
        }
    }
}
