using Fbi.Std.Core;
using Fbi.Std.Qbo.Core;
using System.Collections.Generic;

namespace Fbi.Std.Qbo.Pheidippides
{
    public class QboRefreshTokenStorage : ISecureStorage
    {
        readonly Dictionary<string, object> store = new Dictionary<string, object>();

        private string _blobStorageConnectionString;
        private string _blobStorageAccountName;

        public QboRefreshTokenStorage(string blobStorageConnectionString, string blobStorageAccountName)
        {
            _blobStorageAccountName = blobStorageAccountName;
            _blobStorageConnectionString = blobStorageConnectionString;
        }

        public T Retrieve<T>(string key)
        {
            var result = MyCustomBlobStorage.RetrieveData(_blobStorageConnectionString, _blobStorageAccountName, key);

            store[key] = result;

            return (T)store[key];
        }

        public void Store<T>(string key, T itemToStore)
        {
            //string newT2 = (string)(object)itemToStore;

            MyCustomBlobStorage.WriteBlobData(_blobStorageConnectionString, _blobStorageAccountName, key, (string)(object)itemToStore);
        }
    }

    //public class QboRefreshTokenStorage : ISecureStorage
    //{
    //    readonly Dictionary<string, object> store = new Dictionary<string, object>();

    //    private SqlMule _sqlMule;
    //    private string _qboRefreshKeyTable;

    //    public QboRefreshTokenStorage(string ConnectionString, string QboRefreshKeyTable)
    //    {
    //        _sqlMule = new SqlMule(ConnectionString);
    //        _qboRefreshKeyTable = QboRefreshKeyTable;
    //    }

    //    public T Retrieve<T>(string key)
    //    {
    //        var result = _sqlMule.Query($"SELECT [RefreshKey],[KeyType] FROM {_qboRefreshKeyTable} WHERE [KeyType] = 'RefreshKey'");

    //        store[key] = result.ToArray()[0];

    //        //_sqlMule.DisposeOfConnection();

    //        return (T)store[key];
    //    }

    //    public void Store<T>(string key, T itemToStore)
    //    {
    //        var update = _sqlMule.InsertRefreshToken(itemToStore.ToString());

    //        //sqlMule.DisposeOfConnection();
    //    }
    //}

}
