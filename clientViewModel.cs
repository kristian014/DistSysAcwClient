using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;


namespace DistSysAcwClient
{
    class clientViewModel
    {
        private static HttpClient mClient = new HttpClient();
        private const string mUrl = "https://localhost:44394/api/";

        public static string ApiKey { get; set; }
        public static string mUsername { get; set; }

        public static string mPublicKey { get; set; }

        public clientViewModel()
        {
            // default constructor 
            RunInit();

        }


        public static Task RunRequest(string request)
        {
           
            if (!string.IsNullOrEmpty(request))
            {

                if (request == "Exit")
                {
                    Environment.Exit(0);
                }
                string[] args = request.Split(' ');

                if (args.Length == 2)
                {
                    // get the values of the strings and make request
                    string requestValues = args[0] + " " + args[1];
                    var splitedValue = "";
                    splitedValue = args[1];

                    // Task 10 query 1 and 6
                    switch (requestValues)
                    {
                        case "TalkBack Hello":
                            Console.WriteLine("…please wait…");
                            Hello(splitedValue).Wait();
                            break;
                        case "User Delete":
                            Console.WriteLine("…please wait…");
                            DeleteUser().Wait();
                            break;
                            default:Console.WriteLine("Unknown Command passed");
                                break;
                    }
                }
                    // Task 10 query 2
                else if (args.Length == 3)
                {
                    var endpoint = "integers=";
                    if (args[1].Contains("Sort") && args[0].Contains("TalkBack"))
                    {
                        Console.WriteLine("…please wait…");
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

                    // Task 10 query 3
                   else if (args[0].Contains("User") && args[1].Equals("Get"))
                    {
                        Console.WriteLine("…please wait…");
                        string username = args[2];
                        if (!string.IsNullOrWhiteSpace(username))
                        {
                            GetUserName(username).Wait();
                        }
                    }

                    // Task 10 query 4
                   else if (args[0].Contains("User") && args[1].Equals("Post"))
                    {
                        Console.WriteLine("…please wait…");
                        string username = args[2];
                        if (!string.IsNullOrWhiteSpace(username))
                        {
                            // convert to a json object 
                           // var jsonUsername = await JsonConvert.SerializeObjectAsync(username);
                          var jsonUsername =   JsonConvert.SerializeObject(username);
                            CreateUser(jsonUsername).Wait();
                        }

                    }

                    // task 10 query 8
                    else if (args[0].Equals("Protected") && args[1].Equals("SHA1") && args[2].Equals("Hello"))
                    {
                        Console.WriteLine("…please wait…");
                        string apiKey = GetApiKey();

                        if (string.IsNullOrWhiteSpace(apiKey))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first.");
                        }

                        else
                        {
                            string requestMessage = args[2];
                            ProtectedRequestSha1(apiKey, requestMessage).Wait();
                        }

                    }

                    // Task 10 Query 9
                    else if(args[0].Equals("Protected") && args[1].Equals("SHA256") && args[2].Equals("Hello"))
                    {
                        Console.WriteLine("…please wait…");
                        string apiKey = GetApiKey();
                     
                        if (string.IsNullOrWhiteSpace(apiKey))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first.");
                        }

                        else
                        {
                            string requestMessage = args[2];  
                            ProtectedRequest(apiKey, requestMessage).Wait();
                        }

                    }

                    // Task 10 Query 10
                    else if (args[0].Equals("Protected") && args[1].Equals("Get") && args[2].Equals("PublicKey"))
                    {
                        Console.WriteLine("…please wait…");

                        string apiKey = GetApiKey();

                        if (string.IsNullOrWhiteSpace(apiKey))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first.");
                        }

                        else
                        {
                            
                            PublicKey(apiKey).Wait();
                        }

                    }

                    else if (args[0].Equals("Protected") && args[1].Equals("Sign") && args[2].Equals("Hello"))
                    {
                        Console.WriteLine("…please wait…");
                        string apiKey = GetApiKey();
                        string publicKey = GetPublicKey();

                        if (string.IsNullOrWhiteSpace(apiKey))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first.");
                        }

                        if (String.IsNullOrWhiteSpace(publicKey))
                        {
                            Console.WriteLine("Client doesn’t yet have the public key");
                        }

                        else
                        {
                            string requestMessage = args[2];
                            ProtectedSignInRequest(apiKey, requestMessage).Wait();
                        }
                    }

