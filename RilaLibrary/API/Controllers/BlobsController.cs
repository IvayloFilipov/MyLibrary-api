using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

using static Common.GlobalConstants;

namespace API.Controllers
{
    //It is only for testing the BlobService. Can be deleted later on.
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BlobsController : ControllerBase
    {
        private readonly IBlobService blobService;

        public BlobsController(IBlobService blobService)
        {
            this.blobService = blobService;
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("{blobName}")]
        public async Task<IActionResult> GetBlobFile(string blobName)
        {
            try
            {
                var data = await this.blobService.GetBlobFile(blobName);

                return Ok(data);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpPost("uploadfile")]
        public async Task<IActionResult> UploadFile(IFormFile file, string bookTitle)
        {
            try
            {
                var result = await this.blobService.UploadBlobFileAsync(file, bookTitle);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpPut("updatefile")]
        public async Task<IActionResult> UpdateFile(IFormFile file, string fileNameToUpdate, string newFileName)
        {
            try
            {
                var result = await this.blobService.UpdateBlobFileAsync(file, fileNameToUpdate, newFileName);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllBlobFiles()
        {
            try
            {
                var data = await this.blobService.GetAllBlobFiles();

                return Ok(data);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}")]
        [HttpDelete("{blobName}")]
        public async Task<IActionResult> DeleteFile(string blobName)
        {
            try
            {
                await this.blobService.DeleteBlobFileAsync(blobName);

                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
