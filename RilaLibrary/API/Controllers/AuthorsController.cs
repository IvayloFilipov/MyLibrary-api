using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using Services.Interfaces;

using static Common.GlobalConstants;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService authorService;

        public AuthorsController(IAuthorService authorService)
        {
            this.authorService = authorService;
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpPost("add")]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorDto input)
        {
            AuthorOutput authorOutput;

            try
            {
                authorOutput = await authorService.AddAuthorAsync(input);
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }

            return Ok(authorOutput);
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            List<AuthorOutput> authors = null!;

            try
            {
                authors = await authorService.GetAllAuthorsAsync();
            }
            catch (NullReferenceException ae)
            {

                return NotFound(ae.Message);
            }

            return Ok(authors);
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("getAuthors")]
        public async Task<IActionResult> GetAuthors([FromQuery] PaginatorInputDto input)
        {
            List<AuthorOutput> result;
            int totalCount;

            try
            {
                (result, totalCount) = await this.authorService.GetAuthorsAsync(input);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
                
            return Ok(new { result, totalCount });
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("{authorId}")]
        public async Task<IActionResult> GetAuthor(Guid authorId)
        {
            AuthorOutput author;

            try
            {
                author = await authorService.GetAuthorByIdAsync(authorId);
            }
            catch (NullReferenceException ae)
            {
                return NotFound(ae.Message);
            }

            return Ok(author);
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpPut("{authorId}")]
        public async Task<IActionResult> UpdateAuthor([FromBody]AuthorDto input, Guid authorId)
        {
            AuthorOutput authorOutput;

            try
            {
                authorOutput = await authorService.UpdateAuthorAsync(input, authorId);
            }
            catch (NullReferenceException ne)
            {
                return NotFound(ne.Message);
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
                
            return Ok(authorOutput);
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpDelete("{authorId}")]
        public async Task<IActionResult> DeleteAuthor(Guid authorId)
        {
            try
            {
                await authorService.DeleteAuthorAsync(authorId);
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (NullReferenceException ae)
            {
                return NotFound(ae.Message);
            }

            return Ok();
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}, {READER_ROLE_NAME}")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchAuthors([FromQuery] SearchAuthorDto input, [FromQuery] PaginatorInputDto pagination)
        {
            try
            {
                var (result, totalCount) = await this.authorService.SearchForAuthorsAsync(input, pagination);

                return Ok(new { result, totalCount });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("booksnum/{authorId}")]
        public async Task<IActionResult> GetAuthorBooksNumber(Guid authorId)
        {
            var result = await this.authorService.GetBooksNumberForAuthorAsync(authorId);

            return Ok(result);
        }
    }
}
