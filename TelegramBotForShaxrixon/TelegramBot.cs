using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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


        static List<Orders> orders = new List<Orders>();
        static int plus = 0;
        public static void Start()
        {
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.StartReceiving();
        }

        private async static void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
            var client = ClientService.GetByChatId(e.CallbackQuery.From.Id);
            var langId = lang != null ? lang.LanguageId : 1;
            var count = 0;
            var calback = e.CallbackQuery.Data.IndexOf("_");
            var eventmassive = e.CallbackQuery.Data.Split("_");
            if (client != null && client.IsActive == true)
            {
                if (e.CallbackQuery.Data != "setLang")
                {
                    Bot.AnswerCallbackQueryAsync(
                   callbackQueryId: e.CallbackQuery.Id,
                   text: "Jo'natildi",
                   showAlert: false);
                }

                if (e.CallbackQuery.Data == "order")
                {

                    if (OrdersService.GetByPositionChatId(e.CallbackQuery.From.Id, 1) != null)
                    {
                        var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                                {
                            new KeyboardButton("📍 Location") { RequestLocation = true }
                        });
                        RequestReplyKeyboard.ResizeKeyboard = true;
                        RequestReplyKeyboard.OneTimeKeyboard = true;

                        Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, langId == 1 ? "Locationi tanlang" : "Выберите место");
                        Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "⬇️⬇️⬇️", ParseMode.Default, false, false, 0, RequestReplyKeyboard);
                    }
                    else
                    {
                        Bot.AnswerCallbackQueryAsync(
                                        callbackQueryId: e.CallbackQuery.Id,
                                        text: "Hozircha hech qanaqa hizmatni tanlamadis",
                                        showAlert: false);
                    }
                }
                else if (calback != -1)
                {
                    try
                    {
                        var order = await OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(eventmassive[1]));

                        if (eventmassive[0] == "+" && order != null)
                            count = order.Count + 1;

                        else if (eventmassive[0] == "-" && order != null)
                        {
                            if (order.Count > 0)
                                count = order.Count - 1;
                        }

                        OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = e.CallbackQuery.From.Id, ServiceId = order.ServiceId, DateOrder = order.DateOrder, Position = 1, Count = count });
                        var service = ServicesssDoService.GetById(Convert.ToInt32(eventmassive[1]));
                        var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("➕", $"+_{service.Id}"),InlineKeyboardButton.WithCallbackData($"{service.Name}", "nothing"),InlineKeyboardButton.WithCallbackData($"➖", $"-_{service.Id}"),},
                new[]{ InlineKeyboardButton.WithCallbackData(langId == 1 ? "Buyurtmani tasdiqlash": "Подтвердите заказ", "takeorder") }
                });
                        Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, langId == 1 ? "Tanlangan " + count : "Выбрано " + count, replyMarkup: inline);
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex); ;
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
                    SendToCompany(e);
                }
                else if (e.CallbackQuery.Data == "til1" || e.CallbackQuery.Data == "til2")
                {
                    ChangeLanguage(e);
                }
                else if (e.CallbackQuery.Data == "takeorder")
                {
                    Bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id,
                    text: "Done",
                    showAlert: false);
                    InliniButtonForServices(e);
                }
                else if (e.CallbackQuery.Data == "nothing") { }
                else if (await OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(e.CallbackQuery.Data)) == null)
                {
                    OrdersService.AddOrUpdate(new Orders() { ChatId = e.CallbackQuery.From.Id, ServiceId = Convert.ToInt32(e.CallbackQuery.Data), DateOrder = DateTime.Now, Position = 1 });
                    var service = ServicesssDoService.GetById(Convert.ToInt32(e.CallbackQuery.Data));
                    var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("➕", $"+_{service.Id}"),InlineKeyboardButton.WithCallbackData($"{service.Name}", "nothing"),InlineKeyboardButton.WithCallbackData($"➖", $"-_{service.Id}"),},
                new[]{ InlineKeyboardButton.WithCallbackData(langId == 1 ? "Buyurtmani tasdiqlash" : "Подтвердите заказ", "takeorder") }
                });
                    Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, langId == 1 ? "Tanlangan " + count : "Выбрано " + count, replyMarkup: inline);
                }
                else
                {
                    var counts = await OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(e.CallbackQuery.Data));
                    count = counts != null ? counts.Count : 0;
                    var service = ServicesssDoService.GetById(Convert.ToInt32(e.CallbackQuery.Data));
                    var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("➕", $"+_{service.Id}"),InlineKeyboardButton.WithCallbackData($"{service.Name}", "nothing"),InlineKeyboardButton.WithCallbackData($"➖", $"-_{service.Id}"),},
                new[]{ InlineKeyboardButton.WithCallbackData(langId == 1 ? "Buyurtmani tasdiqlash" : "Подтвердите заказ", "takeorder") }
                });
                    Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, langId == 1 ? "Tanlangan " + count : "Выбрано " + count, replyMarkup: inline);
                }
            }

            else 
            {
                Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Bot active bo'magan");
            }
            

        }

        private async static void DoneOrder(CallbackQueryEventArgs e)
        {
            var orders = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 2);

            foreach (var order in orders)
            {
                OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = order.ChatId, ServiceId = order.ServiceId, Longitude = order.Longitude, Lotetude = order.Lotetude, Position = 3, DateOrder = order.DateOrder, Count = order.Count });
            }
            InliniButtonForServices(e);
        }

        private static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            Message(e);
        }

        static async void Message(MessageEventArgs e)
        {

            var chat = ClientService.GetByChatId(e.Message.Chat.Id);
            var order = OrdersService.GetByPositionChatId(e.Message.Chat.Id, 1);
            //Stream read = File.OpenRead("dry.mp4");
            if (e.Message.Location != null && chat != null && order != null)
            {
                SendPayment(e);
            }
            else if (e.Message.Text != "/start" && chat == null) 
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos /start ni bosing");
            }
            else if (e.Message.Contact != null && chat.Phone == null)
            {
                var random = new Random().Next(10000, 99999);
                ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = e.Message.Contact.PhoneNumber, ChatId = e.Message.Chat.Id, IsActive = false, GenerateCode = random });
                SendSMSForClient(e.Message.Contact.PhoneNumber, random);
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos Kodni kiritin",replyMarkup: new ReplyKeyboardRemove());


            }
            else if (e.Message.Text == "/start" && chat == null)
            {

                var firstmessage = "Assalom alekum Dry Car Washing xush kelibsiz. \n Biz sizga kim deb murojat qilsak bo'ladi ? \n Familiya va ismingizni yuboring \n \n Masalan: Aliyev Vali";
                //Bot.SendVideoAsync(e.Message.Chat.Id, video: read, caption: "Dry car washing");
                ClientService.AddOrUpdate(new Client() { ChatId = e.Message.Chat.Id });
                Bot.SendTextMessageAsync(e.Message.Chat.Id, firstmessage);

            }
            else if (chat.Name == null)
            {
                ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = e.Message.Text, ChatId = e.Message.Chat.Id });
                var chatName = ClientService.GetByChatId(e.Message.Chat.Id).Name;
                var secondmessage = "Ro'yxatdan o'tish uchun telefon raqamingizni kiriting \n \n Raqamni 901234567 shaklida yuboring.";
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
                    var random = new Random().Next(10000, 99999);
                    var phone = Convert.ToInt32(e.Message.Text);
                    ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = chat.Phone, ChatId = e.Message.Chat.Id, IsActive = false, GenerateCode = Convert.ToInt32(e.Message.Text) });
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos Kodni kiritin");
                    SendSMSForClient(e.Message.Text, random);
                }
                catch (Exception ex)
                {
                    var secondmessage = "Telefon raqam noto'g'ri kiritildi \n Raqamni 901234567 shaklida yuboring.";
                    Bot.SendTextMessageAsync(e.Message.From.Id, secondmessage);
                }
            }
            else if (chat.IsActive == false && chat.GenerateCode.ToString() == e.Message.Text)
            {
                ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = chat.Phone, GenerateCode=chat.GenerateCode, ChatId = e.Message.Chat.Id, IsActive = true });
                InliniButtonForServices(e);
            }
            else if (chat.IsActive == true)
            {
                InliniButtonForServices(e);
            }

            else if (chat.IsActive == false && chat.GenerateCode.ToString() != e.Message.Text)
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos kodni to'g'ri kiritin");
            }            
        }

        private async static void SendSMSForClient(string number,int random) 
        {
            var client = new System.Net.Http.HttpClient();
            if (number.Length >= 12)
            {
                var result = await client.GetAsync($"https://developer.apix.uz/index.php?app=ws&u=d55hh&h=54909e71275b7003c2e0bb00b643938c&op=pv&to=" + number + "&msg=Kod+" + random);
            }
            else if (number.Length <= 12)
            {
                var result = await client.GetAsync($"https://developer.apix.uz/index.php?app=ws&u=d55hh&h=54909e71275b7003c2e0bb00b643938c&op=pv&to=998" + number + "&msg=Kod+" + random);
            }            

        }
        private async static void SendToCompany(CallbackQueryEventArgs e)
        {
            try
            {
                var orders = await OrdersService.GetByPositionChatIdDate(e.CallbackQuery.From.Id, 1);
                if (orders != null)
                {
                    var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
                    var langId = lang != null ? lang.LanguageId : 1;
                    string naqt = langId == 1 ? "💵 Naqt to'lash" :"💵 Платить наличными";
                    string card = langId == 1 ? "💳 Karta raqam orqali to'lash" : "💳 Оплата по номеру карты";
                    string cardNum = langId == 1 ? "Karta raqam:" : "Номер карты:";
                    var texts = e.CallbackQuery.Data == "paynaqt" ? "💵 Naqt to'lash" : "💳 Karta raqam orqali to'lash";
                    var textForClient = e.CallbackQuery.Data == "paynaqt" ? "" : " 8600455500005555";
                    double allsum = 0;
                    var ordersText = "";
                    var client = ClientService.GetByChatId(e.CallbackQuery.From.Id);
                    foreach (var order in orders)
                    {
                        allsum = allsum + (order.ServiceModel?.Price * order.Count).Value;
                        ordersText = ordersText + "\n" + order.ServiceModel?.Name + "\n" + "\t" + order.ServiceModel?.Name + " " + order.Count + " x" + " " + order.ServiceModel?.Price + "=" + (order.ServiceModel?.Price * order.Count) +
                            "\n \n ";
                        OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = order.ChatId, ServiceId = order.ServiceId, Longitude = order.Longitude, Lotetude = order.Lotetude, Position = 2, DateOrder = order.DateOrder, Count = order.Count });
                    }
                    ordersText = ordersText + "------ \n Umumiy: " + allsum + " so'm";
                    var companys = CompanyService.GetAll();
                    foreach (var item in companys)
                    {
                        string text = $"Klient- {client.Name} Telefon nomeri- {client.Phone}" + "\n" + ordersText + "\n ------ \n To'lov turi: " + texts;
                        Bot.SendTextMessageAsync(item.ChatId, text);
                        Bot.SendLocationAsync(item.ChatId, orders.FirstOrDefault().Longitude.Value, orders.FirstOrDefault().Lotetude.Value);
                    }
                    var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[] { new KeyboardButton() });
                    ordersText = ordersText + "\n \n" + textForClient;
                    var inline = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Ish yakunlandi", "done") } });
                    Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, ordersText, replyMarkup: inline);

                }
                else
                {
                    InliniButtonForServices(e);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }

        private async static void SendPayment(MessageEventArgs e)
        {
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == e.Message.Chat.Id);
            var langId = lang != null ? lang.LanguageId : 1;

            var orders = await OrdersService.GetByPositionChatIdDate(e.Message.From.Id, 1);
            if (orders != null)
            {

                foreach (var order in orders)
                {
                    OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = order.ChatId, ServiceId = order.ServiceId, Longitude = e.Message.Location.Longitude, Lotetude = e.Message.Location.Latitude, Position = 1, DateOrder = order.DateOrder, Count = order.Count });
                }

                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Bizda samarali to'lov turlari bor", replyMarkup: new ReplyKeyboardRemove());
                var inline = new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "💵 Naxt to'lash" :"💵 Платить наличными", "paynaqt") },
                    new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "💳 Karta raqam orqali to'lash": "💳 Оплата по номеру карты", "paycard") },
                });
                Bot.SendTextMessageAsync(e.Message.Chat.Id, langId == 1 ? "To'lov turini tanlang" : "Выберите способ оплаты", replyMarkup: inline);

            }
            else
            {
                InliniButtonForServices(e);
            }
        }
        private static void InliniButtonForServices(MessageEventArgs e)
        {
            InliniButtonForServices(e.Message.Chat.Id);
        }
        private async static void InliniButtonForServices(long chatId)
        {
            var lang = new DataContext().Languages.FirstOrDefault(f => f.ChatId == chatId);
            var langId = lang != null ? lang.LanguageId : 1;
            double allsum = 0;
            var ordersText = @"";
            string umumiy = langId == 1 ? "Umumiy xisob" : "Общий счет";
            var orders = await OrdersService.GetByPositionChatIdDate(chatId, 1);
            var services = ServicesssDoService.GetAll();
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
            inlines.Add(new[] { InlineKeyboardButton.WithCallbackData(langId == 1 ? "🧾 Buyurtmani tasdiqlash" : "🧾 Подтвердите заказ", "order") });
            inlines.Add(new[] { InlineKeyboardButton.WithCallbackData("Tilni o'zgartirish/Изменить язик", "setLang") });
            var inlineKeyboard = new InlineKeyboardMarkup(inlines);
            if (orders.Count() == 0)
                Bot.SendTextMessageAsync(chatId, langId == 1 ? "Iltimos xizmat turini tanlang" : "Пожалуйста, выберите тип услуги", replyMarkup: inlineKeyboard);
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
                ordersText = ordersText + "------\n"+ umumiy+": " + allsum + " so'm";

                Bot.SendTextMessageAsync(chatId, ordersText, replyMarkup: inlineKeyboard);
            }
        }
        private static void InliniButtonForServices(CallbackQueryEventArgs e)
        {
            InliniButtonForServices(e.CallbackQuery.From.Id);
        }

        private static void CallbackLang(MessageEventArgs e)
        {

            var inline = new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData("🇺🇿 Uz", "til1") },
                    new[] { InlineKeyboardButton.WithCallbackData("🇷🇺 Ru", "til2") },
                });
            Bot.SendTextMessageAsync(e.Message.Chat.Id, "Tilni o'zgartirish/Изменить язик", replyMarkup: inline);
        }
        private static void CallbackLang(CallbackQueryEventArgs e)
        {

            var inline = new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData("🇺🇿 Uz", "til1") },
                    new[] { InlineKeyboardButton.WithCallbackData("🇷🇺 Ru", "til2") },
                });
            Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Tilni o'zgartirish/Изменить язик", replyMarkup: inline);
        }
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
                context.Languages.Update(new Language() { Id=lang.Id, ChatId = lang.ChatId, LanguageId = langId });
                context.SaveChanges();
            }
            InliniButtonForServices(e);

        }

    }
}
