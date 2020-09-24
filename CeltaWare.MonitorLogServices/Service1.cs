using CeltaWare.UtilitariosInfra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace CeltaWare.MonitorLogServices
{
    public partial class Service1 : ServiceBase
    {
        private string serviceName = Properties.Settings.Default.NomeServico;
        private int _interval = Properties.Settings.Default.IntervaloAtualizacao;
        private UtilTelegram telegramApp = new UtilTelegram(Properties.Settings.Default.UidTelegramToken);
        private Timer timer1 = null;

        public Service1()
        {
            InitializeComponent();
        }
        
        protected override void OnStart(string[] args)
        {
            _interval *= 60;
            _interval *= 1000;
            timer1 = new System.Timers.Timer();
            timer1.Interval = _interval;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timer1.Enabled = true;
            Library.Log(serviceName, ": Monitor inicializado!");
            if (String.IsNullOrEmpty(Properties.Settings.Default.UidTelegramToken))
            {
                Library.Log(serviceName, "Token não configurado no arquivo .config. Não será enviada mensagem Telegram.");
            }
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            string serviceName = Properties.Settings.Default.NomeServico;
            string serverName = Properties.Settings.Default.NomeServidor;
            string destinationID = Properties.Settings.Default.UidTelegramDestino;            
            string error = null;
            //Inicia aqui as verificações
            if (!Library.Validate())
            {
                RegisterEvent(serviceName, serverName, destinationID, "Falha conexão VPN");

                //deu ruim manda reiniciar
                while (!Library.Validate())
                {
                    Library.Restart("stop", serviceName, out error);
                    if (!String.IsNullOrEmpty(error))
                        Library.Log(serviceName, error);

                    System.Threading.Thread.Sleep(10000);

                    Library.Restart("start", serviceName, out error);
                    if (!String.IsNullOrEmpty(error))
                        Library.Log(serviceName, error);

                    System.Threading.Thread.Sleep(10000);
                }
            }
            RegisterEvent(serviceName, serverName, destinationID, "Conexão VPN Reestabelecida");
        }

        private void RegisterEvent(string serviceName, string serverName, string destinationID, string message)
        {
            Library.Log(serviceName, $": {message}");
            if (String.IsNullOrEmpty(Properties.Settings.Default.UidTelegramToken) || String.IsNullOrEmpty(Properties.Settings.Default.UidTelegramDestino))
            {
                Library.Log(serviceName, "Configurações Telegram não encontradas no arquivo config.");
            }
            else
            {
                telegramApp.SendMessage($"Servidor:{serverName},\nServiço:{serviceName}\n{message}.", destinationID);
            }
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            Library.Log(Properties.Settings.Default.NomeServico, ": Monitor de serviço parado.");
        }
    }
}
