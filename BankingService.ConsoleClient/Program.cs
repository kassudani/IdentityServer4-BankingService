using IdentityModel.Client;

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankingService.ConsoleClient
{
    class Program
    {
        static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            //for resource owner
            //get the discovery of all end points using resource pwner password Grant type
            var discoveryRO = await  DiscoveryClient.GetAsync("http://localhost:5000");
            if (discoveryRO.IsError)
            {
                Console.WriteLine(discoveryRO.Error);
                return;
            }

            // grabbing bearer token using resource pwner password Grant type
            var tokenClientRO = new TokenClient(discoveryRO.TokenEndpoint, "ro.client", "secret");
            var tokenResponseRO = await tokenClientRO.RequestResourceOwnerPasswordAsync("Kajol", "password", "bankingService");

            if (tokenResponseRO.IsError)
            {
                Console.WriteLine(tokenResponseRO.Error);
                return;
            }

            Console.WriteLine(tokenResponseRO.Json);
            Console.WriteLine("\n\n");





            //for client credentials
            //get the discovery of all end points
            var discovery = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (discovery.IsError)
            {
                Console.WriteLine(discovery.Error);
                return;
            }

            // grabbing bearer token
            var tokenClient = new TokenClient(discovery.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("bankingService");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");


            //consuming our customer api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var customerInfo = new StringContent(
                JsonConvert.SerializeObject(
                        new {Id = 2, Name = "Kajol", Address = "qwert asdf zxcv"}
                    ),
                    Encoding.UTF8,"application/json"
                );

            var createCustomerResponse = await client.PostAsync("http://localhost:51466/api/customers", customerInfo);

            if (!createCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(createCustomerResponse.StatusCode);
            }

            var getCustomerResponse = await client.GetAsync("http://localhost:51466/api/customers");

            if (!getCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(getCustomerResponse.StatusCode);
            }
            else
            {
                var content = await getCustomerResponse.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.Read();

        }
    }
}
