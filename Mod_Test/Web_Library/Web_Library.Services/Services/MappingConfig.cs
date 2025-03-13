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
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ISBN, src => src.ISBN)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Genre, src => src.Genre)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.BorrowedAt, src => src.BorrowedAt)
            .Map(dest => dest.ReturnBy, src => src.ReturnBy)
            .Map(dest => dest.ImageUrl, src => src.ImageUrl)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.IsNotified, src => src.IsNotified)
            .Map(dest => dest.AuthorID, src => src.Author.Id)
            .Map(dest => dest.AuthorName, src => $"{src.Author.FirstName} {src.Author.LastName}");

            TypeAdapterConfig<BookDto, Book>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.ISBN, src => src.ISBN)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Genre, src => src.Genre)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.BorrowedAt, src => src.BorrowedAt)
                .Map(dest => dest.ReturnBy, src => src.ReturnBy)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.IsNotified, src => src.IsNotified)
                .Map(dest => dest.AuthorID, src => src.AuthorID);

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
