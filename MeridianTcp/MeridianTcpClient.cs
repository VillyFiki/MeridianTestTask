using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            var data1 = DateTime.ParseExact(dataStr1, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);

            var dataStr2 = splitted[1].Split('#')[0];
            var data2 = DateTime.ParseExact(dataStr2, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);


            return new Response(data1, data2);
        }
    }
}
