using Fbi.Std.Core;
using Fbi.Std.Qbo.Core;
using System.Collections.Generic;

namespace Fbi.Std.Qbo.Pheidippides
{
    public class QboRefreshTokenStorage : ISecureStorage
    {
        readonly Dictionary<string, object> store = new Dictionary<string, object>();

        public T Retrieve<T>(string key)
        {
            //Fix me
            var sqlMule = new SqlMule("Server=tcp:fbidata.database.windows.net,1433;Initial Catalog=QBO_DATASET;Persist Security Info=False;User ID=MichaelB;Password=Cr@zyFresh;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            var result = sqlMule.Query("SELECT [RefreshKey],[KeyType] FROM [QBO_DATASET].[dbo].[QboTokens] WHERE [KeyType] = 'RefreshKey'");

            store[key] = result.ToArray()[0];

            sqlMule.DisposeOfConnection();

            return (T)store[key];
        }

        public void Store<T>(string key, T itemToStore)
        {
            var sqlMule = new SqlMule("Server=tcp:fbidata.database.windows.net,1433;Initial Catalog=QBO_DATASET;Persist Security Info=False;User ID=MichaelB;Password=Cr@zyFresh;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            var update = sqlMule.InsertRefreshToken(itemToStore.ToString());

            sqlMule.DisposeOfConnection();
        }
    }
}
