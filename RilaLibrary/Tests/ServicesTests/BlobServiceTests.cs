using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Common.ExceptionMessages;

namespace Tests.ServicesTests
{
    [TestFixture]
    public class BlobServiceTests
    {
        private readonly Mock<BlobServiceClient> mockBlobServiceClient = new Mock<BlobServiceClient>();
        private readonly Mock<BlobContainerClient> mockContainerClient = new Mock<BlobContainerClient>();
        private readonly Mock<BlobClient> mockBlobClient = new Mock<BlobClient>();
        private BlobService? blobService;

        [SetUp]
        public void Setup()
        {
            blobService = new BlobService(mockBlobServiceClient.Object);
            mockBlobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(mockContainerClient.Object);
            mockContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(mockBlobClient.Object);
        }

        [Test]
        public void Should_ReturnArgumentException_When_UploadingWrongFormat()
        {
            mockBlobClient.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default)).ReturnsAsync(default(Response<BlobContentInfo>));

            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile inputFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.gif") { Headers = new HeaderDictionary(), ContentType = "image/gif" };

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await blobService!.UploadBlobFileAsync(inputFile, "Book 1"));

            Assert.AreEqual(FILE_NOT_CORRECT_FORMAT, result!.Message);
        }

        [Test]
        public void Should_ReturnArgumentException_When_UploadReturnsNull()
        {
            mockBlobClient.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default)).ReturnsAsync(default(Response<BlobContentInfo>));

            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile inputFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.jpg") { Headers = new HeaderDictionary(), ContentType = "image/jpeg" };

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await blobService!.UploadBlobFileAsync(inputFile, "Book 1"));

            Assert.AreEqual(FILE_UPLOAD_FAILED, result!.Message);
        }
    }
}
