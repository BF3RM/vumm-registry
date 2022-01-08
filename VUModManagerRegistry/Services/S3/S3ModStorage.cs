using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using VUModManagerRegistry.Common.Interfaces;

namespace VUModManagerRegistry.Services.S3
{
    [ExcludeFromCodeCoverage]
    public sealed class S3ModStorageOptions
    {
        public string ServiceUrl { get; set; }

        [Required]
        public string BucketName { get; set; }
    }
    
    [ExcludeFromCodeCoverage]
    public class S3ModStorage : IModStorage
    {
        private readonly ISystemTimeProvider _systemTimeProvider;
        private readonly AmazonS3Client _s3Client;
        private readonly string _bucketName;

        public S3ModStorage(ISystemTimeProvider systemTimeProvider, IOptions<S3ModStorageOptions> options)
        {
            _systemTimeProvider = systemTimeProvider;
            _s3Client = string.IsNullOrEmpty(options.Value.ServiceUrl) ? new AmazonS3Client() : new AmazonS3Client(new AmazonS3Config {ServiceURL = options.Value.ServiceUrl});
            _bucketName = options.Value.BucketName;
        }

        public async Task StoreArchive(string modName, string modVersion, Stream stream)
        {
            await _s3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _bucketName,
                ContentType = "application/octet-stream",
                Key = CreateModVersionKey(modName, modVersion),
                InputStream = stream
            });
        }

        public async Task DeleteArchive(string modName, string modVersion)
        {
            await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = CreateModVersionKey(modName, modVersion)
            });
        }

        public string GetArchiveDownloadLink(string modName, string modVersion)
        {
            return _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Expires = _systemTimeProvider.Now.AddSeconds(30),
                Key = CreateModVersionKey(modName, modVersion),
                Verb = HttpVerb.GET
            });
        }

        private string CreateModVersionKey(string modName, string modVersion)
        {
            return $"{modName}/{modVersion}.tgz";
        }
    }
}