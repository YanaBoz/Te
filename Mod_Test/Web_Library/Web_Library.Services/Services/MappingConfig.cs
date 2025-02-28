using Mapster;
using Web_Library.DTOs;
using Web_Library.Models;

namespace Web_Library.Services
{
    public class MappingConfig
    {
        public static void ConfigureMappings()
        {
            TypeAdapterConfig<Author, AuthorDto>.NewConfig()
                .Map(dest => dest.Books, src => src.Books.Adapt<List<BookDto>>());

            TypeAdapterConfig<AuthorDto, Author>.NewConfig()
                .Map(dest => dest.Books, src => src.Books.Adapt<List<Book>>());

            TypeAdapterConfig<Book, BookDto>.NewConfig()
                .Map(dest => dest.Genre, src => src.GenreNavigation.Name);

            TypeAdapterConfig<BookDto, Book>.NewConfig()
                .Map(dest => dest.GenreNavigation.Name, src => src.Genre);
        }
    }
}
