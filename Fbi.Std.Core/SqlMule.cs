using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Fbi.Std.Core
{
    public class SqlMule
    {
        private string ConnectionString { get; set; }

        public SqlConnection SqlConnection;

        public SqlMule(string connectionString)
        {
            ConnectionString = connectionString;

            OpenConnection();
        }

        private void OpenConnection()
        {
            SqlConnection = new SqlConnection(ConnectionString);
            SqlConnection.Open();
        }

        public void DisposeOfConnection()
        {
            SqlConnection.Close();
        }

        public List<string> Query(string sqlQuery)
        {
            List<string> sqlResponse = new List<string>();

            SqlCommand cmd = new SqlCommand(sqlQuery, SqlConnection);

            SqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader.HasRows)
            {
                //var asdf = dataReader.ToString();
                while (dataReader.Read())
                {
                    sqlResponse.Add(dataReader.GetString(0).ToString());
                    //log.Info(dataReader.GetString(0).ToString());
                }
                //log.Info("Made it out of the where");
            }

            dataReader.Close();

            return sqlResponse;
        }

        public void ArbitrarySqlCode(string cmdString)
        {
            SqlCommand cmd = new SqlCommand(cmdString, SqlConnection);

            cmd.ExecuteNonQuery();
        }

        public bool InsertRefreshToken(string refreshToken, bool clearTable)
        {
            var sb = new StringBuilder();

            if (clearTable) sb.Append("DELETE FROM QboTokens WHERE  KeyType = 'RefreshKey';");

            sb.Append("INSERT INTO [QBO_DATASET].[dbo].[QboTokens] (RefreshKey, KeyType) VALUES ('" + refreshToken + "','RefreshKey' );");

            SqlCommand cmd = new SqlCommand(sb.ToString(), SqlConnection);
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool InsertRefreshToken(string refreshToken)
        {
            return InsertRefreshToken(refreshToken, true);
        }
    }
}
