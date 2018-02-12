using Sandboxable.Microsoft.WindowsAzure.Storage;
using Sandboxable.Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Text;

namespace Fbi.Std.Core
{
    public static class MyCustomBlobStorage
    {
        /// <summary>
        /// Fills a Blob Storage Location with a Json Payload and creates a storage location if one does not exist.</summary>
        /// <param name="BlobConnectionString">Connect to BlobStorage</param>
        /// <param name="BlobStorageAccountName">Define the Account in Blob Storage</param>
        /// <param name="BlobStorageFileName">Define the Location in the BS Account</param>
        /// <param name="JsonPayload">The Json Payload we need to dump in Blob Storage</param>
        public static void OverWriteData(string BlobConnectionString, string BlobStorageAccountName, string BlobStorageFileName, string JsonPayload)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(BlobConnectionString);
            

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(BlobStorageAccountName); 

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobStorageFileName);

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(JsonPayload)))
            {
                blockBlob.UploadFromStream(ms);
            }
        }
    }
}
