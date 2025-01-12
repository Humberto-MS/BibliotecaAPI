using AutoMapper;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entities;

namespace BibliotecaAPI.Utilities {
    public class AutoMapperProfiles: Profile {
        public AutoMapperProfiles () {
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorDTO> ();
            CreateMap<Autor, AutorDTOConLibros> ()
                .ForMember ( autor_dto => autor_dto.Libros, opciones => opciones.MapFrom ( MapAutorDTOLibros ) );

            CreateMap<LibroCreacionDTO, Libro>()
                .ForMember ( libro => libro.AutoresLibros, opciones => opciones.MapFrom ( MapAutoresLibros ) );
            CreateMap<Libro, LibroDTO> ();
            CreateMap<Libro, LibroDTOConAutores> ()
                .ForMember ( libro_dto => libro_dto.Autores, opciones => opciones.MapFrom ( MapLibroDTOAutores ) );
            CreateMap<Libro, LibroPatchDTO>().ReverseMap();

            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
        }

        private List<LibroDTO> MapAutorDTOLibros ( Autor autor, AutorDTO autor_dto ) {
            var result = new List<LibroDTO> ();
            if ( autor.AutoresLibros is null ) return result;

            foreach ( var autor_libro in autor.AutoresLibros )
                result.Add ( new LibroDTO () {
                    Id = autor_libro.LibroId,
                    Titulo = autor_libro.Libro.Titulo
                } );

            return result;
        }

        private List<AutorDTO> MapLibroDTOAutores ( Libro libro, LibroDTO libro_dto ) {
            var result = new List<AutorDTO> ();
            if ( libro.AutoresLibros is null ) return result;

            foreach ( var autor_libro in libro.AutoresLibros )
                result.Add ( new AutorDTO () { 
                    Id = autor_libro.AutorId, 
                    Nombre = autor_libro.Autor.Nombre 
                } );

            return result;
        }

        private List<AutorLibro> MapAutoresLibros ( LibroCreacionDTO libro_creacion_dto, Libro libro ) {
            var result = new List<AutorLibro>();
            if ( libro_creacion_dto.AutoresIds is null ) return result;

            foreach ( var autor_id in libro_creacion_dto.AutoresIds ) 
                result.Add ( new AutorLibro() { AutorId = autor_id } );

            return result;
        }
    }
}
