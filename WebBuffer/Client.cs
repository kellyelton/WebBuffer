using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace WebBuffer
{
    public static class ClientsBox
    {
        public static List<Client> Clients
        {
            get { lock (_locker)return _clients; }
            set { lock (_locker)_clients = value; }
        }
        public static bool Running {get;private set; }
        private static List<Client> _clients = new List<Client>();
        private static Thread _thread;
        private static object _locker = new object();
        public static void Start()
        {
            if (_thread == null)
            {
                _thread = new Thread(PositionChecker);
                _thread.Name = "Position Checker";
                Running = true;
                _thread.Start();
            }
        }
        public static void Stop()
        {
            Running = false;
        }
        private static void PositionChecker()
        {
            int timeout = Int32.Parse(Settings.ThisSettings["usertimeout"]);
            while (Running)
            {
                if (Clients.Count > 0)
                {
                    DateTime nt = new DateTime(Clients[0].ConnectTime.Ticks);
                    if (DateTime.Now >= nt.AddSeconds(timeout * 2))
                        GetPlace(Clients[0].ID);
                }
                Thread.Sleep(50);
            }
        }
        public static long AddToQ()
        {
            long ret = DateTime.Now.ToFileTime();
            if (Clients.Count >= Int32.Parse(Settings.ThisSettings["maxusers"]))
            {
                Clients.Add(new Client(ret,Clients.Count));
                return ret;
            }
            else
            {
                Clients.Add(new Client(ret,Clients.Count));
                return -1;
            }
        }
        public static int GetPlace(long id)
        {
            try
            {
                Client c = Clients.FirstOrDefault(kp => kp.ID == id);
                if (c.Position == 0)
                {
                    Clients.Remove(c);
                    foreach (Client cl in Clients)
                        cl.Position = cl.Position - 1;
                    return -1;
                }
                else
                    return c.Position;
            }
            catch (Exception)
            {
            }
            long idd = AddToQ();
            return Clients.FirstOrDefault(kp => kp.ID == idd).Position;
        }
    }
    public class Client : System.Collections.IComparer
    {
        public long ID;
        public int Position;
        public DateTime ConnectTime;
        public Client(long id, int pos)
        {
            ID = id;
            Position = pos;
            ConnectTime = DateTime.Now;
        }

        public int Compare(object x, object y)
        {
            Client xx = x as Client;
            Client yy = y as Client;
 	        return xx.Position.CompareTo(yy.Position);
        }
}
}
