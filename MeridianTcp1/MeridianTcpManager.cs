using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MeridianTcp
{
    public class MeridianTcpClient
    {
        public TcpClient _client;
        public async Task ConnectAsync(string ip, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);

            
        }

        public async Task<Response> GetResponseAsync()
        {

            NetworkStream stream = _client.GetStream();

            var data = new Byte[256];

            string response = string.Empty;

            Int32 bytes = await stream.ReadAsync(data, 0, data.Length);
            response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            stream.Close();
            _client.Close();


            var splitted = response.Split(';');

            var dataStr1a = splitted[0].Split('#')[3];
            var dataStr1 = dataStr1a.Substring(2);
            string[] formats = new string[] { "MMddyy", "MMddyyyy" };
            var data1 = DateTime.ParseExact(dataStr1, formats, CultureInfo.InvariantCulture);

            var dataStr2 = splitted[1].Split('#')[0];
            var data2 = DateTime.ParseExact(dataStr2, formats, CultureInfo.InvariantCulture);


            return new Response(data1, data2);
        }
    }

    public class MeridianTcpServer
    {
        TcpListener Listener; // Объект, принимающий TCP-клиентов

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
            if(responseList == null || !responseList.Any()) return String.Empty;
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

    public class Response
    {
        public DateTime Data1;
        public DateTime Data2;

        public Response(DateTime data1, DateTime data2)
        {
            Data1 = data1;
            Data2 = data2;
        }
    }
}
