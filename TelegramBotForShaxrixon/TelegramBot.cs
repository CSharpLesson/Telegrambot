using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotForShaxrixon.Db;
using TelegramBotForShaxrixon.Model;
using TelegramBotForShaxrixon.Service;

namespace TelegramBotForShaxrixon
{
    /// <summary>
    /// 
    /// </summary>
    public static class TelegramBot
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly TelegramBotClient Bot = new TelegramBotClient("1585845108:AAHcxTNoJMfaOhnhhoVxVU4YDb345Maet5w");

        /// <summary>
        /// 
        /// </summary>
        const int TABRIKLATION_TIME_HOURS = 8;

        /// <summary>
        /// 
        /// </summary>
        const int TABRIKLATION_TIME_MINUTES = 0;

        /// <summary>
        /// 
        /// </summary>
        const int INTERVAL = 86400;

        /// <summary>
        /// 
        /// </summary>
        public static void Start()
        {
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.StartReceiving();
            //SendAllUsersMessage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async static void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
            var langId = lang != null ? lang.LanguageId : 1;
            try
            {
                CallBack(e);
                Console.WriteLine(e.CallbackQuery.From.Id + " " + e.CallbackQuery.Data + " " + DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + "  " + ex);
                Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, langId == 1 ? "Iltimos /refresh boshing" : "Пожалуйста, начните /refresh");

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async static Task CallBack(CallbackQueryEventArgs e)
        {
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
            var langId = lang != null ? lang.LanguageId : 1;
            var client = await ClientService.GetByChatId(e.CallbackQuery.From.Id);
            var count = 0;
            var calback = e.CallbackQuery.Data.IndexOf("_");
            var eventmassive = e.CallbackQuery.Data.Split("_");
            if (client != null && client.IsActive == true)
            {
                switch (e.CallbackQuery.Data)
                {
                    case "confirm":
                        SendLocation(e);
                        break;
                    case "order":
                        SendConfirms(e);
                        break;
                    case "cancel":
                        OrderCancel(e);
                        break;
                    case "done":
                        DoneOrder(e);
                        break;
                    case "setLang":
                        CallbackLang(e);
                        break;
                    case "paynaqt":
                        SendToCompany(e);
                        break;
                    case "paycard":
                        SendToCompany(e);
                        break;
                    case "back":
                        InliniButtonForServices(e);
                        break;
                    case "til1":
                        ChangeLanguage(e);
                        break;
                    case "til2":
                        ChangeLanguage(e);
                        break;
                    case "dontaddSuxoyPar":
                        SendConfirms(e);
                        break;
                }
                if (calback != -1)
                    AddSuxoyPar(e, eventmassive);


                else if (await OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(e.CallbackQuery.Data)) == null)
                    AddOrders(e);

                else if (await OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(e.CallbackQuery.Data)) != null)
                    await Bot.AnswerCallbackQueryAsync(
                                      callbackQueryId: e.CallbackQuery.Id,
                                      text: langId == 1 ? "Bu servisni tanlangan!" : "Эта услуга выбрана!",
                                      showAlert: false);
                Bot.AnswerCallbackQueryAsync(
                                       callbackQueryId: e.CallbackQuery.Id,
                                       text: langId == 1 ? "Jo'natildi" : "Отправлено",
                                       showAlert: false);
            }
            else
                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Bot active bo'magan");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="eventmassive"></param>
        /// <returns></returns>
        private async static Task AddSuxoyPar(CallbackQueryEventArgs e, string[] eventmassive)
        {
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
            var langId = lang != null ? lang.LanguageId : 1;
            var order = await OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(eventmassive[1]));
            if (order != null)
            {
                Bot.AnswerCallbackQueryAsync(
          callbackQueryId: e.CallbackQuery.Id,
          text: langId == 1 ? "Jo'natildi" : "Отправлено",
          showAlert: false);
                OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = e.CallbackQuery.From.Id, ServiceId = order.ServiceId, DateOrder = order.DateOrder, Position = 1, Count = order.Count, SuxoyPar = 20000 });
                SendConfirms(e);
            }
            else
            {
                Bot.AnswerCallbackQueryAsync(
                      callbackQueryId: e.CallbackQuery.Id,
                      text: langId == 1 ? "Bunday xizmat mavjud emas" : "Такая услуга недоступна",
                      showAlert: false);
                SendConfirms(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static void AddOrders(CallbackQueryEventArgs e)
        {
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
            var langId = lang != null ? lang.LanguageId : 1;
            OrdersService.AddOrUpdate(new Orders() { ChatId = e.CallbackQuery.From.Id, ServiceId = Convert.ToInt32(e.CallbackQuery.Data), DateOrder = DateTime.Now, Position = 1, Count = 1 });
            var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "✅Ha" : "✅Да", $"addSuxoyPar_{e.CallbackQuery.Data}"),InlineKeyboardButton.WithCallbackData(langId == 1 ? "❌Yo'q" : "❌Нет", "dontaddSuxoyPar")},
                });
            Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, langId == 1 ? "Suxoy tuman xoxlaysizmi?\n 💨Suxoy tuman 20000 so'm" : "Вы не хотите сухой пар?\n 💨Сухой пар 20000 сўм", replyMarkup: inline);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async static Task SendLocation(CallbackQueryEventArgs e)
        {
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
            var langId = lang != null ? lang.LanguageId : 1;
            var order = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 1);
            if (order.Count() > 0)
            {

                if (order.FirstOrDefault().Count > 0)
                {
                    var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                       {
                            new KeyboardButton(langId==1?"📍 Geolokatsiyani yuboring":"📍 Отправить геолокацию") { RequestLocation = true }
                        });
                    RequestReplyKeyboard.ResizeKeyboard = true;
                    Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, langId == 1 ? "📍 Geolokatsiyani yuboring" : "📍 Отправить геолокацию");
                    Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "⬇️⬇️⬇️", ParseMode.Default, false, false, 0, RequestReplyKeyboard);
                }
                else
                {
                    Bot.AnswerCallbackQueryAsync(
                                callbackQueryId: e.CallbackQuery.Id,
                                text: langId == 1 ? "Hozircha hech qanaqa hizmatni tanlamadingiz!" : "Вы еще не выбрали ни одной услуги!",
                                showAlert: true, cacheTime: 4);
                }

            }
            else
                Bot.AnswerCallbackQueryAsync(
                               callbackQueryId: e.CallbackQuery.Id,
                               langId == 1 ? "Hozircha hech qanaqa hizmatni tanlamadingiz!" : "Вы еще не выбрали ни одной услуги!",
                               showAlert: true, cacheTime: 4);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async static Task OrderCancel(CallbackQueryEventArgs e)
        {
            var orders = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 1);
            if (orders.Count != 0)
            {
                foreach (var order in orders)
                {
                    OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = order.ChatId, ServiceId = order.ServiceId, Position = 4, DateOrder = order.DateOrder, Count = order.Count });
                }
            }
            InliniButtonForServices(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private async static Task SendConfirms(CallbackQueryEventArgs e)
        {
            double allsum = 0;
            var ordersText = @"";
            var lang = await new DataContext().Languages.FirstOrDefaultAsync(f => f.ChatId == e.CallbackQuery.From.Id);
            var langId = lang != null ? lang.LanguageId : 1;
            string umumiy = langId == 1 ? "Umumiy xisob" : "Общий счет";
            string suxoypar = langId == 1 ? "💨Suxoy tuman" : "💨Сухой пар";
            var orders = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 1);
            foreach (var item in orders)
            {
                allsum = allsum + (item.ServiceModel?.Price * item.Count).Value;
                ordersText = ordersText + "\n" + item.ServiceModel?.Name + "\n   " + item.ServiceModel?.Name + " " + item.Count + " x" + " " + item.ServiceModel?.Price + "=" + (item.ServiceModel?.Price * item.Count);
                if (item.SuxoyPar != null)
                {
                    ordersText += ordersText = "\n  " + suxoypar + " = 20000";
                    allsum += 20000;
                }
                ordersText += "\n\n--------\n";
            }
            ordersText = ordersText + "\n" + umumiy + ": " + allsum + " so'm";
            var sendDescript = langId == 1 ? "Tasdiqlashni xohlaysizmi?" : "Вы хотите подтвердить?";
            ordersText += "\n--------\n" + sendDescript;
            var inline = new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "✅Tasdiqlash" : "✅Подтвердить", "confirm") },
                    new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "🚫Bekor qilish" : "🚫Отменить", "cancel") },
                    new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "⬅️Ortga qaytish" : "⬅️Вернуться назад", "back") },
                });
            Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, ordersText, replyMarkup: inline);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private async static Task DoneOrder(CallbackQueryEventArgs e)
        {
            var orders = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 2);

            foreach (var order in orders)
                OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = order.ChatId, ServiceId = order.ServiceId, Longitude = order.Longitude, Lotetude = order.Lotetude, Position = 3, DateOrder = order.DateOrder, Count = order.Count });

            InliniButtonForServices(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now}  {e.Message.Chat.Id}  {e.Message.Chat.Username} {e.Message.Text}");
            Message(e);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        static async Task Message(MessageEventArgs e)
        {
            try
            {
                await TryMessage(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " " + ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task TryMessage(MessageEventArgs e)
        {
            var chat = await ClientService.GetByChatId(e.Message.Chat.Id);
            var company = CompanyService.GetByChatId(e.Message.Chat.Id);
            var order = await OrdersService.GetByPositionChatId(e.Message.Chat.Id, 1);
            //Stream read = File.OpenRead("dry.mp4");
            if (e.Message.Location != null && chat != null && order != null)
            {
                if (order.Longitude == null && order.Longitude == null)
                    SendPayment(e);
                else
                    InliniButtonForServices(e);
            }
            else if (company != null && e.Message.Video != null || company != null && e.Message.Photo != null)
            {
                SendPhotoOrVideo(e);
            }
            else if (e.Message.Text == "/start" && chat == null)
            {

                var firstmessage = "Biz sizga kim deb murojaat qilsak bo’ladi?\n Как мы можем обратиться к вам?";
                //Bot.SendVideoAsync(e.Message.Chat.Id, video: read, caption: "Dry car washing");
                ClientService.AddOrUpdate(new Client() { ChatId = e.Message.Chat.Id });
                Bot.SendTextMessageAsync(e.Message.Chat.Id, firstmessage);

            }
            else if (e.Message.Text == "/todayPeople" && company != null)
            {
                var count = await ClientService.GetCount();
                Bot.SendTextMessageAsync(e.Message.Chat.Id, count != 0 ? "Bugungi qo'shilgan odamlar - " + count : "Bugun odam qo'shilmagan");
            }
            else if (e.Message.Text != "/start" && chat == null)
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos /start ni bosing");

            else if (e.Message.Text == "/info")
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Call center \nTelefon: \n +998 95 001 07 99 \n \n Телефон: \n +998 95 001 07 99");
            else if (e.Message.Text == "/start" && chat.Name == null)
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos telefon ism ni kiriting!");

            else if (e.Message.Text == "/start" && chat.Phone == null)
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos telefon nomer ni kiriting!");

            else if (e.Message.Contact != null && chat.Phone == null)
            {
                var random = new Random().Next(10000, 99999);
                ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = e.Message.Contact.PhoneNumber, ChatId = e.Message.Chat.Id, IsActive = false, GenerateCode = random });
                SendSMSForClient(e);
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos Kodni kiriting! \n \n Пожалуйста, введите код", replyMarkup: new ReplyKeyboardRemove());
            }
            else if (e.Message.Text == "/changenumber")
            {
                ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, ChatId = e.Message.Chat.Id });
                var secondmessage = "Ro'yxatdan o'tish uchun telefon raqamingizni kiriting \nRaqamni 901234567 shaklida yuboring. \n \n Введите свой номер телефона для регистрации \nОтправьте номер в форме 901234567.";
                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]// bu yerda location qabul qilish ishlatilvotdi
                        {
                            new KeyboardButton("📱 Contact") { RequestContact = true }
                        });
                RequestReplyKeyboard.ResizeKeyboard = true;
                RequestReplyKeyboard.OneTimeKeyboard = true;

                Bot.SendTextMessageAsync(e.Message.Chat.Id, secondmessage, ParseMode.Default, false, false, 0, RequestReplyKeyboard);
            }
            else if (chat.Name == null)
            {
                ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = e.Message.Text, ChatId = e.Message.Chat.Id });
                var secondmessage = "Ro'yxatdan o'tish uchun telefon raqamingizni kiriting \nRaqamni 901234567 shaklida yuboring. \n \n Введите свой номер телефона для регистрации \nОтправьте номер в форме 901234567.";
                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]// bu yerda location qabul qilish ishlatilvotdi
                        {
                            new KeyboardButton("📱 Contact") { RequestContact = true }
                        });
                RequestReplyKeyboard.ResizeKeyboard = true;
                RequestReplyKeyboard.OneTimeKeyboard = true;

                Bot.SendTextMessageAsync(e.Message.Chat.Id, secondmessage, ParseMode.Default, false, false, 0, RequestReplyKeyboard);

            }
            else if (chat.Phone == null)
            {
                try
                {
                    if (e.Message.Text.Length == 9)
                    {
                        var random = new Random().Next(10000, 99999);
                        var phone = Convert.ToInt32(e.Message.Text);
                        ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = e.Message.Text, ChatId = e.Message.Chat.Id, IsActive = false, GenerateCode = random });
                        Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos kodni kiriting! \nПожалуйста, введите код!");
                        SendSMSForClient(e);
                    }
                    else
                    {
                        var secondmessage = "Telefon raqam noto'g'ri kiritildi \n Raqamni 901234567 shaklida yuboring!";
                        Bot.SendTextMessageAsync(e.Message.From.Id, secondmessage);
                    }
                }
                catch
                {
                    var secondmessage = "Telefon raqam noto'g'ri kiritildi \n Raqamni 901234567 shaklida yuboring!";
                    Bot.SendTextMessageAsync(e.Message.From.Id, secondmessage);
                }
            }
            else if (chat.IsActive == false && chat.GenerateCode.ToString() == e.Message.Text)
            {
                ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = chat.Phone, GenerateCode = chat.GenerateCode, ChatId = e.Message.Chat.Id, IsActive = true, DateCreate = DateTime.Now.Date });
                InliniButtonForServices(e);
            }
            else if (chat.IsActive == true)
                InliniButtonForServices(e);


            else if (chat.IsActive == false && chat.GenerateCode.ToString() != e.Message.Text)
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos kodni to'g'ri kiriting!");

            else if (e.Message != null && chat != null)
                Bot.SendTextMessageAsync(e.Message.Chat.Id, chat.Name == null ? "Iltimos ismni kiritin" : "Iltimos nomerni kiritin");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private static async Task SendPhotoOrVideo(MessageEventArgs e)
        {
            try
            {
                TrySendPhotoOrVideo(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + e.Message.Chat.Id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task TrySendPhotoOrVideo(MessageEventArgs e)
        {
            var clients = await ClientService.GetAll();
            foreach (var client in clients)
            {
                Bot.ForwardMessageAsync(client.ChatId, e.Message.Chat.Id, e.Message.MessageId);
                Console.WriteLine(client.Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private async static void SendSMSForClient(MessageEventArgs e)
        {
            try
            {
                await TrySendSMSForClient(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " " + ex);
            }
        }

        private static async Task TrySendSMSForClient(MessageEventArgs e)
        {
            var clients = await ClientService.GetByChatId(e.Message.Chat.Id);
            var client = new System.Net.Http.HttpClient();
            if (clients.Phone.Length >= 12)
            {
                var link = $"https://developer.apix.uz/index.php?app=ws&u=d55hh&h=54909e71275b7003c2e0bb00b643938c&op=pv&to=" + clients.Phone + "&msg=Kod+" + clients.GenerateCode;
                Console.WriteLine(link);
                var result = await client.GetAsync(link);
            }
            else if (clients.Phone.Length <= 12)
            {
                var link = $"https://developer.apix.uz/index.php?app=ws&u=d55hh&h=54909e71275b7003c2e0bb00b643938c&op=pv&to=998" + clients.Phone + "&msg=Kod+" + clients.GenerateCode;
                Console.WriteLine(link);
                var result = await client.GetAsync(link);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async static Task SendToCompany(CallbackQueryEventArgs e)
        {
            try
            {
                await TrySendToCompany(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " " + e.CallbackQuery.From.Id + " " + ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task TrySendToCompany(CallbackQueryEventArgs e)
        {
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
            var langId = lang != null ? lang.LanguageId : 1;
            var orders = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 1);
            if (orders != null)
            {
                string suxoypar = langId == 1 ? "💨Suxoy tuman" : "💨Сухой пар";
                string naqt = langId == 1 ? "💵 Naqd to'lash" : "💵 Платить наличными";
                string card = langId == 1 ? "💳 Karta raqam orqali to'lash" : "💳 Оплата по номеру карты";
                string cardNum = langId == 1 ? "Karta raqam:" : "Номер карты:";
                var texts = e.CallbackQuery.Data == "paynaqt" ? "💵 Naqd to'lash" : "💳 Karta raqam orqali to'lash";
                var textForClient = e.CallbackQuery.Data == "paynaqt" ? "" : "";
                double allsum = 0;
                var ordersText = "";
                var client = await ClientService.GetByChatId(e.CallbackQuery.From.Id);
                foreach (var order in orders)
                {
                    allsum = allsum + (order.ServiceModel?.Price * order.Count).Value;
                    ordersText = ordersText + "\n" + order.ServiceModel?.Name + "\n" + "\t" + order.ServiceModel?.Name + " " + order.Count + " x" + " " + order.ServiceModel?.Price + "=" + (order.ServiceModel?.Price * order.Count);
                    if (order.SuxoyPar != null)
                    {
                        ordersText += ordersText = "\n  " + suxoypar + " = 20000";
                        allsum += 20000;
                    }
                    ordersText += "\n\n---------\n\n";
                    OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = order.ChatId, ServiceId = order.ServiceId, Longitude = order.Longitude, Lotetude = order.Lotetude, Position = 2, DateOrder = order.DateOrder, Count = order.Count });
                }
                ordersText = ordersText + "Umumiy: " + allsum + " so'm";
                var companys = CompanyService.GetAll();
                foreach (var item in companys)
                {
                    string text = $"Klient- {client.Name} Telefon nomeri- {client.Phone}" + "\n" + ordersText + "\n --------- \n To'lov turi: " + texts;
                    Bot.SendTextMessageAsync(item.ChatId, text);
                    var longetude = float.Parse(orders.FirstOrDefault().Longitude);
                    var lotetude = float.Parse(orders.FirstOrDefault().Lotetude);
                    Bot.SendLocationAsync(item.ChatId, lotetude, longetude);
                }
                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[] { new KeyboardButton() });
                ordersText = ordersText + "\n \n" + textForClient;
                var inline = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "Ish yakunlandi" : "Работа завершена", "done") } });
                Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, ordersText, replyMarkup: inline);

            }
            else
            {
                InliniButtonForServices(e);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async static Task SendPayment(MessageEventArgs e)
        {
            try
            {
                await TrySendPayment(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " " + e.Message.Chat.Id + " " + ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task TrySendPayment(MessageEventArgs e)
        {
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.Message.Chat.Id);
            var langId = lang != null ? lang.LanguageId : 1;
            var orders = await OrdersService.GetByPositionChatIdDate(e.Message.From.Id, 1);
            if (orders != null)
            {
                foreach (var order in orders)
                {
                    OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = order.ChatId, ServiceId = order.ServiceId, Longitude = e.Message.Location.Longitude.ToString(), Lotetude = e.Message.Location.Latitude.ToString(), Position = 1, DateOrder = order.DateOrder, Count = order.Count, SuxoyPar = order.SuxoyPar });
                }

                await Bot.SendTextMessageAsync(e.Message.Chat.Id, langId == 1 ? "Bizda samarali to'lov turlari bor" : "У нас есть эффективные способы оплаты", replyMarkup: new ReplyKeyboardRemove());
                var inline = new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "💵 Naqd  to'lov" : "💵 Платить наличными", "paynaqt") },
                    new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "💳 Karta raqam orqali to'lash": "💳 Оплата по номеру карты", "paycard") },
                });
                Bot.SendTextMessageAsync(e.Message.Chat.Id, langId == 1 ? "To'lov turini tanlang" : "Выберите способ оплаты", replyMarkup: inline);

            }
            else
                InliniButtonForServices(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async static Task InliniButtonForServices(MessageEventArgs e)
        {
            InliniButtonForServices(e.Message.Chat.Id, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        private async static Task InliniButtonForServices(long chatId, int messageId)
        {
            try
            {
                TryInliniButtonForServices(chatId, messageId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " " + chatId + " " + ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        private static async Task TryInliniButtonForServices(long chatId, int messageId)
        {
            var lang = await new DataContext().Languages.FirstOrDefaultAsync(f => f.ChatId == chatId);
            var langId = lang != null ? lang.LanguageId : 1;
            var services = await ServicesssDoService.GetAll();
            services = services.OrderBy(f => f.Id).ToList();
            var inlines = new List<InlineKeyboardButton[]>();

            for (int i = 0; i < services.Count; i++)
            {
                if (i + 1 >= services.Count)
                {
                    inlines.Add(new[] { InlineKeyboardButton.WithCallbackData($"{ services[i].Name } - {services[i].Price}", services[i].Id.ToString()) });
                }
                else
                {
                    inlines.Add(new[] { InlineKeyboardButton.WithCallbackData($"{ services[i].Name } - {services[i].Price}", services[i].Id.ToString()), InlineKeyboardButton.WithCallbackData($"{ services[i + 1].Name } - {services[i + 1].Price}", services[i + 1].Id.ToString()) });
                }
                i++;
            }

            inlines.Add(new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "🧾 Buyurtma berish" : "🧾 Заказать", "order") });
            inlines.Add(new[] { InlineKeyboardButton.WithCallbackData("Tilni o'zgartirish/Изменить язык", "setLang") });
            var inlineKeyboard = new InlineKeyboardMarkup(inlines);


            if (messageId == 0)
                await Bot.SendTextMessageAsync(chatId, langId == 1 ? "Iltimos xizmat turini tanlang" : "Пожалуйста, выберите тип услуги", replyMarkup: inlineKeyboard);
            else
                await Bot.EditMessageTextAsync(chatId, messageId, langId == 1 ? "Iltimos xizmat turini tanlang" : "Пожалуйста, выберите тип услуги", replyMarkup: inlineKeyboard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async static Task InliniButtonForServices(CallbackQueryEventArgs e)
        {
            var order = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 2);
            if (e.CallbackQuery.Data == "done")
            {
                Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, "✅");
                InliniButtonForServices(e.CallbackQuery.From.Id, 0);
            }
            else if (order.Count() == 0)
                InliniButtonForServices(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId);
            else
                IshTugadimi(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async static Task IshTugadimi(CallbackQueryEventArgs e)
        {
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
            var langId = lang != null ? lang.LanguageId : 1;
            double allsum = 0;
            var ordersText = @"";
            string umumiy = langId == 1 ? "Umumiy xisob" : "Общий счет";
            var orders = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 2);
            foreach (var item in orders)
            {
                allsum = allsum + (item.ServiceModel?.Price * item.Count).Value;
                ordersText = ordersText + @"
" + item.ServiceModel?.Name + @"
   " + item.ServiceModel?.Name + " " + item.Count + " x" + " " + item.ServiceModel?.Price + "=" + (item.ServiceModel?.Price * item.Count) + @"

";
            }
            var inline = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "Ish yakunlandi" : "Работа завершена", "done") } });
            Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, ordersText, replyMarkup: inline);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private static void CallbackLang(MessageEventArgs e)
        {

            var inline = new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData("🇺🇿 Uz", "til1") },
                    new[] { InlineKeyboardButton.WithCallbackData("🇷🇺 Ru", "til2") },
                });
            Bot.SendTextMessageAsync(e.Message.Chat.Id, "O'zingizga qulay tilni tanlang!\n----- \nВыберите удобный Вам язык!.", replyMarkup: inline);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private static void CallbackLang(CallbackQueryEventArgs e)
        {
            var inline = new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData("🇺🇿 Uz", "til1") },
                    new[] { InlineKeyboardButton.WithCallbackData("🇷🇺 Ru", "til2") },
                });
            Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "O'zingizga qulay tilni tanlang!\n----- \nВыберите удобный Вам язык!.", replyMarkup: inline);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private static void ChangeLanguage(CallbackQueryEventArgs e)
        {

            var context = new DataContext();
            var calbackData = e.CallbackQuery.Data;
            var langId = calbackData == "til1" ? 1 : 2;
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
            if (lang == null)
            {
                context.Languages.Add(new Language() { ChatId = e.CallbackQuery.From.Id, LanguageId = langId });
                context.SaveChanges();
            }
            else
            {
                context.Languages.Update(new Language() { Id = lang.Id, ChatId = lang.ChatId, LanguageId = langId });
                context.SaveChanges();
            }

            InliniButtonForServices(e);

        }

        /// <summary>
        /// 
        /// </summary>
        private static async void TaskFriday()
        {
            var clients = await ClientService.GetAll();
            var now = DateTime.Now;
            var tabriklationDateTime = new DateTime(now.Year, now.Month, now.Day, TABRIKLATION_TIME_HOURS, TABRIKLATION_TIME_MINUTES, 0);
            if (tabriklationDateTime < now && now.DayOfWeek == DayOfWeek.Friday)
            {
                foreach (var client in clients)
                {
                    Bot.SendTextMessageAsync(client.ChatId, "Ассалому алайкум. Бизларга берилган умр жудаям қисқа ва ғанимат.  Доимо бир-биримизга меҳрли, эътиборли бўлишимиз билан қалбдаги самимиятлик юксалар экан. Ғанимат бу дунёда бир-биримизни қадрлашга, азиз деб билишга уринайлик. Аллоҳ бизга тақдим этаётган тонгларни шукроналик билан кутиб олайлик." + "Яратган Зот ушбу тонгда барчамизни муродимизни хосил, мушкулларимизни осон, қилган яхши ниятларимизни ижобат айлаб, ҳайру - барокотларини зиёдаси билан тухфа этсин.Иймонимизни бут, сабримизни кенг, виждонимизни пок қилсин.Барчаларимизни Аллоҳ ўз паноҳида асрасин.Жуманинг хайри ва баракоти сизга бўлсин....");
                    Console.WriteLine(client.Name);
                }
                tabriklationDateTime = tabriklationDateTime.AddDays(1);
            }


            int interval = Convert.ToInt32(tabriklationDateTime.Subtract(now).TotalMilliseconds);
            Thread.Sleep(interval);
            while (true)
            {
                if (now.DayOfWeek == DayOfWeek.Friday)
                {

                    foreach (var client in clients)
                    {
                        Bot.SendTextMessageAsync(client.ChatId, "Ассалому алайкум. Бизларга берилган умр жудаям қисқа ва ғанимат.  Доимо бир-биримизга меҳрли, эътиборли бўлишимиз билан қалбдаги самимиятлик юксалар экан. Ғанимат бу дунёда бир-биримизни қадрлашга, азиз деб билишга уринайлик. Аллоҳ бизга тақдим этаётган тонгларни шукроналик билан кутиб олайлик." + "Яратган Зот ушбу тонгда барчамизни муродимизни хосил, мушкулларимизни осон, қилган яхши ниятларимизни ижобат айлаб, ҳайру - барокотларини зиёдаси билан тухфа этсин.Иймонимизни бут, сабримизни кенг, виждонимизни пок қилсин.Барчаларимизни Аллоҳ ўз паноҳида асрасин.Жуманинг хайри ва баракоти сизга бўлсин....");
                    }
                }

                interval = INTERVAL * 1000;
                Thread.Sleep(interval);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public async static void SendAllUsersMessage()
        {
            var clients = await ClientService.GetAll();
            foreach (var client in clients)
            {

                Bot.SendTextMessageAsync(client.ChatId, @"Азиз дўстлар, бу рубрикамизда биз энг кўп бериладиган саволларга жавоб бердик😊

❇️Green wash ўзи нима?

➡️Бу уникал  восита хисобланиб машинани  3 босқичда  тозаланилишда қўлланилади 
Булар:⬇️
 1️⃣. Green wash восита билан          машинани қамраш.
 2️⃣. Кирни юмшоқ  мато билан йўқотиш (микрофибра сочиқларини  кўллаш шарт!)
 3️⃣. Тоза мато билан блеск пайдо бўлгунича артиш

♻️Бу қандай ишлайди❔

✅Машинанинг  кир юзасига тушиб, Green wash актив моддалари ишга тушади ва машина устидаги ифлосланишларни йўқотади.
↪️Ундан сўнг, воситанинг суртилувчи нано-агентлари машинанинг бўёқ кисмига кириб бор ифлосланишни чиқариб ташлайди.
↪️Полировка жараёнида , ишлов берилаётган юзаликдаги махсус моддалар полимерлашади ва мустахкам химоя қатламига айланади. Полимернинг айнан шу қатлами машинанинг юзасига блеск бериб туради ва музлашга карши реагентлар, хамда ултрабинафша нурлари, кислота чўкмаларидан мустахкам химоя хосил қилади.

🔓🔓🔓
Нега машина юзасига зарар етказмайди?

Green wash автомобилни сувсиз тозалаш усулини қўллаганда машина юзасига зарар етқазиши мумкин бўлган барча қаттик юзаликлар полимер микро капсулаларига қамраб олинади.
Ёрдамчи элементлар эса, уларни машина юзасидан буёқни шикастламай тозалашга хизмат қилади

⏳Бир машинани тозалаш🛁  учун қанча миқдорда Green wash махсулоти керак?

⌛️Green wash махсулотининг бир флакони, 🧴яъни 0.5 литри ўртача седанни 3 марта ювиб, блеск беришга етади.
☘️Умуман олганда махсулот сарфланиши машина катталиги ва ифлосланиш даражасига боғлиқ. Ундан ташқари воситани биринчи маротаба қўллаганингизда кейинги жараёнлардан кўра купроқ сарфланади. 😍
Бу ерда тажрибасизлик ўз сўзини айтади! Энг мухими шуки ✔️Green wash воситаси ёрдамида бир бор тозаланган машина юзасида (кузовида)  химоя қатлами хосил бўлади ва бу машинани камроқ ифлосланишига хизмат қилади
☑️☑️☑️ Асосийси осон ювилади 🚿✨

💚💚💚💚Green wash сувсиз тозалаш воситасининг универсаллиги нимада?

❣️Восита универсаллиги шундаки, сиз уни олганда шунчаки тозалаш учун воситага эга бўлиб колмайсиз. Сиз универсал тозалаш тизимини қўлга киритасиз. 
Агарда⏳❌вактингиз тиғиз булиб, машинани ювишни  хохламасангиз - воситани шунчаки унинг юзасига сепиб, микрофибра сочиқ ёрдамида артиб чиқишингиз керак бўлади (20-30 мин вақт кетади)

📎🧾 Кандай юзаликларда қўллаш мумкин?

Деярли барча қаттиқ юзаликларда: машинанинг бўёқ қатлами, пластик, хром, резина, чарм( кожа), алюминий, зангламас пўлат. 
Воситани жуда кенг тармоқларда қўллаш мумкин, масалан скутерлар,  мотоцикл, велосипед, хаттоки қайик ва яхталарда хам! Кўшимчасига Green wash ни кундалик хаётда: уй жихозлари, кафель , интерьер элементлари ва чарм оёқ кийимларга ишлов беришда қўллаш мумкин.


#️⃣Green wash воситаси қўл териси учун зарарсизми?

🔴Табий компонентлардан таёрлангани ва зарарли моддалардан холис бўлгани туфайли, Green wash воситаси  мутлако ➡️зарарсиз!⬅️ Булар мувофиқлик сертификатлари ❗️ва гигиеник хулосалар билан тасдиқланган.
@suxayamoykauz");
                Console.WriteLine(client.Name);
            }
        }
    }
}
