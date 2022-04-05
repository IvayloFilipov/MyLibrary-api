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
    public class GenresController : ControllerBase
    {
        private readonly IGenreService genreService;

        public GenresController(IGenreService genreService)
        {
            this.genreService = genreService;
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpPost("add")]
        public async Task<IActionResult> AddGenre([FromBody] Genre input)
        {
            try
            {
                var result = await this.genreService.AddGenreAsync(input);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpPut("{genreId}")]
        public async Task<IActionResult> UpdateGenre(Guid genreId, [FromBody] Genre input)
        {
            try
            {
                var result = await this.genreService.UpdateGenreAsync(genreId, input);

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
        [HttpDelete("{genreId}")]
        public async Task<IActionResult> DeleteGenre(Guid genreId)
        {
            try
            {
                await this.genreService.DeleteGenreAsync(genreId);

                return Ok();
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
        [HttpGet("{genreId}")]
        public async Task<IActionResult> GetGenre(Guid genreId)
        {
            try
            {
                var result = await this.genreService.GetGenreByIdAsync(genreId);

                return Ok(result);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("getgenres")]
        public async Task<IActionResult> GetGenres([FromQuery] PaginatorInputDto input)
        {
            try
            {
                var (result, totalCount) = await this.genreService.GetGenresAsync(input);
                return Ok(new { result, totalCount });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllGenres()
        {
            try
            {
                var result = await this.genreService.GetAllGenresAsync();

                return Ok(result);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}, {READER_ROLE_NAME}")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchGenres([FromQuery] SearchGenreDto input, [FromQuery] PaginatorInputDto pagination)
        {
            try
            {
                var (result, totalCount) = await this.genreService.SearchForGenresAsync(input, pagination);

                return Ok(new { result, totalCount });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("booksnum/{genreId}")]
        public async Task<IActionResult> GetGenreBooksNumber(Guid genreId)
        {
            var result = await this.genreService.GetBooksNumberForGenreAsync(genreId);

            return Ok(result);
        }
    }
}
