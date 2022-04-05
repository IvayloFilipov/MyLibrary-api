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
    public class BooksController : ControllerBase
    {
        private readonly IBookService booksService;

        public BooksController(IBookService booksService)
        {
            this.booksService = booksService;
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpPost("add")]
        public async Task<IActionResult> AddBook([FromForm] AddBookDto input)
        {
            try
            {
                var result = await this.booksService.AddBookAsync(input);

                return Ok(result.Id);
            }

            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpPut("{bookId}")]
        public async Task<IActionResult> UpdateBook(Guid bookId, [FromForm] AddBookDto input)
        {
            try
            {
                var result = await this.booksService.UpdateBookAsync(bookId, input);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBook(Guid bookId)
        {
            try
            {
                await this.booksService.DeleteBookAsync(bookId);

                return Ok();
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}, {READER_ROLE_NAME}")]
        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetBook(Guid bookId)
        {
            try
            {
                var result = await this.booksService.GetBookByIdAsync(bookId);

                return Ok(result);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}, {READER_ROLE_NAME}")]
        [HttpGet("getbooks")]
        public async Task<IActionResult> GetBooks([FromQuery] PaginatorInputDto input)
        {
            try
            {
                var (result, totalCount) = await this.booksService.GetBooksAsync(input);

                return Ok(new { result, totalCount });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var result = await this.booksService.GetAllBooksAsync();

                return Ok(result);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}, {READER_ROLE_NAME}")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] SearchBookDto input, [FromQuery] PaginatorInputDto pagination)
        {
            try
            {
                var (result, totalCount) = await this.booksService.SearchForBooksAsync(input, pagination);

                return Ok(new { result, totalCount });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("quantity/{bookId}")]
        public async Task<IActionResult> CompareBookQuantity(Guid bookId)
        {
            var result = await booksService.CompareBookQuantity(bookId);

            return Ok(result);
        }
    }
}
