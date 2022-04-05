
namespace Common.Models.OutputDtos
{
    public class BookReservationResult
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReviewed { get; set; }
    }
}
