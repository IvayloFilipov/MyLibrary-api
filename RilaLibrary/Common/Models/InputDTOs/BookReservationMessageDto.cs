namespace Common.Models.InputDTOs
{
    public class BookReservationMessageDto
    {
        public Guid bookReservationId { get; set; }
        public Guid librarianId { get; set; }
        public string? Message { get; set; }
    }
}
