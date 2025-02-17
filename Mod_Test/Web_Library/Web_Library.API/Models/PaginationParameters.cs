namespace Web_Library.API.Models
{
    public class PaginationParameters
    {
        public int PageNumber { get; set; } = 1; // Номер страницы по умолчанию
        public int PageSize { get; set; } = 10;  // Размер страницы по умолчанию
    }
}
