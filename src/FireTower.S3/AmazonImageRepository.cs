using System;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using FireTower.Domain.Services;

namespace FireTower.S3
{
    public class AmazonImageRepository : IImageRepository
    {
        const string AccessKey = "AKIAJSG4NKQ3XIDP7QGA";
        const string SecretKey = "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";

        #region IImageRepository Members

        public Uri Save(MemoryStream imageStream)
        {
            string url;
            using (IAmazonS3 client = new AmazonS3Client(AccessKey, SecretKey, RegionEndpoint.USEast1))
            {
                Guid imageFilename = Guid.NewGuid();
                PutObjectRequest request = BuildRequest(imageFilename, imageStream);
                client.PutObject(request);
                url = "https://s3.amazonaws.com/FireTower_DisasterImages/disasters/" + imageFilename.ToString();
            }
            return new Uri(url);
        }

        #endregion

        static PutObjectRequest BuildRequest(Guid id, MemoryStream ms)
        {
            var request = new PutObjectRequest
                              {
                                  BucketName = "FireTower_DisasterImages",
                                  CannedACL = S3CannedACL.PublicRead,
                                  Key = "disasters/" + id,
                                  InputStream = ms
                              };
            return request;
        }

        static MemoryStream GetMemoryStreamFromBase64(string base64ImageString)
        {
            byte[] data = Convert.FromBase64String(base64ImageString);
            var ms = new MemoryStream(data);
            return ms;
        }
    }
}