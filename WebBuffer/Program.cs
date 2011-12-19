using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WebBuffer
{
    public static class Program
    {
        public static String WaitPage;
        static void Main(string[] args)
        {
            Console.WriteLine("Loading wait.html");
            try
            {
                WaitPage = File.ReadAllText("wait.html");
            }
            catch (IOException)
            {
                Console.WriteLine("Failed to open wait.html. Exiting.");
                goto Done;
            }
            Console.WriteLine("Starting Server.");
            Server.Start();
            ClientsBox.Start();
            while (Server.Running)
            {
                String s = Console.ReadLine();
                if (s == "quit")
                {
                    Server.Stop();
                    ClientsBox.Stop();
                }
            }
            Done:
            Console.WriteLine("Server quit.");
            Console.ReadKey();
        }
    }
}
