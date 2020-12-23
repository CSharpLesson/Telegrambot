using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotForShaxrixon.BotInlinesComponent
{
    public  static class Inlines
    {
        public async static Task<bool> SendInline(InlineColums column, TelegramBotClient bot) 
        {
            try
            {
                var inlines = new List<InlineKeyboardButton[]>();
                foreach(var item in column.CallBacks)
                    inlines.Add(new[] { InlineKeyboardButton.WithCallbackData($"{ item.CallBackName}", item.CallBackData) });

                var inlineKeyboard = new InlineKeyboardMarkup(inlines);
                bot.EditMessageTextAsync(column.ChatId, messageId: column.MessageId, column.TextMessage, replyMarkup: inlineKeyboard);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);                
            }
            return true;
        }
    }

    public class InlineColums
    { /// <summary>
      /// 
      /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public int ChatId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TextMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CallBack> CallBacks { get; set; }

       
    }
    public class CallBack 
    {

        /// <summary>
        /// 
        /// </summary>
        public string CallBackData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CallBackName { get; set; }
    }
}
