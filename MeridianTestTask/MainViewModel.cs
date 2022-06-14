using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MeridianJsonExtencion;
using MeridianTcp;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace MeridianTestTask
{
    internal class MainViewModel : ViewModel
    {
        private MeridianTcpServer _server;
        public Command ConnectCommand { get; }
        public Command ReadJsonCommand { get; }

        public Settings Settings { get; private set; }

        public string Result { 
            get => _result; 
            set 
            { 
                _result = value; SetProperty(_result); 
            } 
        }
        private string _result;

        public bool IsJsonLoaded
        {
            get => _isJsonLoaded;
            set
            {
                _isJsonLoaded = value; SetProperty(_isJsonLoaded);
            }
        }
        private bool _isJsonLoaded = false;

        public MainViewModel()
        {
            ConnectCommand = new Command(Connect);
            ReadJsonCommand = new Command(ReadJson);
        }

        private async void Connect()
        {
            var list = new List<Response>();

            var tasks = Settings.Client.ServersList.Select(async server =>
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
                    MessageBox.Show("Wrong response \n from server " + server.Ip + ":" + server.Port, "Error");
                }
            });

            await Task.WhenAll(tasks);

            _server.ResponseList = list;
        }

        public void SendResponse()
        {

        }

        private void ReadJson()
        {
            try
            {
                var json = File.ReadAllText(OpenFileExplorer());
                Settings = JsonConvert.DeserializeObject<Settings>(json);

                IsJsonLoaded = true;
            }
            catch
            {
                MessageBox.Show("Error when reading the settings file");
            }

            if (!_isServerConnected)
            {
                RunServer();
            }
        }

        private bool _isServerConnected = false;
        void RunServer()
        {
            _isServerConnected = true;
            _server = new MeridianTcpServer(Settings.Server.Ip, Settings.Server.Port);
            _server.OnDisconnected += () => 
            { 
                _isServerConnected = false; 
            };

            Task.Run(() => {
                _server.Connect();
            }).ConfigureAwait(true);
        }

        private string OpenFileExplorer()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().Value)
                return openFileDialog.FileName;
            else return null;
        }
    }
}
