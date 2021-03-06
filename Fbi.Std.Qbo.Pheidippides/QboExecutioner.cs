using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Fbi.Std.Core;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace Fbi.Std.Qbo.Pheidippides
{
    public static class QboExecutioner
    {
        private static string _sqlConnectionString;
        private static string _connectionID;
        private static string _baseURI;
        private static string _qboTbDb;
        private static string _qboRfDb;
        private static string _blobStorageConnectionString;
        private static string _blobStorageAccountName;
        private static bool _areWeHappyToRunTheScript;

        [FunctionName("QboExecutioner")]
        public static void Run([TimerTrigger("0 */4 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            //log.Info($"So you finally got me to run, eh?");
            //log.Info($"Let's get check everything is set up...");

            log.Info($"Loading params from appsettings");
            _connectionID = ConfigurationManager.AppSettings["QBOCONNECTIONID"];
            _baseURI = ConfigurationManager.AppSettings["BASEURIFORQBO"];
            _blobStorageConnectionString = ConfigurationManager.AppSettings["BLOBSTORAGECONNECTIONSTRING"];
            _blobStorageAccountName = ConfigurationManager.AppSettings["BLOBSTORAGEACCOUNTNAME"];
            _areWeHappyToRunTheScript = true;
            log.Info($"Verified params exist");

            try
            {
                log.Info($"Checking for valid Connections to Azure Environment");

                MyCustomBlobStorage.WriteBlobData(_blobStorageConnectionString, _blobStorageAccountName, "QBOTEST", "FreshBI was here, just kidding, this is just a Solution Template Test");
                log.Info($"Azure is set up properly");

                log.Info($"Look for RefreshTokens and Company IDs");
                log.Info($"Checking for null params");
                if (_connectionID.Length == 0 || _baseURI.Length == 0)
                {
                    throw new Exception("The QBO connection ID or the Base URI is not there");
                }
                log.Info($"No errors found in the Params set");
            }
            catch (Exception e)
            {
                _areWeHappyToRunTheScript = false;
                log.Error($"Error in the Params Check \n\nException: {e.Message} ");
            }

            if (_areWeHappyToRunTheScript)
            {
                QboPheidippides(log);
            }


        }

        public static async void QboPheidippides(TraceWriter log)
        {
            String sDate = DateTime.Now.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            String thisDay = datevalue.Day.ToString();
            String thisMonth = datevalue.Month.ToString();
            String thisYear = datevalue.Year.ToString();

            List<dynamic> tbDataRaw = new List<dynamic>();
            List<dynamic> plDataRaw = new List<dynamic>();
            List<JArray> tbDataJArrays = new List<JArray>();

            var intuitOAuthWrapper = new QboClientWrapper(_connectionID, _baseURI, new QboRefreshTokenStorage(_blobStorageConnectionString, _blobStorageAccountName));
            //var intuitOAuthWrapper = new QboClientWrapper(_connectionID, _baseURI, new QboRefreshTokenStorage(_sqlConnectionString, _qboRfDb));

            log.Info($"Retrieved a custom QBO Client");

            log.Info($"Loading Data into memory");
            var prefrencesRaw = await intuitOAuthWrapper.ExecuteGet("preferences");
            var accountSchema = await intuitOAuthWrapper.ExecuteGet("/query?query=select%20%2a%20from%20Account&minorversion=4");
            var glMaxMinDateDataSetRaw = await intuitOAuthWrapper.ExecuteGet("/reports/GeneralLedger?date_macro=all&columns=tx_date&sort_order=ascend");
            var glAccrualFullDataSetRaw = await intuitOAuthWrapper.ExecuteGet("/reports/GeneralLedger?date_macro=all&columns=tx_date,txn_type,doc_num,name,item_name,account_name,subt_nat_amount,debt_amt,credit_amt,inv_date,quantity,rate,cust_name,vend_name,emp_name,split_acc,is_ar_paid&accounting_method=Accrual");
            var glCashFullDataSetRaw = await intuitOAuthWrapper.ExecuteGet("/reports/GeneralLedger?date_macro=all&columns=tx_date,txn_type,doc_num,name,item_name,account_name,subt_nat_amount,debt_amt,credit_amt,inv_date,quantity,rate,cust_name,vend_name,emp_name,split_acc,is_ar_paid&accounting_method=Cash");
            var accountsList = await intuitOAuthWrapper.ExecuteGet("/reports/AccountList");

            //Load TB data for the last 2 years
            for (int year = int.Parse(thisYear); year >= int.Parse(thisYear) - 2; --year)
            {
                for (int month = 1; month <= 12; month++)
                {
                    var lastDayOfMonth = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
                    try
                    {
                        tbDataRaw.Add(await intuitOAuthWrapper.ExecuteGet($"reports/TrialBalance?start_date={year}-{month}-01&end_date={year}-{month}-{lastDayOfMonth.Day.ToString()}"));
                    }
                    catch (Exception e)
                    {
                        log.Info($"TB load error- {e.Message} ");
                    }
                }
            }
            // got all the TB data

            //log.Info($"Data in memeory, let's do something with it...");

            log.Info($"Let's load our data to LTS");
            MyCustomBlobStorage.WriteBlobData(_blobStorageConnectionString, _blobStorageAccountName, "GlMaxMinDate", StringCleaner.Clean(glMaxMinDateDataSetRaw.ToString()));
            MyCustomBlobStorage.WriteBlobData(_blobStorageConnectionString, _blobStorageAccountName, "GlAccrualFullDataSet", StringCleaner.Clean(glAccrualFullDataSetRaw.ToString()));
            MyCustomBlobStorage.WriteBlobData(_blobStorageConnectionString, _blobStorageAccountName, "GlCashFullDataSet", StringCleaner.Clean(glCashFullDataSetRaw.ToString()));
            MyCustomBlobStorage.WriteBlobData(_blobStorageConnectionString, _blobStorageAccountName, "PrefrencesDataSet", StringCleaner.Clean(prefrencesRaw.ToString()));
            MyCustomBlobStorage.WriteBlobData(_blobStorageConnectionString, _blobStorageAccountName, "AccountsListDataSet", StringCleaner.Clean(accountSchema.ToString()));


            //There must be a better way to skip the first instance of an Ienumerable instead of jsut removing it
            //This is done to facilitate the appending of the Json Queries in the TB
            //basically, if we're on the first loop through, i will equal 0, which means the Writedata will overwrite
            //on all other loopthroughs, the i will not equal 0, thus enabling the append
            for (int i = 0; i < tbDataRaw.Count; i++)
            {
                MyCustomBlobStorage.WriteBlobData(_blobStorageConnectionString, _blobStorageAccountName, "TBDump", StringCleaner.Clean(tbDataRaw[i].ToString()), i != 0);
            }

            //Fix me: add additional Union Quality to the TB


            log.Info("Good job Kids, we did it. Shut Down and wait for the next 12 hours.");
        }
    }
}
