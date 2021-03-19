using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace DistSysAcwClient
{
    class clientViewModel
    {
        private static HttpClient mClient = new HttpClient();
        private const string mUrl = "https://localhost:44394/api/";

        public clientViewModel()
        {
            // default constructor 
            RunAsync().Wait();

        }

        public static async Task RunRequest(string request)
        {
            if (!string.IsNullOrEmpty(request))
            {
                string[] args = request.Split(' ');

                if (args.Length == 2)
                    {
                        // get the values of the strings and make request
                        string requestValues = args[0] + " " + args[1];
                        var splitedValue = "";
                        splitedValue = args[1];


                        switch (requestValues)
                        {
                            case "TalkBack Hello":
                                Hello(splitedValue).Wait();
                                break;
                        }
                    }
           
                if (args.Length == 3)
                {
                    var endpoint = "integers=";
                        // get the values of the strings and make request
                        string requestValues = args[0] + " " + args[1] + " " + args[2];
                        if (requestValues.Contains("Sort") && requestValues.Contains("TalkBack"))
                        {

                            string[] parsed = args[2].Split(',');

                            int i = 0;
                            foreach (var integer in parsed)
                            {
                                string trimmedValue = integer.Trim(' ', '[', ']', ',');
                                
                                if (i++ == 0)
                                {
                                    endpoint += trimmedValue;
                                }
                                else
                                {
                                    endpoint += "&integers=" + trimmedValue;
                                }

                            }
                            Sort(endpoint).Wait();
                    }
                      

                }

            }

        }


        static async Task Hello(string args)
       {
           var response = mClient.GetAsync("talkback/" + args);
           var result = await response.Result.Content.ReadAsStringAsync();
           Console.WriteLine(result);
       }

       static async Task Sort(string args)
       {
           var endpoint = mClient.GetAsync("talkback/Sort?" + args);
           var response = await endpoint.Result.Content.ReadAsStringAsync();
           Console.WriteLine(response);
       }

        static async Task RunAsync()
       {
           mClient.BaseAddress = new Uri(mUrl);
       }

    }
}
