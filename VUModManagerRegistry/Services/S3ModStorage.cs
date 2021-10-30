using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Services
{
    public class S3ModStorage : IModStorage
    {
        private readonly AmazonS3Client _s3Client;
        
        public S3ModStorage()
        {
            // _s3Client = new AmazonS3Client(new AmazonS3Config { ServiceURL = "https://s3.wasabisys.com" });
            _s3Client = new AmazonS3Client();
        }

        public string GetUploadLink(ModVersion version)
        {
            return _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = "vumm",
                Expires = DateTime.UtcNow.AddSeconds(30),
                Key = CreateModVersionKey(version),
                Verb = HttpVerb.PUT
            });
        }

        public string GetDownloadLink(ModVersion version)
        {
            return _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = "vumm",
                Expires = DateTime.UtcNow.AddSeconds(30),
                Key = CreateModVersionKey(version),
                Verb = HttpVerb.GET
            });
        }

        private string CreateModVersionKey(ModVersion modVersion)
        {
            return $"{modVersion.Name}/{modVersion.Version}.tar.gz";
        }
    }
}