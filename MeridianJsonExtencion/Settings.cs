using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeridianJsonExtencion
{
    public class Settings
    {
        [JsonProperty("Client")]
        public Servers Client;
        [JsonProperty("Server")]
        public Server Server;
    }
    public class Servers
    {
        [JsonProperty("Servers")]
        public Server[] ServersList { get; set; }
    }
    public class Server
    {
        [JsonProperty("Ip")]
        public string Ip { get; set; }
        [JsonProperty("Port")]
        public int Port { get; set; }
    }
}
