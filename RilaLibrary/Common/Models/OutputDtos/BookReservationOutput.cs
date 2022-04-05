
namespace Common.Models.OutputDtos
{
    public class BookReservationOutput
    {
        public Guid Id { get; set; }
        public string BookTitle { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CreatedOn { get; set; }
        public bool IsApproved { get; set; }
    }
}
