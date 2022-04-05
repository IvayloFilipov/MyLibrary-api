using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace Services.Interfaces
{
    public interface IBlobService
    {
        Task<string> GetBlobFile(string name);

        Task<IEnumerable<string>> GetAllBlobFiles();

        Task<string> UploadBlobFileAsync(IFormFile file, string bookTitle);

        Task<string> UpdateBlobFileAsync(IFormFile file, string fileNameToUpdate, string newFileName);

        Task<string> RenameBlobFileAsync(string oldFileName, string newFileName);

        Task RemoveBlobFileAsync(string oldFileName);

        Task DeleteBlobFileAsync(string name);

        void CheckFileExtension(string fileExtension);

        void CheckFileSize(IFormFile file);

        Task UploadFileToBlobAndCheckResultAsync(IFormFile file, BlobClient blobClient);

        void CheckBlobClientExists(BlobClient blobClient);

        void CheckBlobStorageIsEmpty(Azure.AsyncPageable<BlobItem>? blobs);

        string GetOldFileName(BlobContainerClient containerClient, string inputName);

        Task StartFileCopy(BlobClient blobClient, Uri oldUri);
    }
}
