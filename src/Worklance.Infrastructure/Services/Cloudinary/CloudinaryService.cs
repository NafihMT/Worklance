using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Worklance.Application.Interfaces.CloudinaryInterface;
using Worklance.Application.Common.Settings;

namespace Worklance.Infrastructure.Services.Cloudinary
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> options)
        {
            var settings = options.Value;

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret);

            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(Stream stream, string fileName)
        {
            if (stream == null || stream.Length == 0)
                throw new Exception("Invalid Image");

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = "worklance/aadhaar"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);

            return uploadResult.SecureUrl.ToString();
        }

        public async Task<string> UploadFileAsync(Stream stream, string fileName, string folder)
        {
            if (stream == null || stream.Length == 0)
                throw new Exception("Invalid file");

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);

            return uploadResult.SecureUrl.ToString();
        }
    }
}
