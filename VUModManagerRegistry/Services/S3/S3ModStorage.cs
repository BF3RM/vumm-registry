using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using VUModManagerRegistry.Common.Interfaces;

namespace VUModManagerRegistry.Services.S3
{
    public sealed class S3ModStorageOptions
    {
        public string ServiceURL { get; set; }
        
        [NotNull]
        public string BucketName { get; set; }
    }
    public class S3ModStorage : IModStorage
    {
        private readonly string _bucketName;
        private readonly AmazonS3Client _s3Client;

        public S3ModStorage(IOptions<S3ModStorageOptions> options)
        {
            _bucketName = options.Value.BucketName;
            _s3Client = new AmazonS3Client(new AmazonS3Config { ServiceURL = options.Value.ServiceURL });
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
                Expires = DateTime.UtcNow.AddSeconds(30),
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