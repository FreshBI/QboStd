using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Fbi.Std.Core;
using Fbi.Std.Qbo.Core;
using Newtonsoft.Json.Linq;

namespace Fbi.Std.Qbo.Pheidippides
{
    public static class QboExecutioner
    {
        [FunctionName("QboExecutioner")]
        public static async void Run([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            log.Info($"So you finally got me to run, eh?");
            log.Info($"Let's get check everything is set up...");


            //Check for DBs and tables
            log.Info($"Look for DBs and tables");
            log.Info($"Error- Not implemented yet, check for another version later. Skipping step");
            log.Info($"Look for RefreshTokens and Company IDs");
            log.Info($"Error- Not implemented yet, check for another version later. Skipping step");

            //Return a bool and switch for first time run
            log.Info($"Look's like we've done this before, let's try to get a custom client from the Qbo.Core and load some data");
            try
            {
                //Loading data
                var intuitOAuthWrapper = new QboClientWrapper();
                log.Info($"Got our Client");

                log.Info($"Loading Data into memory");
                var prefrencesRaw = await intuitOAuthWrapper.ExecuteGet("preferences");
                var glMaxMinDateDataSetRaw = await intuitOAuthWrapper.ExecuteGet("/reports/GeneralLedger?date_macro=all&columns=tx_date&sort_order=ascend");
                var glAccrualFullDataSetRaw = await intuitOAuthWrapper.ExecuteGet("/reports/GeneralLedger?date_macro=all&columns=tx_date,txn_type,doc_num,name,item_name,account_name,subt_nat_amount,debt_amt,credit_amt,inv_date,quantity,rate,cust_name,vend_name,emp_name,split_acc,is_ar_paid&accounting_method=Accrual");
                var glCashFullDataSetRaw = await intuitOAuthWrapper.ExecuteGet("/reports/GeneralLedger?date_macro=all&columns=tx_date,txn_type,doc_num,name,item_name,account_name,subt_nat_amount,debt_amt,credit_amt,inv_date,quantity,rate,cust_name,vend_name,emp_name,split_acc,is_ar_paid&accounting_method=Cash");
                var tbFullDataSetRaw = await intuitOAuthWrapper.ExecuteGet("reports/TrialBalance");
                log.Info($"Data in memeory, let's do something with it...");

                log.Info($"Parsing some of our Data to Jarrays");
                JArray tbParsedJsonRaw = JArray.Parse(tbFullDataSetRaw.Rows.Row.ToString());
                JArray glMaxMinDateParsedJsonRaw = JArray.Parse(glMaxMinDateDataSetRaw.Rows.Row.ToString());
                JArray glFullDataParsedJsonRaw = JArray.Parse(glAccrualFullDataSetRaw.Rows.Row.ToString());
                log.Info($"Required data parsed");

                log.Info($"Let's load our data to LTS");
                //QboJsonEtl.TrialBalance(tbParsedJsonRaw);
                log.Info($"TB Data Load Failed");
                MyCustomBlobStorage.OverWriteData(StringCleaner.Clean(glMaxMinDateDataSetRaw.ToString()), "GlMaxMinDate");
                MyCustomBlobStorage.OverWriteData(StringCleaner.Clean(glAccrualFullDataSetRaw.ToString()), "GlAccrualFullDataSet");
                MyCustomBlobStorage.OverWriteData(StringCleaner.Clean(glCashFullDataSetRaw.ToString()), "GlCashFullDataSet");
                MyCustomBlobStorage.OverWriteData(StringCleaner.Clean(prefrencesRaw.ToString()), "PrefrencesDataSet");

                log.Info("Good job Kids, we did it. Shut the Fuck Down and wait for the next 12 hours.");
                
            }
            catch ( Exception e )
            {
                log.Info($"Error- {e.Message} ");
            }
        }
    }
}
