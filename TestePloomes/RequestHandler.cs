using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Collections.Generic;
using System.Text;

namespace TestePloomes
{
    class RequestHandler
    {
        private static string PLOOMES_API_PATH = "https://api2.ploomes.com/";
        private static HttpClient ploomesClient;
        private static string uk = "BA104C4B382F621B7F293F9FF3E49A168B68FFA7BA34B6860A157EB6C79EE6D06A942AC43B040C3AFE39796594005FC299A25A4477E3B5E3A770848066AE9678";

        public static void instantiatePloomesConnection()
        {
            ploomesClient = new HttpClient();
            ploomesClient.DefaultRequestHeaders.Add("User-Key", uk);
            ploomesClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static JArray MakePloomesRequest(string url, Method method, JObject json = null)
        {
            try
            {
                instantiatePloomesConnection();

                System.Threading.Thread.Sleep(1000);
                string response = string.Empty;
                url = PLOOMES_API_PATH + url;

                if (method == Method.GET)
                    response = ploomesClient.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                else if (method == Method.POST)
                {
                    if (json != null)
                        response = ploomesClient.PostAsync(url, new StringContent(json.ToString())).Result.Content.ReadAsStringAsync().Result;
                    else
                        response = ploomesClient.PostAsync(url, new StringContent(new JObject().ToString())).Result.Content.ReadAsStringAsync().Result;
                }
                else if (method == Method.DELETE)
                {
                    ploomesClient.DeleteAsync(url);
                    return null;
                }
                else if (method == Method.PATCH)
                {
                    var content = new ObjectContent<JObject>(json, new JsonMediaTypeFormatter());
                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = content };
                    response = ploomesClient.SendAsync(request).Result.Content.ReadAsStringAsync().Result;

                    Console.WriteLine(response.ToString());
                }

                return JsonConvert.DeserializeObject<JObject>(response)["value"] as JArray;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error in MakePloomesRequest method --- " + e.Message);
                throw e;
            }
        }
    }
}
