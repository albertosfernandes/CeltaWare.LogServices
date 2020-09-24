using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CeltaWare.MonitorLogServices
{
    public class Library
    {
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

        public static bool Validate()
        {
            try
            {
                string url = Properties.Settings.Default.UrlValidaTeste;

                // Creates an HttpWebRequest for the specified URL.
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                // Sends the HttpWebRequest and waits for a response.
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                myHttpWebRequest.Timeout = 30000;
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    myHttpWebResponse.Close();
                    return true;
                }
                else
                {
                    //Vou tentar mais uma vez .. vai que volte nesse meio tempo!!
                    System.Threading.Thread.Sleep(30000);
                    myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                    if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //opa deu certo !!
                        myHttpWebResponse.Close();
                        return true;
                    }
                    
                    myHttpWebResponse.Close();
                    return false;
                }
            }
            catch(Exception err)
            {
                Log(Properties.Settings.Default.NomeServico, err.Message);
                return false;
            }                                   
        }
    }
}
