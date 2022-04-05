namespace Common.Models.OutputDtos
{
    public class BookOutput
    {
        public string Title { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public string? ImageAddress { get; set; }
        public bool IsAvailable { get; set; }
        public int CurrentQuantity { get; set; }
        public int TotalQuantity { get; set; }
        public string AllAuthors { get; set; } = string.Empty;
        public string AllGenres { get; set; } = string.Empty;
    }
}
