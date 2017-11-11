using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AmazonServices
{
    public class Bucket
    {
        public string BucketDestino { get; set; }
        public string FileName { get; set; }
        public StreamReader File { get; set; }

        public Bucket()
        {
        }

        public string SaveToBlob(string bucketDestino, MemoryStream filePath, string contentType)
        {
            try
            {
               
                TransferUtility fileTransferUtility = new
                    TransferUtility(new AmazonS3Client(ConfigurationManager.AppSettings["awsAccessKeyId"].ToString(), ConfigurationManager.AppSettings["awsSecretAccessKey"].ToString(), Amazon.RegionEndpoint.SAEast1)
                    );


                string nomeArquivo = Guid.NewGuid().ToString();
                nomeArquivo = nomeArquivo + "." + contentType;
                fileTransferUtility.Upload(filePath, bucketDestino, nomeArquivo);
                var urlBase = @"https://s3-sa-east-1.amazonaws.com/";
                var urlRetorno = urlBase + bucketDestino + "/" + nomeArquivo;
                return urlRetorno;

            }
            catch (AmazonS3Exception s3Exception)
            {
                var x = s3Exception.Message;
                return "Error" + x;


            }
        }


        public async Task<String> SaveToBlobAsync(string bucketDestino, MemoryStream filePath, string contentType)
        {
            try
            {
                TransferUtility fileTransferUtility = new
                   TransferUtility(new AmazonS3Client(ConfigurationManager.AppSettings["awsAccessKeyId"].ToString(), ConfigurationManager.AppSettings["awsSecretAccessKey"].ToString(), Amazon.RegionEndpoint.SAEast1)
                   );

                string nomeArquivo = Guid.NewGuid().ToString();
                nomeArquivo = nomeArquivo + "." + contentType;
                await fileTransferUtility.UploadAsync(filePath, bucketDestino, nomeArquivo);
                string url = @"https://s3-sa-east-1.amazonaws.com/";
                url += bucketDestino + "/" + nomeArquivo;
                return url;
            }
            catch (AmazonS3Exception s3Exception)
            {
                var x = s3Exception.Message;
                return "Error" + x;


            }
        }



    }
}
