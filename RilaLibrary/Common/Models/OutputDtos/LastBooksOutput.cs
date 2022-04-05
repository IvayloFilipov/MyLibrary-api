namespace Common.Models.OutputDtos
{
    public class LastBooksOutput
    {
        public Guid Id { get; set; }

        public string? ImageAddress { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string AllAuthors { get; set; } = string.Empty;
    }
}
