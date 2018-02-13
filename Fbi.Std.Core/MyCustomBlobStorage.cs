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
        public static void WriteBlobData(string BlobConnectionString, string BlobStorageAccountName, string BlobStorageFileName, string JsonPayload)
        {
            WriteBlobData(BlobConnectionString, BlobStorageAccountName, BlobStorageFileName, JsonPayload, false);
        }

        /// <summary>
        /// Fills a Blob Storage Location with a Json Payload and creates a storage location if one does not exist. Using the AppendSwitch, you can overwrite or append to the data set ( defauts to Overwrite )</summary>
        /// <param name="BlobConnectionString">Connect to BlobStorage</param>
        /// <param name="BlobStorageAccountName">Define the Account in Blob Storage</param>
        /// <param name="BlobStorageFileName">Define the Location in the BS Account</param>
        /// <param name="JsonPayload">The Json Payload we need to dump in Blob Storage</param>
        /// <param name="Append">Set to true to Enable Appending of pre-existing data</param>
        public static void WriteBlobData(string BlobConnectionString, string BlobStorageAccountName, string BlobStorageFileName, string JsonPayload, bool Append)
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

            if (Append)
            {
                JsonPayload += "<<-->>" + RetrieveData(BlobConnectionString, BlobStorageAccountName, BlobStorageFileName);
            }

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(JsonPayload)))
            {

                blockBlob.UploadFromStream(ms);

            }
        }

        /// <summary>
        /// Retrieves data from a Blob Storage Location.</summary>
        /// <param name="BlobConnectionString">Connect to BlobStorage</param>
        /// <param name="BlobStorageAccountName">Define the Account in Blob Storage</param>
        /// <param name="BlobStorageFileName">Define the Location in the BS Account</param>
        public static string RetrieveData(string BlobConnectionString, string BlobStorageAccountName, string BlobStorageFileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(BlobConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(BlobStorageAccountName);

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobStorageFileName);

            string memoryBlockToReturn;
            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                memoryBlockToReturn = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return memoryBlockToReturn;
        }
    }
}
