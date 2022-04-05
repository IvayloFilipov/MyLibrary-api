using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Models.InputDTOs;
using Services.Interfaces;

using static Common.GlobalConstants;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BookReservationsController : ControllerBase
    {
        private readonly IBookReservationService bookReservationService;

        public BookReservationsController(IBookReservationService bookReservationService)
        {
            this.bookReservationService = bookReservationService;
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}, {READER_ROLE_NAME}")]
        [HttpPost]
        [Route("add-reservation")]
        public async Task<IActionResult> AddReservationBook([FromBody] BookReservationDto input)
        {
            try
            {
                var result = await bookReservationService.AddBookReservationAsync(input);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpPost]
        [Route("reject-reservation")]
        public async Task<IActionResult> RejectReservationBook([FromBody] BookReservationMessageDto input)
        {
            try
            {
                await bookReservationService.RejectBookReservationByIdAsync(input);

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("{bookReservationId}")]
        public async Task<IActionResult> GetBookReservation(Guid bookReservationId)
        {
            try
            {
                var result = await bookReservationService.GetBookReservationByIdAsync(bookReservationId);

                return Ok(result);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllBookReservations([FromQuery] PaginatorInputDto input)
        {
            try
            {
                var (result, totalCount) = await bookReservationService.GetBooksReservationsAsync(input);

                return Ok(new { result, totalCount });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpPost("approve-reservation")]
        public async Task<IActionResult> ApproveReservation([FromBody] BookReservationMessageDto input)
        {
            try
            {
                await bookReservationService.ApproveBookReservation(input);
            }
            catch (ArgumentNullException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (InvalidOperationException oe)
            {
                return NotFound(oe.Message);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
            
            return Ok();
        }
    }
}