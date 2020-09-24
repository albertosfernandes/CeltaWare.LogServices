using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaWare.UtilitariosInfra
{
    public class UtilTelegram
    {
        private string token;
        public UtilTelegram(string _token)
        {
            this.token = _token;
        }

        public void SendMessage(string message, string destinationId)
        {
            try
            {
                send(message, destinationId);
            }
            catch(Exception err)
            {
                throw err;
            }            
        }         
        
        private void send(string text, string destID )
        {
            try
            {                
                var bot = new Telegram.Bot.TelegramBotClient(token);
                bot.SendTextMessageAsync(destID, text);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
