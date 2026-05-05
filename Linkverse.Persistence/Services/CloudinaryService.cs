using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Linkverse.Application.Interfaces.IServices;
using Linkverse.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;


namespace Linkverse.Persistence.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            //var settings = config.Value;

            //if (string.IsNullOrWhiteSpace(settings.CloudName))
            //    throw new ArgumentException("CloudName is missing in CloudinarySettings");

            var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
            _cloudinary = new Cloudinary(account);

        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "avatars"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
    }
}
