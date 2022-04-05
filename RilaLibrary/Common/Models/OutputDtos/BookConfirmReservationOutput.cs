
namespace Common.Models.OutputDtos
{
    public class BookConfirmReservationOutput
    {
        public string BookTitle { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable{ get; set; }
        public string UserName { get; set; }
        public string CreatedRequestDate { get; set; }
        public string Message { get; set; }
    }
}
