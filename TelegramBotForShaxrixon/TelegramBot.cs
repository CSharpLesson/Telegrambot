using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotForShaxrixon.Db;
using TelegramBotForShaxrixon.Model;
using TelegramBotForShaxrixon.Service;

namespace TelegramBotForShaxrixon
{
    public static class TelegramBot
    {
        public static readonly TelegramBotClient Bot = new TelegramBotClient("1585845108:AAHHZhtx-GGVzIvUub5ucX8OTiL6oscyAGY");
        const int TABRIKLATION_TIME_HOURS = 8;
        const int TABRIKLATION_TIME_MINUTES = 0;
        const int INTERVAL = 86400;

        static List<Orders> orders = new List<Orders>();
        public static void Start()
        {
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.StartReceiving();
        }

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
                if (e.CallbackQuery.Data != "setLang" && e.CallbackQuery.Data != "order")
                {
                    Bot.AnswerCallbackQueryAsync(
                   callbackQueryId: e.CallbackQuery.Id,
                   text: langId == 1 ? "Jo'natildi" : "Отправлено",
                   showAlert: false);
                }

                if (e.CallbackQuery.Data == "order")
                {
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
                    {
                        Bot.AnswerCallbackQueryAsync(
                                        callbackQueryId: e.CallbackQuery.Id,
                                        langId == 1 ? "Hozircha hech qanaqa hizmatni tanlamadingiz!" : "Вы еще не выбрали ни одной услуги!",
                                        showAlert: true, cacheTime: 4);
                    }
                }
                else if (calback != -1)
                {

                    var order = await OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(eventmassive[1]));

                    if (eventmassive[0] == "+" && order != null)
                        count = order.Count + 1;

                    else if (eventmassive[0] == "-" && order != null)
                    {
                        if (order.Count > 0)
                            count = order.Count - 1;
                    }

