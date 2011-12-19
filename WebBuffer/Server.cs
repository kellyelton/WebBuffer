using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace WebBuffer
{
    public static class Server
    {
        public static Socket Sock;
        public static bool Running { get { return _running; } private set { _running = value; } }
        private static bool _running;
        private static Thread _thread;
        static Server()
        {

        }
        public static void Start()
        {
            if (!Running)
            {
                EndPoint ep = new IPEndPoint(IPAddress.Any, 80);
                Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Sock.Bind(ep);
                Sock.Listen(1);
                _thread = new Thread(Listen);
                _thread.Name = "Listen Thread";
                Running = true;
                _thread.Start();
            }
        }
        public static void Stop()
        {
            Running = false;
            Sock.Close();
        }
        private static void Listen()
        {
            while (Running)
            {
                try
                {
                    Socket sc = Sock.Accept();
                    Dictionary<string, string> Headers = GetHeaders(sc);
                    if (Headers.Count == 0)
                        continue;
                    ///place?
                    if (Headers["Location"].Length > 7 && Headers["Location"].Substring(1, 6) == "place?")
                    {
                        try
                        {
                            long id = Int64.Parse(Headers["Location"].Split('?')[1]);
                            int place = ClientsBox.GetPlace(id);
                            if (place == -1)
                                RedirectToBrowser(sc);
                            else
                                WriteToBrowser(sc, place.ToString());
                        }
                        catch (Exception)
                        {
                            sc.Close();
                        }

                    }
                    else
                    {
                        long tw = ClientsBox.AddToQ();
                        if (tw == -1)
                        {
                            RedirectToBrowser(sc);
                        }
                        else
                            WriteToBrowser(sc, Program.WaitPage.Replace("%%id%%",tw.ToString() ));
                    }

                }
                catch (Exception)
                {
                    if(System.Diagnostics.Debugger.IsAttached)System.Diagnostics.Debugger.Break();
                }
                Thread.Sleep(20);
            }
        }
        public static void WriteToBrowser(Socket sc,String str)
        {
            using (NetworkStream ns = new NetworkStream(sc))
            using (StreamWriter writer = new StreamWriter(ns))
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Server: WebBuffer 1.0");
                writer.WriteLine("Content-Type: text/html");
                writer.WriteLine("");
                writer.WriteLine(str);
                writer.Flush();
            }
            sc.Close();
        }
        public static void RedirectToBrowser(Socket sc)
        {
            //HTTP/1.1 301 Moved Permanently
            //Location: http://www.example.org/
            using (NetworkStream ns = new NetworkStream(sc))
            using (StreamWriter writer = new StreamWriter(ns))
            {
                writer.WriteLine("HTTP/1.1 301 Moved Permanently");
                writer.WriteLine("Server: WebBuffer 1.0");
                writer.WriteLine("Content-Type: text/html");
                writer.WriteLine("Location: http://www.skylabsonline.com");
                writer.WriteLine("");
                writer.Flush();
            }
            sc.Close();
        }
        public static Dictionary<string,string> GetHeaders(Socket sc)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            try
            {
                using (NetworkStream ns = new NetworkStream(sc))
                using (StreamReader sr = new StreamReader(ns))
                {
                    String input = sr.ReadLine();
                    if (input == null)
                        return ret;
                    string[] tp = input.Split(' ');
                    ret.Add("Method", tp[0]);
                    ret.Add("Location", tp[1]);
                    ret.Add("HttpVersion", tp[2]);
                    input = sr.ReadLine();
                    while (!String.IsNullOrWhiteSpace(input))
                    {
                        try
                        {
                            string[] parts = input.Split(':');
                            if (parts.Length == 1)
                            {
                                ret.Add(parts[0], "");
                            }
                            else if (parts.Length == 2)
                            {
                                ret.Add(parts[0], parts[1]);
                            }
                        }
                        catch (ArgumentException)
                        {

                        }
                        input = sr.ReadLine();
                    }
                }
            }
            catch (Exception) { }
            return ret;
        }
    }
}
