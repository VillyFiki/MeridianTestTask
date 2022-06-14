using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MeridianTcp
{
    public class MeridianTcpServer
    {
        TcpListener Listener;

        public IEnumerable<Response> ResponseList;

        public delegate void DisconnectedEventHandler();
        public event DisconnectedEventHandler OnDisconnected;

        public MeridianTcpServer(string ip, int Port)
        {
            Listener = new TcpListener(IPAddress.Parse(ip), Port);
        }

        public void Connect()
        {
            Listener.Start();

            while (true)
            {
                TcpClient client = Listener.AcceptTcpClient();
                new Client(client, CreateResponse(ResponseList));
            }
        }
        string CreateResponse(IEnumerable<Response> responseList)
        {
            if (responseList == null || !responseList.Any()) return String.Empty;
            var data1 = responseList.Any(x => x.Data1 != responseList.First().Data1) ? "NoRead" : responseList.First().Data1.ToString("MMddyy");
            var data2 = responseList.Any(x => x.Data2 != responseList.First().Data2) ? "NoRead" : responseList.First().Data2.ToString("MMddyy");

            return $"#90#010102#27{data1};{data2}#91";
        }

        public void Disconnect()
        {
            OnDisconnected.Invoke();
            if (Listener != null)
            {
                Listener.Stop();
            }
        }

        ~MeridianTcpServer()
        {
            Disconnect();
        }
    }

    class Client
    {
        public Client(TcpClient client, string message)
        {
            Console.WriteLine("New Client");
            string str = message;

            byte[] Buffer = Encoding.ASCII.GetBytes(str);
            var stream = client.GetStream();
            stream.Write(Buffer, 0, Buffer.Length);

            stream.Close();
            client.Close();
        }
    }
}
