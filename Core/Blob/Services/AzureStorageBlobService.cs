﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Lens.Core.Blob.Models;
using Lens.Core.Lib.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Lens.Core.Blob.Services
{
    public class AzureStorageBlobService : BaseService<AzureStorageBlobService>, IBlobService
    {
        private readonly BlobSettings _blobServiceSettings;
        private readonly BlobContainerClient blobcontainerClient;

        public AzureStorageBlobService(
            IApplicationService<AzureStorageBlobService> applicationService,
            IConfiguration configuration, BlobServiceClient blobServiceClient) : base(applicationService)
        {
            _blobServiceSettings = configuration.GetSection(nameof(BlobSettings)).Get<BlobSettings>();
            this.blobcontainerClient = blobServiceClient.GetBlobContainerClient(_blobServiceSettings.ContainerPath);
        }

        public async Task<bool> DeleteBlob(string relativePathAndName)
        {
            BlobClient blobClient = blobcontainerClient.GetBlobClient(relativePathAndName);

            return (await blobClient.DeleteIfExistsAsync()).Value;
        }

        public async Task<Stream> Download(string relativePathAndName)
        {
            BlobClient blobClient = blobcontainerClient.GetBlobClient(relativePathAndName);

            var fStream = new MemoryStream();
            await blobClient.DownloadToAsync(fStream);

            return fStream;
        }

        public async Task<BlobDownloadResultModel> DownloadWithMetadata(string relativePathAndName)
        {
            BlobClient blobClient = blobcontainerClient.GetBlobClient(relativePathAndName);

            var fStream = new MemoryStream();
            var response = await blobClient.DownloadToAsync(fStream);

            return new BlobDownloadResultModel(
                        fStream, 
                        response.Headers.ContentType, 
                        response.Headers.ContentLength);
        }

        public async Task<string[]> GetBlobs()
        {
            var values = new List<string>();
            await foreach (BlobItem blob in blobcontainerClient.GetBlobsAsync())
            {
                values.Add(blob.Name);
            }

            return values.ToArray();
        }

        public async Task<string> GetBlobUrl(string relativePathAndName)
        {
            BlobClient blobClient = blobcontainerClient.GetBlobClient(relativePathAndName);


            return blobClient.Uri.AbsoluteUri.ToString();
        }

        public async Task<BlobMetadataModel> Upload(string relativePathAndName, Stream stream)
        {
            BlobClient blobClient = blobcontainerClient.GetBlobClient(relativePathAndName);

            var uploadInfo = await blobClient.UploadAsync(stream);

            var blobMetadata = new BlobMetadataModel()
            {
                RelativePathAndName = relativePathAndName,
                FullPathAndName = relativePathAndName
            };

            return blobMetadata;
        }

        public Task MoveBlob(string sourceRelativePathAndName, string targetRelativePathAndName)
        {
            BlobClient sourceBlobClient = blobcontainerClient.GetBlobClient(sourceRelativePathAndName);
            BlobClient targetBlobClient = blobcontainerClient.GetBlobClient(targetRelativePathAndName);
            var result = targetBlobClient.StartCopyFromUri(sourceBlobClient.Uri);
            return sourceBlobClient.DeleteAsync();
        }
    }
}
