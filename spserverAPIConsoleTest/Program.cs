using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace spserverAPIConsoleTest
{
    class Program
    {
        // You will need to substitute your own host Url here:
        static string host = "http://localhost:53848/";

        static void Main(string[] args)
        {
            // Use the User Names/Emails and Passwords we set up in IdentityConfig:
            string adminUserName = "spsAdmin@itsligo.ie";
            string adminUserPassword = "Admin$1";

            string vanillaUserName = "tutor1@mail.itsligo.ie";
            string vanillaUserPassword = "Tutor$1";

            // Use the new GetToken method to get a token for each user:
            string adminUserToken = GetToken(adminUserName, adminUserPassword);
            //string vaniallaUserToken = GetToken(vanillaUserName, vanillaUserPassword);

            // Try to get some data as an Admin:
            Console.WriteLine("Attempting to get User info as Admin User");
            string adminUserInfoResult = GetUserInfo(adminUserToken);
            Console.WriteLine("Admin User Info Result: {0}", adminUserInfoResult);
            Console.WriteLine("");

            //Console.WriteLine("Attempting to get Values info as Admin User");
            //string adminValuesInfoResult = GetValues(adminUserToken);
            //Console.WriteLine("Admin Values Info Result: {0}", adminValuesInfoResult);
            //Console.WriteLine("");

             //Try to get some data as a plain old user:
            //Console.WriteLine("Attempting to get User info as Vanilla User");
            //string vanillaUserInfoResult = GetUnregistered(vaniallaUserToken);
            //Console.WriteLine("Vanilla User Info Result: {0}", vanillaUserInfoResult);
            //Console.WriteLine("");

            //Console.WriteLine("Attempting to get Values info as Vanilla User");
            //string vanillaValuesInfoResult = GetValues(vaniallaUserToken);
            //Console.WriteLine("Vanilla Values Info Result: {0}", vanillaValuesInfoResult);
            //Console.WriteLine("");

            Console.Read();
        }

        private static string GetUnregistered(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(host + "api/Account/GetUnauthorised").Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }


        static string GetToken(string userName, string password)
        {
            HttpClient client = new HttpClient();
            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ), 
                        new KeyValuePair<string, string>( "username", userName ), 
                        new KeyValuePair<string, string> ( "Password", password )
                    };
            var content = new FormUrlEncodedContent(pairs);

            // Attempt to get a token from the token endpoint of the Web Api host:
            HttpResponseMessage response =
                client.PostAsync(host + "Token", content).Result;
            var result = response.Content.ReadAsStringAsync().Result;

            // De-Serialize into a dictionary and return:
            Dictionary<string, string> tokenDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            return tokenDictionary["access_token"];
        }


        static string GetUserInfo(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(host + "api/Account/UserInfo").Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }


        static string GetValues(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = client.GetAsync(host + "api/Values").Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
