using System.Web;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using log4net;
using Services.Interfaces;

using static Common.ExceptionMessages;
using static Common.GlobalConstants;

namespace Services.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient blobServiceClient;
        private ILog log = LogManager.GetLogger(typeof(BlobService));

        public BlobService(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadBlobFileAsync(IFormFile file, string bookTitle)
        {
            var fileName = bookTitle;
            var fileExtension = Path.GetExtension(file.FileName);
            var allFileName = string.Concat(fileName, fileExtension);

            CheckFileExtension(fileExtension);
            CheckFileSize(file);

            var containerClient = blobServiceClient.GetBlobContainerClient(BLOB_STORAGE_CONTAINER);
            var blobClient = containerClient.GetBlobClient(allFileName);

            await UploadFileToBlobAndCheckResultAsync(file, blobClient);
            log.Info("File is uploaded.");

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> GetBlobFile(string name)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(BLOB_STORAGE_CONTAINER);
            var blobClient = containerClient.GetBlobClient(name);

            var blobs = await GetAllBlobFiles();

            if (!blobs.Any())
            {
                log.Error($"Blobs are not found. Exception is thrown: {BLOBSTORAGE_IS_EMPTY}");
                throw new ArgumentException(BLOBSTORAGE_IS_EMPTY);
            }

            if (!blobs.Contains(blobClient.Name))
            {
                log.Error($"there is no blob with name {blobClient.Name}. Exception is thrown: {BLOBFILE_NOT_EXIST}");
                throw new ArgumentException(BLOBFILE_NOT_EXIST);
            }

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UpdateBlobFileAsync(IFormFile file, string fileNameToUpdate, string newFileName)
        {
            var fileName = newFileName;
            var fileExtension = Path.GetExtension(file.FileName);
            var createdFileName = string.Concat(fileName, fileExtension);

            var containerClient = blobServiceClient.GetBlobContainerClient(BLOB_STORAGE_CONTAINER);

            var oldFileName = GetOldFileName(containerClient, fileNameToUpdate);

            var oldBlobClient = containerClient.GetBlobClient(oldFileName);
            var newBlobClient = containerClient.GetBlobClient(createdFileName);

            CheckBlobClientExists(oldBlobClient);

            await oldBlobClient.DeleteAsync();

            await UploadFileToBlobAndCheckResultAsync(file, newBlobClient);
            log.Info("Blob is updated.");

            return newBlobClient.Uri.AbsoluteUri;
        }

        public async Task<string> RenameBlobFileAsync(string oldFileName, string newFileName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(BLOB_STORAGE_CONTAINER);

            var oldUri = new Uri(oldFileName);
            var oldName = System.IO.Path.GetFileName(oldUri.LocalPath);
            var oldExtension = System.IO.Path.GetExtension(oldUri.LocalPath);

            var createdFileName = string.Concat(newFileName, oldExtension);

            var oldBlobClient = containerClient.GetBlobClient(oldName);
            var newBlobClient = containerClient.GetBlobClient(createdFileName);

            CheckBlobClientExists(oldBlobClient);
            await StartFileCopy(newBlobClient, oldUri);

            await oldBlobClient.DeleteAsync();
            log.Info("Blob is renamed.");

            return newBlobClient.Uri.AbsoluteUri;
        }

        public async Task RemoveBlobFileAsync(string oldFileName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(BLOB_STORAGE_CONTAINER);

            var oldUri = new Uri(oldFileName);
            var oldName = Path.GetFileName(oldUri.LocalPath);
            oldName = oldName.Replace("%20", " ");

            var oldBlobClient = containerClient.GetBlobClient(oldName);

            CheckBlobClientExists(oldBlobClient);

            await oldBlobClient.DeleteAsync();
        }

        public async Task<IEnumerable<string>> GetAllBlobFiles()
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(BLOB_STORAGE_CONTAINER);
            var files = new List<string>();
            var blobs = containerClient.GetBlobsAsync();

            CheckBlobStorageIsEmpty(blobs);

            await foreach (var blob in blobs)
            {
                files.Add(blob.Name);
            }

            return files;
        }

        public async Task DeleteBlobFileAsync(string name)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(BLOB_STORAGE_CONTAINER);
            var blobClient = containerClient.GetBlobClient(name);

            CheckBlobClientExists(blobClient);

            await blobClient.DeleteAsync();
            log.Info("Bob id deleted.");
        }

        public void CheckFileExtension(string fileExtension)
        {
            if (fileExtension != ".png" &&
                fileExtension != ".jpg" &&
                fileExtension != ".jpeg" &&
                fileExtension != ".PNG" &&
                fileExtension != ".JPG" &&
                fileExtension != ".JPEG")
            {
                log.Error($"File extension is nit alowed. Exception is thrown {FILE_NOT_CORRECT_FORMAT}.");
                throw new ArgumentException(FILE_NOT_CORRECT_FORMAT);
            }
        }

        public void CheckFileSize(IFormFile file)
        {
            if (file.Length > 512 * 1024)
            {
                log.Error($"File is too big. Exception is thrown {FILE_OVER_SIZE}.");
                throw new ArgumentException(FILE_OVER_SIZE);
            }
        }

        public async Task UploadFileToBlobAndCheckResultAsync(IFormFile file, BlobClient blobClient)
        {
            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);

            if (result == null)
            {
                log.Error($"File uploading failed. Exception is thrown {FILE_UPLOAD_FAILED}.");
                throw new ArgumentException(FILE_UPLOAD_FAILED);
            }
        }

        public void CheckBlobClientExists(BlobClient blobClient)
        {
            if (!blobClient.Exists())
            {
                log.Error($"Blob client was not found. Exception is thrown {BLOBFILE_NOT_EXIST}.");
                throw new ArgumentException(BLOBFILE_NOT_EXIST);
            }
        }

        public void CheckBlobStorageIsEmpty(Azure.AsyncPageable<BlobItem>? blobs)
        {
            if (blobs == null)
            {
                log.Error($"Exception is thrown {BLOBSTORAGE_IS_EMPTY}.");
                throw new ArgumentException(BLOBSTORAGE_IS_EMPTY);
            }
        }

        public string GetOldFileName(BlobContainerClient containerClient, string inputName)
        {
            inputName = HttpUtility.UrlDecode(inputName);
            var result = inputName.Replace(containerClient.Uri.AbsoluteUri + '/', string.Empty);

            return result;
        }

        public async Task StartFileCopy(BlobClient blobClient, Uri oldUri)
        {
            var result = await blobClient.StartCopyFromUriAsync(oldUri);

            if (result == null)
            {
                log.Error($"Uploading file failed. Exception is thrown {FILE_UPLOAD_FAILED}.");
                throw new ArgumentException(FILE_UPLOAD_FAILED);
            }
        }
    }
}
