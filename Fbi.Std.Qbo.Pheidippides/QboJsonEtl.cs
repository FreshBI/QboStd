using Fbi.Std.Core;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fbi.Std.Qbo.Pheidippides
{
    public class QboJsonEtl
    {

        //private string connectionString { get; set; }

        private SqlMule _sqlMule;
        private bool _persistConnection;
        private TraceWriter _log;
        private string _qboTbDb;

        /// <summary>
        /// The class constructor. </summary>
        /// <param name="log">This allows for logging in the ETL process</param>
        /// <param name="QbiTbDb">Defines the Table where we store the TB dataset</param>
        /// <param name="sqlConnectionString">This opens the Connection to a specified SQL Server</param>
        /// <param name="persistConnection">Manually Close the connection</param>
        public QboJsonEtl(TraceWriter log, string QboTbDb, string sqlConnectionString, bool persistConnection)
        {
            _sqlMule = new SqlMule(sqlConnectionString);
            _persistConnection = persistConnection;
            _log = log;
            _qboTbDb = QboTbDb;
        }

        /// <summary>
        /// This thing loads the TB tables </summary>
        /// <param name="historicalTBs">This is a list of JSON Payloads</param>
        public void TrialBalance(IEnumerable<dynamic> historicalTBs)
        {
            _sqlMule.ArbitrarySqlCode($"delete {_qboTbDb};");
            foreach (var tb in historicalTBs)
            {
                QboTbObject jsonObject = JsonConvert.DeserializeObject<QboTbObject>(tb.ToString());

                if (jsonObject.Rows.Row != null)
                {
                    foreach (Row row in jsonObject.Rows.Row)
                    {
                        if (row.group != "GrandTotal")
                        {
                            var debit = string.Format("{0:#.00}", Convert.ToDecimal("0.00"));
                            var credit = string.Format("{0:#.00}", Convert.ToDecimal("0.00"));

                            var account = row.ColData[0].value;
                            var accountID = row.ColData[0].id;

                            if (row.ColData[1].value == "")
                            {
                                debit = string.Format("{0:#.00}", Convert.ToDecimal("0.00"));
                            }
                            else
                            {
                                debit = string.Format("{0:#.00}", Convert.ToDecimal(row.ColData[1].value));
                            }

                            if (row.ColData[2].value == "")
                            {
                                credit = string.Format("{0:#.00}", Convert.ToDecimal("0.00"));
                            }
                            else
                            {
                                credit = string.Format("{0:#.00}", Convert.ToDecimal(row.ColData[2].value));
                            }
                            //[QBO_DATASET].[dbo].[QBO_TB]
                            _sqlMule.ArbitrarySqlCode($"INSERT INTO {_qboTbDb} (OpeningPeriod, Account, id, Debit, Credit) VALUES ('{jsonObject.Header.EndPeriod}', '{account}', '{accountID}', {debit}, {credit} )");
                        }

                    }
                }
            }

            if (!_persistConnection) { DisposeConnection(); }
        }

        /// <summary>
        /// This Kills the SQL Mule </summary>
        public void DisposeConnection()
        {
            _sqlMule.DisposeOfConnection();
        }
    }
}
