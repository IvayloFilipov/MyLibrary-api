namespace Common.Models
{
    public class Book
    {
        public int Id { get; set; } 
        public string? Name { get; set; }
        public string? AuthorName { get; set; }
        public int ReaderId { get; set; }
    }
}
