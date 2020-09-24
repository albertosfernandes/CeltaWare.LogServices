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
        public Service1()
        {
            InitializeComponent();
        }

        private System.Timers.Timer timer1 = null;

        protected override void OnStart(string[] args)
        {
            string serviceName = Properties.Settings.Default.NomeServico;
            int _interval = Properties.Settings.Default.IntervaloAtualizacao;
            _interval *= 60;
            _interval *= 1000;
            timer1 = new System.Timers.Timer();
            timer1.Interval = _interval;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timer1.Enabled = true;
            Library.Log(serviceName, ": Monitor inicializado!");
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            string serviceName = Properties.Settings.Default.NomeServico;
            string error = null;
            //Inicia aqui as verificações
            if (!Library.Validate())
            {
                Library.Log(serviceName, ": Erro na validação da url");
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
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            Library.Log(Properties.Settings.Default.NomeServico, ": Monitor de serviço parado.");
        }
    }
}
