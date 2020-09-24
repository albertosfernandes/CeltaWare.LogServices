using CeltaWare.MonitorLogServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CeltaWare.LogServices
{
    class Program
    {
        static void Main(string[] args)
        {
            var customer = Properties.Settings.Default.Customer;
            var serviceName = Properties.Settings.Default.ServiceName;
            int interval = Properties.Settings.Default.TimeInterval;
            interval *= 60;
            interval *= 1000;
            string error = null;
            string result;
            Log(serviceName, "Inicio");
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

        public static void Log(string _serviceName, string _message)
        {
            StreamWriter sw = null;
            sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile" + _serviceName + ".txt", true);
            sw.WriteLine(DateTime.Now.ToString() + ": " + _serviceName + "- " + _message);
            sw.Flush();
            sw.Close();
        }

        public static string Restart(string _verb, string _serviceName, out string error)
        {
            string message = " ";
            error = null;
            using (Process p1 = new Process())
            {
                p1.StartInfo.FileName = @"c:\Windows\system32\net.exe ";
                p1.StartInfo.Arguments = _verb + " " + _serviceName;
                p1.StartInfo.CreateNoWindow = true;
                p1.StartInfo.UseShellExecute = false;
                p1.StartInfo.RedirectStandardOutput = true;
                p1.StartInfo.RedirectStandardError = true;
                p1.StartInfo.WorkingDirectory = @"c:\Windows\system32\";

                p1.Start();
                
                if (p1.Start())
                {
                    message = p1.StandardOutput.ReadToEnd();
                    error = p1.StandardError.ReadToEnd();
                }
                return message;
            }
        }
    }
}
