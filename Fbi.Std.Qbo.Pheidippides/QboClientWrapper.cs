using Fbi.Std.Qbo.Core;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fbi.Std.Qbo.Pheidippides
{
    /// <summary>  
    ///  This wrapper inserts all the necessary components from a SetUpObject into a QBO Resource Proivder and stands up a connection to QBO
    /// </summary> 
    public class QboClientWrapper
    {
        ResouceProvider resourceProvider;

        private string _companyID;
        private string _baseURI;
        //string CompanyId = "123145774537644";
        // string BaseURI = "https://sandbox-quickbooks.api.intuit.com";

        /// <summary>
        /// The class constructor. </summary>
        /// <param name="CompanyID">This is the Company ID. It is specific to a users's company.</param>
        /// <param name="BaseURI">Base URI, SandBox id different from Live</param>
        /// <param name="qboRefreshTokenStorage">How are we storing and retrieving the Refresh Token?</param>
        public QboClientWrapper(string CompanyID, string BaseURI, QboRefreshTokenStorage qboRefreshTokenStorage)
        {
            var authClient = new IntuitAuthClient(qboRefreshTokenStorage);
            _companyID = CompanyID;
            _baseURI = BaseURI;
            resourceProvider = new ResouceProvider(authClient, _baseURI);
        }

        public async Task<dynamic> ExecuteGet(string ResourcePart)
        {
            var response = await resourceProvider.GetResourceData(_companyID, ResourcePart);

            return response;
        }

        class ResouceProvider : IntuitResourceProvider
        {
            public ResouceProvider(IntuitAuthClient authClient, string baseURI)
              : base(new HttpClient() { BaseAddress = new Uri(baseURI) }, authClient) 
            {
            }

            public async Task<dynamic> GetResourceData(string companyId, string resourcePart)
            {
                var getResourceResponse = await SendAsync(HttpMethod.Get, $"v3/company/{ companyId }/{ resourcePart }");

                //Console.WriteLine(await getResourceResponse.Content.ReadAsStringAsync());

                return await getResourceResponse.Content.ReadAsAsync<dynamic>();
            }
        }

    }
}