                    if (order == null)
                    {
                        await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, "Siz hech qanday servis tanlamadis");
                    }
                    else
                    {
                        await OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = e.CallbackQuery.From.Id, ServiceId = order.ServiceId, DateOrder = order.DateOrder, Position = 1, Count = count });
                        var service = await ServicesssDoService.GetById(Convert.ToInt32(eventmassive[1]));
                        var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("➕", $"+_{service.Id}"),InlineKeyboardButton.WithCallbackData($"{service.Name}", "nothing"),InlineKeyboardButton.WithCallbackData($"➖", $"-_{service.Id}"),},
                new[]{ InlineKeyboardButton.WithCallbackData(langId == 1 ? "Buyurtmani tasdiqlash": "Подтвердите заказ", "takeorder") }
                });
                        await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, langId == 1 ? "Tanlangan " + count : "Выбрано " + count, replyMarkup: inline);
                    }
                }
                else if (e.CallbackQuery.Data == "done")
                {
                    DoneOrder(e);
                }
                else if (e.CallbackQuery.Data == "setLang")
                {
                    CallbackLang(e);
                }
                else if (e.CallbackQuery.Data == "paynaqt" || e.CallbackQuery.Data == "paycard")
                {
                    await SendToCompany(e);
                }
                else if (e.CallbackQuery.Data == "til1" || e.CallbackQuery.Data == "til2")
                {
                    ChangeLanguage(e);
                }
                else if (e.CallbackQuery.Data == "takeorder")
                {
                    await Bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id,
                    text: "Done",
                    showAlert: false);
                    InliniButtonForServices(e);
                    await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, "done");
                }
                else if (e.CallbackQuery.Data == "nothing") { }
                else if (await OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(e.CallbackQuery.Data)) == null)
                {
                    await OrdersService.AddOrUpdate(new Orders() { ChatId = e.CallbackQuery.From.Id, ServiceId = Convert.ToInt32(e.CallbackQuery.Data), DateOrder = DateTime.Now, Position = 1 });
                    var service = await ServicesssDoService.GetById(Convert.ToInt32(e.CallbackQuery.Data));
                    var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("➕", $"+_{service.Id}"),InlineKeyboardButton.WithCallbackData($"{service.Name}", "nothing"),InlineKeyboardButton.WithCallbackData($"➖", $"-_{service.Id}"),},
                new[]{ InlineKeyboardButton.WithCallbackData(langId == 1 ? "Buyurtmani tasdiqlash" : "Подтвердите заказ", "takeorder") }
                });
                    await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, langId == 1 ? "Tanlangan " + count : "Выбрано " + count, replyMarkup: inline);
                }
                else
                {
                    var counts = await OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(e.CallbackQuery.Data));
                    count = counts != null ? counts.Count : 0;
                    var service = await ServicesssDoService.GetById(Convert.ToInt32(e.CallbackQuery.Data));
                    var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("➕", $"+_{service.Id}"),InlineKeyboardButton.WithCallbackData($"{service.Name}", "nothing"),InlineKeyboardButton.WithCallbackData($"➖", $"-_{service.Id}"),},
                new[]{ InlineKeyboardButton.WithCallbackData(langId == 1 ? "Buyurtmani tasdiqlash" : "Подтвердите заказ", "takeorder") }
                });
                    await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, langId == 1 ? "Tanlangan " + count : "Выбрано " + count, replyMarkup: inline);

                }
            }

            else
            {
                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Bot active bo'magan");
            }
        }



        private async static void DoneOrder(CallbackQueryEventArgs e)
        {
            var orders = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 2);

            foreach (var order in orders)
            {
                await OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = order.ChatId, ServiceId = order.ServiceId, Longitude = order.Longitude, Lotetude = order.Lotetude, Position = 3, DateOrder = order.DateOrder, Count = order.Count });
            }
            await InliniButtonForServices(e);
        }

        private static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now}  {e.Message.Chat.Id}  {e.Message.Chat.Username} {e.Message.Text}");
            Task.Run(() => Message(e));
        }

        static async Task Message(MessageEventArgs e)
        {
            try
            {
                var chat = await ClientService.GetByChatId(e.Message.Chat.Id);
                var order = await OrdersService.GetByPositionChatId(e.Message.Chat.Id, 1);
                //Stream read = File.OpenRead("dry.mp4");
                if (e.Message.Location != null && chat != null && order != null)
                {
                    if (order.Longitude == null && order.Longitude == null)
                        SendPayment(e);
                    else
                        InliniButtonForServices(e);
                }
                else if (e.Message.Text == "/start" && chat == null)
                {

                    var firstmessage = "Biz sizga kim deb murojaat qilsak bo’ladi?\n Как мы можем обратиться к вам?";
                    //Bot.SendVideoAsync(e.Message.Chat.Id, video: read, caption: "Dry car washing");
                    await ClientService.AddOrUpdate(new Client() { ChatId = e.Message.Chat.Id });
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, firstmessage);

                }
                else if (e.Message.Text != "/start" && chat == null)
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos /start ni bosing");
                }
                else if (e.Message.Text == "/info")
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Call center \nTelefon: \n +998 95 0045899 \n \n Телефон: \n +998 95 0045899");
                else if (e.Message.Text == "/start" && chat.Name == null)
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos telefon ism ni kiriting!");
                }
                else if (e.Message.Text == "/start" && chat.Phone == null)
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos telefon nomer ni kiriting!");
                }
                else if (e.Message.Contact != null && chat.Phone == null)
                {
                    var random = new Random().Next(10000, 99999);
                    await ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = e.Message.Contact.PhoneNumber, ChatId = e.Message.Chat.Id, IsActive = false, GenerateCode = random });
                    SendSMSForClient(e);
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos Kodni kiriting! \n \n Пожалуйста, введите код", replyMarkup: new ReplyKeyboardRemove());


                }
                else if (chat.Name == null)
                {
                    await ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = e.Message.Text, ChatId = e.Message.Chat.Id });
                    var secondmessage = "Ro'yxatdan o'tish uchun telefon raqamingizni kiriting \nRaqamni 901234567 shaklida yuboring. \n \n Введите свой номер телефона для регистрации \nОтправьте номер в форме 901234567.";
                    var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]// bu yerda location qabul qilish ishlatilvotdi
                            {
                            new KeyboardButton("📱 Contact") { RequestContact = true }
                        });
                    RequestReplyKeyboard.ResizeKeyboard = true;
                    RequestReplyKeyboard.OneTimeKeyboard = true;

                    await Bot.SendTextMessageAsync(e.Message.Chat.Id, secondmessage, ParseMode.Default, false, false, 0, RequestReplyKeyboard);

                }
                else if (chat.Phone == null)
                {
                    try
                    {
                        var random = new Random().Next(10000, 99999);
                        var phone = Convert.ToInt32(e.Message.Text);
                        await ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = e.Message.Text, ChatId = e.Message.Chat.Id, IsActive = false, GenerateCode = random });
                        Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos kodni kiriting! \nПожалуйста, введите код!");
                        SendSMSForClient(e);
                    }
                    catch
                    {
                        var secondmessage = "Telefon raqam noto'g'ri kiritildi \n Raqamni 901234567 shaklida yuboring!";
                        Bot.SendTextMessageAsync(e.Message.From.Id, secondmessage);
                    }
                }
                else if (chat.IsActive == false && chat.GenerateCode.ToString() == e.Message.Text)
                {
                    await ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = chat.Phone, GenerateCode = chat.GenerateCode, ChatId = e.Message.Chat.Id, IsActive = true });
                    InliniButtonForServices(e);
                }
                else if (chat.IsActive == true)
                {
                    InliniButtonForServices(e);
                }

                else if (chat.IsActive == false && chat.GenerateCode.ToString() != e.Message.Text)
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos kodni to'g'ri kiriting!");
                }
                else if (e.Message != null && chat != null)
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, chat.Name == null ? "Iltimos ismni kiritin" : "Iltimos nomerni kiritin");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " " + ex);
            }            
        }

        private async static void SendSMSForClient(MessageEventArgs e)
        {
            try
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
            catch (Exception ex)
            {

                Console.WriteLine(DateTime.Now + " " + ex);
            }


        }
        private async static Task SendToCompany(CallbackQueryEventArgs e)
        {
            try
            {
                var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
                var langId = lang != null ? lang.LanguageId : 1;
                var orders = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 1);
                if (orders != null)
                {
                    string naqt = langId == 1 ? "💵 Naqt to'lash" : "💵 Платить наличными";
                    string card = langId == 1 ? "💳 Karta raqam orqali to'lash" : "💳 Оплата по номеру карты";
                    string cardNum = langId == 1 ? "Karta raqam:" : "Номер карты:";
                    var texts = e.CallbackQuery.Data == "paynaqt" ? "💵 Naqt to'lash" : "💳 Karta raqam orqali to'lash";
                    var textForClient = e.CallbackQuery.Data == "paynaqt" ? "" : "";
                    double allsum = 0;
                    var ordersText = "";
                    var client = await ClientService.GetByChatId(e.CallbackQuery.From.Id);
                    foreach (var order in orders)
                    {
                        allsum = allsum + (order.ServiceModel?.Price * order.Count).Value;
                        ordersText = ordersText + "\n" + order.ServiceModel?.Name + "\n" + "\t" + order.ServiceModel?.Name + " " + order.Count + " x" + " " + order.ServiceModel?.Price + "=" + (order.ServiceModel?.Price * order.Count) +
                            "\n \n ";
                        await OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = order.ChatId, ServiceId = order.ServiceId, Longitude = order.Longitude, Lotetude = order.Lotetude, Position = 2, DateOrder = order.DateOrder, Count = order.Count });
                    }
                    ordersText = ordersText + "------ \n Umumiy: " + allsum + " so'm";
                    var companys = CompanyService.GetAll();
                    foreach (var item in companys)
                    {
                        string text = $"Klient- {client.Name} Telefon nomeri- {client.Phone}" + "\n" + ordersText + "\n ------ \n To'lov turi: " + texts;
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
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " " + e.CallbackQuery.From.Id + " " + ex);
            }


        }

        private async static Task SendPayment(MessageEventArgs e)
        {
            try
            {

                var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.Message.Chat.Id);
                var langId = lang != null ? lang.LanguageId : 1;

                var orders = await OrdersService.GetByPositionChatIdDate(e.Message.From.Id, 1);
                if (orders != null)
                {
                    foreach (var order in orders)
                    {
                        await OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = order.ChatId, ServiceId = order.ServiceId, Longitude = e.Message.Location.Longitude.ToString(), Lotetude = e.Message.Location.Latitude.ToString(), Position = 1, DateOrder = order.DateOrder, Count = order.Count });
                    }

                    await Bot.SendTextMessageAsync(e.Message.Chat.Id, "Bizda samarali to'lov turlari bor", replyMarkup: new ReplyKeyboardRemove());
                    var inline = new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "💵 Naqd  to'lov" : "💵 Платить наличными", "paynaqt") },
                    new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "💳 Karta raqam orqali to'lash": "💳 Оплата по номеру карты", "paycard") },
                });
                    await Bot.SendTextMessageAsync(e.Message.Chat.Id, langId == 1 ? "To'lov turini tanlang" : "Выберите способ оплаты", replyMarkup: inline);

                }
                else
                {
                    await InliniButtonForServices(e);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " " + e.Message.Chat.Id + " " + ex);
            }
        }
        private async static Task InliniButtonForServices(MessageEventArgs e)
        {
            await Task.Run(() => InliniButtonForServices(e.Message.Chat.Id));
        }
        private async static void InliniButtonForServices(long chatId)
        {
            try
            {


                var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == chatId);
                var langId = lang != null ? lang.LanguageId : 1;
                double allsum = 0;
                var ordersText = @"";
                string umumiy = langId == 1 ? "Umumiy xisob" : "Общий счет";
                var orders = await OrdersService.GetByPositionChatIdDate(chatId, 1);
                var services = await ServicesssDoService.GetAll();
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
                if (orders.Count() == 0)
                    await Bot.SendTextMessageAsync(chatId, langId == 1 ? "Iltimos xizmat turini tanlang" : "Пожалуйста, выберите тип услуги", replyMarkup: inlineKeyboard);
                else if (orders.FirstOrDefault().Count == 0)
                    await Bot.SendTextMessageAsync(chatId, langId == 1 ? "Iltimos xizmat turini tanlang" : "Пожалуйста, выберите тип услуги", replyMarkup: inlineKeyboard);
                else
                {
                    foreach (var item in orders)
                    {
                        allsum = allsum + (item.ServiceModel?.Price * item.Count).Value;
                        ordersText = ordersText + @"
" + item.ServiceModel?.Name + @"
   " + item.ServiceModel?.Name + " " + item.Count + " x" + " " + item.ServiceModel?.Price + "=" + (item.ServiceModel?.Price * item.Count) + @"

";
                    }
                    ordersText = ordersText + "------\n" + umumiy + ": " + allsum + " so'm";

                    await Bot.SendTextMessageAsync(chatId, ordersText, replyMarkup: inlineKeyboard);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " " + chatId + " " + ex);
            }
        }
        private async static Task InliniButtonForServices(CallbackQueryEventArgs e)
        {
            var order = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 2);
            if (order.Count() == 0)
                await Task.Run(() => InliniButtonForServices(e.CallbackQuery.From.Id));
            else
            {
                await Task.Run(() => IshTugadimi(e));
            }
        }


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
            await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, ordersText, replyMarkup: inline);
        }

        private static void CallbackLang(MessageEventArgs e)
        {

            var inline = new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData("🇺🇿 Uz", "til1") },
                    new[] { InlineKeyboardButton.WithCallbackData("🇷🇺 Ru", "til2") },
                });
            Bot.SendTextMessageAsync(e.Message.Chat.Id, "O'zingizga qulay tilni tanlang!\n----- \nВыберите удобный Вам язык!.", replyMarkup: inline);
        }
        private async static void CallbackLang(CallbackQueryEventArgs e)
        {
            await Task.Run(() =>
            {
                var inline = new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData("🇺🇿 Uz", "til1") },
                    new[] { InlineKeyboardButton.WithCallbackData("🇷🇺 Ru", "til2") },
                });
                Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "O'zingizga qulay tilni tanlang!\n----- \nВыберите удобный Вам язык!.", replyMarkup: inline);
            });

        }
        private async static void ChangeLanguage(CallbackQueryEventArgs e)
        {
            await Task.Run(() =>
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
            });

            await InliniButtonForServices(e);

        }
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

    }
}
