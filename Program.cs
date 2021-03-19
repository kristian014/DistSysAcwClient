using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DistSysAcwClient
{
    class Program
    {

        private static string request = "";

        public static string ApiKey { get; set; }
        public static string username { get; set; }

       
        static void Initialise()
        {
           

            Console.WriteLine("Hello. What would you like to do?");
            while (true)
            {
                request = Console.ReadLine();
                Console.Clear();
                clientViewModel.RunRequest(request).Wait();
                Console.WriteLine("Hello. What would you like to do?");
            }
        }

        
        static void Main(string[] args)
        {
            clientViewModel clientViewModel = new clientViewModel();
            Initialise();
            
        }
    }
}