                    else
                    {
                        Console.WriteLine("Unknown Command with three arguments passed");
                    }
                    
                }

                // Task 10 Query 5
                else if (args.Length == 4)
                {
                    if (args[0].Equals("User") && args[1].Equals("Set"))
                    {
                        Console.WriteLine("…please wait…");
                        SetUsername(args[2]);
                        SetApiKey(args[3]);
                        Console.WriteLine("Stored");
                    }

                    // Task 10 Query 7
                    else if (args[0].Equals("User") && args[1].Equals("Role") && args[3].Equals("Admin"))
                    {
                        Console.WriteLine("…please wait…");
                        string apiKey = GetApiKey();
                        string userName = getUsername();

                        if (string.IsNullOrWhiteSpace(apiKey) && string.IsNullOrWhiteSpace(userName))
                        {
                            Console.WriteLine("You need to do a User Post or User Set first.");
                        }

                        else
                        {
                            string role = args[3];
                            string body = "{\"username\":\"" + userName + "\", \"role\":\"" + role + "\"}";
                            // call the function to change the user role 
                            ChangeRole(apiKey, body).Wait();
                        }


                    }

                    else
                    {
                        Console.WriteLine("Unknown Request. Try creating a request with the right arguments ");
                    }
                }

                else
                {
                    Console.WriteLine("Unknown Request. Try creating a request with the right arguments ");
                }

            }

            return Task.CompletedTask;
        }

        static void SetUsername(string pUserName)
        {
            mUsername = pUserName;
        }

        static void SetApiKey(string pApiKey)
        {
            ApiKey = pApiKey;
        }

        static void SetPublicKey(string pPublicKey)
        {
            mPublicKey = pPublicKey;
        }

        static string GetPublicKey()
        {
            return mPublicKey;
        }


        static string GetApiKey()
        {
            return ApiKey;
        }


        static string getUsername()
        {
            return mUsername;
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

       static async Task GetUserName(string userName)
       {
           var endpoint = mClient.GetAsync("user/new?username=" + userName);
           var response = await endpoint.Result.Content.ReadAsStringAsync();
           Console.WriteLine(response);
       }

       static async Task CreateUser(string userName)
       {
           try
           {
               var dataContent = new StringContent(userName, Encoding.UTF8, "application/json");
               var httpResponseMessage = await mClient.PostAsync("user/new", dataContent);
               var result = await httpResponseMessage.Content.ReadAsStringAsync();


               if (httpResponseMessage.IsSuccessStatusCode)
               {
                   Console.WriteLine("Got API Key");
                  
               }
               else
               {
                   Console.WriteLine(result);
               }
            }
           catch (Exception e)
           {
               Console.WriteLine("Error creating user. Try a correct format e.g User Post UserOne " + e.Message);
              
           }
          
          
       }

       static async Task DeleteUser()
       {
           try
           {
               string apiKey = GetApiKey();
               string userName = getUsername();

               if (string.IsNullOrWhiteSpace(apiKey) && string.IsNullOrWhiteSpace(userName))
               {
                    Console.WriteLine("You need to do a User Post or User Set first.");
               }
               else
               {
                   if (mClient.DefaultRequestHeaders.Contains("ApiKey"))
                   {
                       // remove the apikey from the header 
                       mClient.DefaultRequestHeaders.Clear();

                   }
                    // add the api key to the header request
                    mClient.DefaultRequestHeaders.Add("ApiKey" , apiKey);
                   // check if the header contains a previous key.. If it does, remove the apiKey from the header
                   var httpResponseMessage = await mClient.DeleteAsync("user/removeuser?username=" + userName);
               

                   if (httpResponseMessage.IsSuccessStatusCode)
                   {
                       Console.WriteLine("True");
                   }
                   else
                   {
                       Console.WriteLine("False");
                   }
                }
           }
           catch (Exception e)
           {
               Console.WriteLine(e);
               throw;
           }
       }

       static async Task ChangeRole(string apiKey, string bodyRequest)
       {
           if (mClient.DefaultRequestHeaders.Contains("ApiKey"))
           {
               // remove the apikey from the header 
               mClient.DefaultRequestHeaders.Clear();

            }
            // add the api key to the header request
            mClient.DefaultRequestHeaders.Add("ApiKey", apiKey);
            var dataContent = new StringContent(bodyRequest, Encoding.UTF8, "application/json");
           var httpResponseMessage = await mClient.PostAsync("user/changerole", dataContent);
           var result = await httpResponseMessage.Content.ReadAsStringAsync();
          
            if (httpResponseMessage.IsSuccessStatusCode)
           {
               Console.WriteLine(result);
           }
           else
           {
               Console.WriteLine(result);
           }

        }

       static async Task ProtectedRequest(string apiKey, string requestMessage)
       {
           if (mClient.DefaultRequestHeaders.Contains("ApiKey"))
           {
               // remove the apikey from the header 
               mClient.DefaultRequestHeaders.Clear();

           }
           // add the api key to the header request
           mClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

           var httpResponseMessage = mClient.GetAsync("protected/sha256?message=" + requestMessage);

           var response = await httpResponseMessage.Result.Content.ReadAsStringAsync();

           if (httpResponseMessage.Result.IsSuccessStatusCode)
           {
               Console.WriteLine(response);
            }
           else
           {
               Console.WriteLine(response);
           }
           
        }

       static async Task ProtectedSignInRequest(string apiKey, string requestMessage)
       {
           if (mClient.DefaultRequestHeaders.Contains("ApiKey"))
           {
               // remove the apikey from the header 
               mClient.DefaultRequestHeaders.Clear();

           }

           // add the api key to the header request
           mClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

           var httpResponseMessage = mClient.GetAsync("protected/sign?message=" + requestMessage);

           var response = await httpResponseMessage.Result.Content.ReadAsStringAsync();

           if (httpResponseMessage.Result.IsSuccessStatusCode)
           {
               Console.WriteLine(response);
           }
           else
           {
               Console.WriteLine(response);
           }
        }

       static async Task ProtectedRequestSha1(string apiKey, string requestMessage)
       {
           if (mClient.DefaultRequestHeaders.Contains("ApiKey"))
           {
               // remove the apikey from the header 
               mClient.DefaultRequestHeaders.Clear();

           }
           // add the api key to the header request
           mClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

           var httpResponseMessage = mClient.GetAsync("protected/sha1?message=" + requestMessage);

           var response = await httpResponseMessage.Result.Content.ReadAsStringAsync();

           if (httpResponseMessage.Result.IsSuccessStatusCode)
           {
               Console.WriteLine(response);
           }
           else
           {
               Console.WriteLine(response);
           }

        }

        static async Task PublicKey(string apiKey)
       {
           if (mClient.DefaultRequestHeaders.Contains("ApiKey"))
           {
               // remove the apikey from the header 
               mClient.DefaultRequestHeaders.Clear();

           }
           // add the api key to the header request
           mClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

           var httpResponseMessage = mClient.GetAsync("protected/getpublickey");

           var response = await httpResponseMessage.Result.Content.ReadAsStringAsync();

           if (httpResponseMessage.Result.IsSuccessStatusCode)
           {
               SetPublicKey(response);
               Console.WriteLine("Got Public Key");
           }
           else
           {
               Console.WriteLine("Couldn’t Get the Public Key");
           }
        }

      

        static void RunInit()
       {
           mClient.BaseAddress = new Uri(mUrl);
       }

    }
}
