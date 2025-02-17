using System.ComponentModel.DataAnnotations;

namespace Web_Library.API.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(13)]
        public string Name { get; set; }
    }
}
