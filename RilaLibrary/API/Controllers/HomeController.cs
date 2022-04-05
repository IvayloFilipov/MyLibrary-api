using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Models.InputDTOs;
using Services.Interfaces;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IBookService booksService;
        private readonly IGenreService genreService;
        private readonly IUserService userService;

        public HomeController(IBookService booksService, IGenreService genreService, IUserService userService)
        {
            this.booksService = booksService;
            this.genreService = genreService;
            this.userService = userService;
        }

        [HttpGet("last-books")]
        public async Task<IActionResult> GetLastBooks([FromQuery] PaginatorInputDto paginatorInput)
        {
            var (retrievedBooks, booksCount) = await this.booksService.GetBooksForLastTwoWeeksAsync(paginatorInput);

            return Ok(new { retrievedBooks, booksCount });
        }

        [HttpGet("books-count")]
        public async Task<ActionResult> GetBooksCount()
        {
            var booksCount = await this.booksService.GetCountOfAllBooksAsync();

            return Ok(booksCount);
        }

        [HttpGet("genres-count")]
        public async Task<ActionResult> GetGenresCount()
        {
            var genresCount = await this.genreService.GetCountOfAllGenresAsync();

            return Ok(genresCount);
        }

        [HttpGet("readers-count")]
        public async Task<ActionResult> GetReadersCount()
        {
            var readersCount = await this.userService.GetCountOfAllReadersAsync();

            return Ok(readersCount);
        }
    }
}
