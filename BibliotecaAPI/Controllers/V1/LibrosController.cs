using AutoMapper;
using Azure;
using BibliotecaAPI.Data;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entities;
using BibliotecaAPI.Utilities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers.V1 {
    [ApiController]
    [Route ( "api/libros" )]
    public class LibrosController: ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController ( ApplicationDbContext context, IMapper mapper ) {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet ( Name = "ObtenerLibros" )]
        public async Task<IEnumerable<LibroDTO>> Get ( [FromQuery] PaginacionDTO paginacion_dto ) {
            var queryable = context.Libros.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera ( queryable );
            var libros = await queryable.OrderBy ( libro => libro.Titulo ).Paginar ( paginacion_dto ).ToListAsync();
            return mapper.Map<List<LibroDTO>> ( libros );
        }

        [HttpGet ( "{id:int}", Name = "ObtenerLibro" )] // api/libros/id
        public async Task<ActionResult<LibroDTOConAutores>> Get ( int id ) {
            var libro = await context.Libros
                .Include ( libro_db => libro_db.AutoresLibros )
                .ThenInclude ( autor_libro_db => autor_libro_db.Autor )
                .FirstOrDefaultAsync ( x => x.Id == id );

            if ( libro is null ) return NotFound ();
            libro.AutoresLibros = libro.AutoresLibros.OrderBy ( x => x.Orden ).ToList ();
            return mapper.Map<LibroDTOConAutores> ( libro );
        }

        [HttpPost ( Name = "CrearLibro" )]
        public async Task<ActionResult> Post ( LibroCreacionDTO libro_creacion_dto ) {
            if ( libro_creacion_dto.AutoresIds is null ) return BadRequest ( "No se puede crear un libro sin autores" );

            var autores_ids = await context.Autores
                .Where ( autorBD => libro_creacion_dto.AutoresIds.Contains ( autorBD.Id ) ).Select ( x => x.Id ).ToListAsync ();

            if ( libro_creacion_dto.AutoresIds.Count != autores_ids.Count )
                return BadRequest ( "No existe uno o más de los autores enviados" );

            var libro = mapper.Map<Libro> ( libro_creacion_dto );
            AsignarOrdenAutores ( libro );
            context.Add ( libro );
            await context.SaveChangesAsync ();
            var libro_dto = mapper.Map<LibroDTO> ( libro );
            return CreatedAtRoute ( "obtenerLibro", new { id = libro.Id }, libro_dto );
        }

        [HttpPut ( "{id:int}", Name = "ActualizarLibro" )]
        public async Task<ActionResult> Put ( int id, LibroCreacionDTO libro_creacion_dto ) {
            var libro_db = await context.Libros
                .Include ( x => x.AutoresLibros )
                .FirstOrDefaultAsync ( x => x.Id == id );

            if ( libro_db is null ) return NotFound ();
            libro_db = mapper.Map ( libro_creacion_dto, libro_db );
            AsignarOrdenAutores ( libro_db );
            await context.SaveChangesAsync ();
            return NoContent ();
        }

        [HttpPatch ( "{id:int}", Name = "PatchLibro" )]
        public async Task<ActionResult> Patch ( int id, JsonPatchDocument<LibroPatchDTO> patch_document ) {
            if ( patch_document is null ) return BadRequest ();
            var libro_db = await context.Libros.FirstOrDefaultAsync ( x => x.Id == id );
            if ( libro_db is null ) return NotFound ();
            var libro_dto = mapper.Map<LibroPatchDTO> ( libro_db );
            patch_document.ApplyTo ( libro_dto, ModelState );
            var es_valido = TryValidateModel ( libro_dto );
            if ( !es_valido ) return BadRequest ( ModelState );
            mapper.Map ( libro_dto, libro_db );
            await context.SaveChangesAsync ();
            return NoContent ();
        }

        [HttpDelete ( "{id:int}", Name = "BorrarLibro" )]
        public async Task<ActionResult> Delete ( int id ) {
            var registros_borrados = await context.Libros.Where ( x => x.Id == id ).ExecuteDeleteAsync ();
            if ( registros_borrados == 0 ) return NotFound ();
            return NoContent ();
        }

        private void AsignarOrdenAutores ( Libro libro ) {
            if ( libro.AutoresLibros is not null )
                for ( int i = 0; i < libro.AutoresLibros.Count; i++ )
                    libro.AutoresLibros [ i ].Orden = i;
        }
    }
}
