using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_Library.DTOs
{
    public class UserWithBooksDto : UserDto
    {
        public List<BookDto> BorrowedBooks { get; set; } = new();
    }
}
