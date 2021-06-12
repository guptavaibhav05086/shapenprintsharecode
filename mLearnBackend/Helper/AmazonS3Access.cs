using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace mLearnBackend.Helper
{
    public static class AmazonS3Access
    {
        private const string bucketName = "rjbintdata";
        //private const string GSTBucketName = "userkycdata";
        // For simplicity the example creates two objects from the same file.
        // You specify key names for these objects.
        private const string keyName1 = "Test1upload";
        private const string keyName2 = "Test2upload";
        private const string filePath = @"F:\SiteLinks.txt";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        public static async Task<MemoryStream> ReadFileDataAsync(string keyName, string GSTBucketName)
        {
           
            IAmazonS3 client = new AmazonS3Client(bucketRegion);
            MemoryStream fileStream = new MemoryStream();
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = GSTBucketName,
                    Key = keyName
                };
               
                using (GetObjectResponse response = await client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                {

                    responseStream.CopyTo(fileStream);
                    fileStream.Position = 0;
                }
                //using (StreamReader reader = new StreamReader(responseStream))
                //{
                //    string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                //    string contentType = response.Headers["Content-Type"];
                //   // Console.WriteLine("Object metadata, Title: {0}", title);
                //    //Console.WriteLine("Content type: {0}", contentType);

                //    responseBody = reader.ReadToEnd(); // Now you process the response body.
                //}
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return fileStream;
        }


        public async static Task<bool> UploadKYCData(Stream stream, string fileName,string GSTBucketName)
        {
            IAmazonS3 client = new AmazonS3Client(bucketRegion);
            try
            {
                var fileTransferUtility =
                    new TransferUtility(client);
                //await fileTransferUtility.UploadAsync(stream, bucketName, fileName);
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = GSTBucketName,
                    InputStream = stream,
                    StorageClass = S3StorageClass.StandardInfrequentAccess,

                    Key = fileName,                   
                };

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public  static async Task WritingAnObjectAsync()
        {
            IAmazonS3 client = new AmazonS3Client(bucketRegion);
            try
            {
                
                // 1. Put object-specify only key name for the new object.
                var putRequest1 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName1,
                    ContentBody = "sample text"
                };
               
                PutObjectResponse response1 = await client.PutObjectAsync(putRequest1);

                client.PutACL(new PutACLRequest
                {
                    BucketName = bucketName,
                    Key = keyName1,
                    CannedACL = S3CannedACL.PublicRead
                });

                // 2. Put the object-set ContentType and add metadata.
                var putRequest2 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName2,
                    FilePath = filePath,
                    ContentType = "text/plain"
                };
                putRequest2.Metadata.Add("x-amz-meta-title", "someTitle");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
        }

        public async static Task<bool> UploadFileStream(Stream stream,string fileName)
        {
            IAmazonS3 client = new AmazonS3Client(bucketRegion);
            try
            {
                var fileTransferUtility =
                    new TransferUtility(client);
                //await fileTransferUtility.UploadAsync(stream, bucketName, fileName);
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    InputStream = stream,
                    StorageClass = S3StorageClass.StandardInfrequentAccess,
                   
                   Key = fileName,
                    CannedACL = S3CannedACL.PublicRead
                };
                //fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                //fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

            }
    }
}