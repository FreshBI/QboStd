using Sandboxable.Microsoft.WindowsAzure.Storage;
using Sandboxable.Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Text;

namespace Fbi.Std.Core
{
    public static class MyCustomBlobStorage
    {
        public static void OverWriteData(string json, string file)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=qbotestoneb385;AccountKey=/yQQq/YUX6nMOC5umuzuhpDVeMtjbKawBOBz3GJSjjuoXpIYJx9Wo5kNECh9ANEMX2/0H8LhqdySiiRmUsolMg==;EndpointSuffix=core.windows.net");
            //Fix me

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("qbo-data");
            //Fix me


            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(file);

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                blockBlob.UploadFromStream(ms);
            }
        }
    }
}
