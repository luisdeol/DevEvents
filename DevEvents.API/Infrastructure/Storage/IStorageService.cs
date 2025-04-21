using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace DevEvents.API.Infrastructure.Storage
{
    public interface IStorageService
    {
        Task<bool> UploadPhoto(string prefix, string fileName, Stream file);
        Task<Stream> DownloadPhoto(string prefix, string fileName);
    }

    public class BlobStorageService : IStorageService
    {
        readonly BlobContainerClient _containerClient;
        const string PhotoContainerName = "photos";

        public BlobStorageService(BlobServiceClient client)
        {
            _containerClient = client.GetBlobContainerClient(PhotoContainerName);
        }

        public async Task<Stream> DownloadPhoto(string prefix, string fileName)
        {
            var blobName = $"{prefix}/{fileName}"; // 123/foto.jpg
            var blobClient = _containerClient.GetBlobClient(blobName);

            var memoryStream = new MemoryStream();

            await blobClient.DownloadToAsync(memoryStream);

            memoryStream.Position = 0;

            var contentType = blobClient.GetProperties().Value.ContentType;

            return memoryStream;
        }

        public async Task<bool> UploadPhoto(string prefix, string fileName, Stream file)
        {
            try
            {
                var blobName = $"{prefix}/{fileName}"; // 123/foto.jpg

                var blobClient = _containerClient.GetBlobClient(blobName);

                var result = await blobClient.UploadAsync(file, true);

                return true;
            } catch (Exception)
            {
                return false;
            }
        }
    }
}
