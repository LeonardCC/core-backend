﻿using Lens.Core.Lib.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Lens.App.Blob
{
    public class FilesystemBlobService : BaseService<FilesystemBlobService>, IBlobService
    {
        private readonly BlobSettings _blobServiceSettings;

        public FilesystemBlobService(
            IApplicationService<FilesystemBlobService> applicationService, 
            IConfiguration configuration) : base(applicationService)
        {
            _blobServiceSettings = configuration.GetSection(nameof(BlobSettings)).Get<BlobSettings>();
        }

        public async Task<bool> DeleteBlob(string relativePathAndName)
        {
            var root = _blobServiceSettings.ContainerName;
            var path = Path.Combine(root, relativePathAndName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return await Task.FromResult(true);
        }

        public async Task<Stream> Download(string relativePathAndName)
        {
            var root = _blobServiceSettings.ContainerName;
            var path = Path.Combine(root, relativePathAndName);
            return await Task.FromResult(File.OpenRead(path));
        }

        public async Task<string[]> GetBlobs()
        {
            var root = _blobServiceSettings.ContainerName;
            var path = Path.Combine(root);
            string[] fileEntries = Directory.GetFiles(path);

            return await Task.FromResult(fileEntries);
        }

        public async Task<string> GetBlobUrl(string relativePathAndName)
        {
            var root = _blobServiceSettings.ContainerName;
            var path = Path.Combine(root, relativePathAndName);
            return File.Exists(path) 
                ? await Task.FromResult(path.Replace(root, "~").Replace("\\", "/")) 
                : await Task.FromResult(string.Empty);
        }

        public async Task<BlobMetadata> Upload(string blobInfoId, string relativePathAndName, Stream stream)
        {
            var extension = Path.GetExtension(relativePathAndName);
            var newFileName = $"{Guid.NewGuid()}{extension}";
            var relativePath = Path.GetDirectoryName(relativePathAndName);
            var folderPath = Path.Combine(_blobServiceSettings.ContainerName, relativePath);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var filePath = Path.Combine(folderPath, newFileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            return new BlobMetadata 
            { 
                RelativePathAndName = Path.Combine(relativePath, newFileName),
                FullPathAndName = filePath 
            };
        }
    }
}
