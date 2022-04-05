using Microsoft.AspNetCore.Http;

namespace Common.Models.InputDTOs
{
    public class AddBookDto
    {
        public string BookTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? BookCover { get; set; }
        public bool Availability { get; set; }
        public int TotalQuantity { get; set; }
        public ICollection<string>? Genres { get; set; }
        public ICollection<string>? BookAuthors { get; set; }
        public bool? DeleteCover { get; set; }
    }
}
