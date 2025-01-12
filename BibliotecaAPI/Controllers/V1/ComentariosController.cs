using AutoMapper;
using BibliotecaAPI.Data;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entities;
using BibliotecaAPI.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers.V1 {

    [ApiController]
    [Route ( "api/libros/{libroId:int}/comentarios" )]
    public class ComentariosController: ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> user_manager;

        public ComentariosController ( ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> user_manager ) {
            this.context = context;
            this.mapper = mapper;
            this.user_manager = user_manager;
        }

        [HttpGet ( Name = "ObtenerComentariosLibro" )]
        public async Task<ActionResult<List<ComentarioDTO>>> Get ( int libroId, [FromQuery] PaginacionDTO paginacion_dto ) {
            var existe_libro = await context.Libros.AnyAsync ( libro_db => libro_db.Id == libroId );
            if ( !existe_libro ) return NotFound ( $"No existe un libro con el id {libroId}" );
            var queryable = context.Comentarios.Where ( comentario_db => comentario_db.LibroId == libroId ).AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera ( queryable );
            var comentarios = await queryable.OrderBy ( comentario => comentario.Id ).Paginar ( paginacion_dto ).ToListAsync();
            return mapper.Map<List<ComentarioDTO>> ( comentarios );
        }

        [HttpGet ( "{id:int}", Name = "ObtenerComentario" )]
        public async Task<ActionResult<ComentarioDTO>> Get ( int libroId, int id ) {
            var comentario = await context.Comentarios.FirstOrDefaultAsync ( x => x.Id == id );
            if ( comentario is null ) return NotFound ();
            return mapper.Map<ComentarioDTO> ( comentario );
        }

        [HttpPost ( Name = "CrearComentario" )]
        [Authorize ( AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme )]
        public async Task<ActionResult> Post ( int libroId, ComentarioCreacionDTO comentario_creacion_dto ) {
            var email_claim = HttpContext.User.Claims.Where ( claim => claim.Type == "email" ).FirstOrDefault ();
            var email = email_claim!.Value;
            var usuario = await user_manager.FindByEmailAsync ( email );
            var usuario_id = usuario!.Id;
            var existe_libro = await context.Libros.AnyAsync ( libro_db => libro_db.Id == libroId );
            if ( !existe_libro ) return NotFound ( $"No existe un libro con el id {libroId}" );
            var comentario = mapper.Map<Comentario> ( comentario_creacion_dto );
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuario_id;
            context.Add ( comentario );
            await context.SaveChangesAsync ();
            var comentario_dto = mapper.Map<ComentarioDTO> ( comentario );
            return CreatedAtRoute ( "obtenerComentario", new { id = comentario.Id, libroId }, comentario_dto );
        }

        [HttpPut ( "{id:int}", Name = "ActualizarComentario" )]
        public async Task<ActionResult> Put ( int libroId, int id, ComentarioCreacionDTO comentario_creacion_dto ) {
            var existe_libro = await context.Libros.AnyAsync ( libro_db => libro_db.Id == libroId );
            if ( !existe_libro ) return NotFound ( $"No existe un libro con el id {libroId}" );
            var existe_comentario = await context.Comentarios.AnyAsync ( comentario_db => comentario_db.Id == id );
            if ( !existe_comentario ) return NotFound ( $"No existe un comentario con el id {libroId}" );
            var comentario = mapper.Map<Comentario> ( comentario_creacion_dto );
            comentario.Id = id;
            comentario.LibroId = libroId;
            context.Update ( comentario );
            await context.SaveChangesAsync ();
            return NoContent ();
        }
    }
}
