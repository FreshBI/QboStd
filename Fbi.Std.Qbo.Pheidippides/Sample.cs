
//using System;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
//using Fbi.Std.Core;
//using Fbi.Std.Qbo.Core;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Configuration;

//namespace Fbi.Std.Qbo.Pheidippides
//{
//    public static class QboExecutioner
//    {
//        [FunctionName("QboExecutioner")]
//        public static async void Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, TraceWriter log)
//        {
//            String sDate = DateTime.Now.ToString();
//            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

//            String thisDay = datevalue.Day.ToString();
//            String thisMonth = datevalue.Month.ToString();
//            String thisYear = datevalue.Year.ToString();

//            string connectionString = "Server=tcp:fbidata.database.windows.net,1433;" +
//                "Initial Catalog=QBO_DATASET;" +
//                "Persist Security Info=False;" +
//                "User ID=MichaelB;" +
//                "Password=Cr@zyFresh;" +
//                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

//            string connectionID = "123145774537644";

//            List<dynamic> tbDataRaw = new List<dynamic>();
//            List<JArray> tbDataJArrays = new List<JArray>();
//            QboJsonEtl jsonEtl = new QboJsonEtl(log, connectionString, false);

//            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
//            log.Info($"So you finally got me to run, eh?");
//            log.Info($"Let's get check everything is set up...");
//            var intuitOAuthWrapper = new QboClientWrapper(connectionID, new QboRefreshTokenStorage(connectionString));
//            log.Info($"Got our Client");

//            log.Info($"Loading Data into memory");
//            var prefrencesRaw = await intuitOAuthWrapper.ExecuteGet("preferences");
//            var glMaxMinDateDataSetRaw = await intuitOAuthWrapper.ExecuteGet("/reports/GeneralLedger?date_macro=all&columns=tx_date&sort_order=ascend");
//            var glAccrualFullDataSetRaw = await intuitOAuthWrapper.ExecuteGet("/reports/GeneralLedger?date_macro=all&columns=tx_date,txn_type,doc_num,name,item_name,account_name,subt_nat_amount,debt_amt,credit_amt,inv_date,quantity,rate,cust_name,vend_name,emp_name,split_acc,is_ar_paid&accounting_method=Accrual");
//            var glCashFullDataSetRaw = await intuitOAuthWrapper.ExecuteGet("/reports/GeneralLedger?date_macro=all&columns=tx_date,txn_type,doc_num,name,item_name,account_name,subt_nat_amount,debt_amt,credit_amt,inv_date,quantity,rate,cust_name,vend_name,emp_name,split_acc,is_ar_paid&accounting_method=Cash");
//            var accountsList = await intuitOAuthWrapper.ExecuteGet("/reports/AccountList");

//            //Load TB data for the last 2 years
//            for (int year = int.Parse(thisYear); year >= int.Parse(thisYear) - 2; --year)
//            {
//                for (int month = 1; month <= 12; month++)
//                {
//                    var lastDayOfMonth = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
//                    try
//                    {
//                        tbDataRaw.Add(await intuitOAuthWrapper.ExecuteGet($"reports/TrialBalance?start_date={year}-{month}-01&end_date={year}-{month}-{lastDayOfMonth.Day.ToString()}"));
//                    }
//                    catch (Exception e)
//                    {
//                        log.Info($"TB load error- {e.Message} ");
//                    }
//                }
//            }

//            log.Info($"Let's load our data to LTS");
//            MyCustomBlobStorage.OverWriteData(StringCleaner.Clean(glMaxMinDateDataSetRaw.ToString()), "GlMaxMinDate");
//            MyCustomBlobStorage.OverWriteData(StringCleaner.Clean(glAccrualFullDataSetRaw.ToString()), "GlAccrualFullDataSet");
//            MyCustomBlobStorage.OverWriteData(StringCleaner.Clean(glCashFullDataSetRaw.ToString()), "GlCashFullDataSet");
//            MyCustomBlobStorage.OverWriteData(StringCleaner.Clean(prefrencesRaw.ToString()), "PrefrencesDataSet");
//            MyCustomBlobStorage.OverWriteData(StringCleaner.Clean(accountsList.ToString()), "AccountsListDataSet");

//            jsonEtl.TrialBalance(tbDataRaw);

//            log.Info("Good job Kids, we did it. Shut Down and wait for the next 12 hours.");

//        }
//    }
//}
