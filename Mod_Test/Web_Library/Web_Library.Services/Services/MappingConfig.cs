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
            
            TypeAdapterConfig<User, UserDto>.NewConfig();

            TypeAdapterConfig<UserDto, User>.NewConfig()
                .Ignore(dest => dest.PasswordHash)
                .Ignore(dest => dest.BorrowedBooks)
                .Ignore(dest => dest.Role)
                .Ignore(dest => dest.FullName)
                .Ignore(dest => dest.Id);

            TypeAdapterConfig<User, UserWithBooksDto>.NewConfig()
                 .Map(dest => dest.BorrowedBooks, src => src.BorrowedBooks.Adapt<List<BookDto>>());
            
            TypeAdapterConfig<LoginDto, User>.NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.FullName)
                .Ignore(dest => dest.Role)
                .Ignore(dest => dest.PasswordHash)
                .Ignore(dest => dest.BorrowedBooks);

            TypeAdapterConfig<RefreshTokenDto, RefreshToken>.NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.User);

            TypeAdapterConfig<RefreshToken, RefreshTokenDto>.NewConfig()
                .Map(dest => dest.RefreshToken, src => src.Token);

        }
    }
}
