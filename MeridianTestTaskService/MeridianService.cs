using MeridianJsonExtencion;
using MeridianTcp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MeridianTestTaskService
{
    public partial class MeridianService : ServiceBase
    {
        private MeridianTcpServer _server;
        private Settings _settings;

        public MeridianService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (args.Length == 0) this.Stop();

            Task.Run(() => RunServer());
            Task.Run(() => Start(args[0]));
        }

        public async Task Start(string path)
        {
            var json = File.ReadAllText(path);
            _settings = JsonConvert.DeserializeObject<Settings>(json);



            var list = new List<Response>();

            var tasks = _settings.Client.ServersList.Select(async server =>
            {
                var tcp = new MeridianTcpClient();

                try
                {
                    await tcp.ConnectAsync(server.Ip, server.Port);

                    var response = await tcp.GetResponseAsync();


                    list.Add(response);
                }
                catch
                {
                    Console.WriteLine("Wrong response \n from server " + server.Ip + ":" + server.Port, "Error");
                }
            });

            await Task.WhenAll(tasks);

            _server.ResponseList = list;


        }

        void RunServer()
        {
            _server = new MeridianTcpServer(_settings.Server.Ip, _settings.Server.Port);

            Task.Run(() => {
                _server.Connect();
            }).ConfigureAwait(true);
        }

        protected override void OnStop()
        {
            _server.Disconnect();
        }
    }
}
