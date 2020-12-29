﻿using System;
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
using TelegramBotForShaxrixon.Model;
using TelegramBotForShaxrixon.Service;

namespace TelegramBotForShaxrixon
{
    public static class TelegramBot
    {
        public static readonly TelegramBotClient Bot = new TelegramBotClient("1474459510:AAEThp8HYzii3Wm9iGGYu81SyKO0sGJpLbc");
        static List<Orders> orders = new List<Orders>();
        static int plus = 0;
        public static void Start()
        {
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.StartReceiving();
        }

        private static void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {

            var count = 0;
            var calback = e.CallbackQuery.Data.IndexOf("_");
            var eventmassive = e.CallbackQuery.Data.Split("_");

            if (e.CallbackQuery.Data != "setLang")
            {
                Bot.AnswerCallbackQueryAsync(
                callbackQueryId: e.CallbackQuery.Id,
                text: "Jo\'natildi",
                showAlert: false);
            }
            else
            {
                Bot.AnswerCallbackQueryAsync(
                callbackQueryId: e.CallbackQuery.Id,
                text: "Hozircha ishlamayabdi",
                showAlert: true, cacheTime: 5);
            }

            if (e.CallbackQuery.Data == "order")
            {

                if (OrdersService.GetByPositionChatId(e.CallbackQuery.From.Id, 1) != null)
                {
                    var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]// bu yerda location qabul qilish ishlatilvotdi
                            {
                            new KeyboardButton("📍 Location") { RequestLocation = true } //keyboard bilan locationi qabul qilinvotdi
                        });
                    RequestReplyKeyboard.ResizeKeyboard = true;
                    Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, "Locationi tanlang");
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
                    var order = OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(eventmassive[1]));

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
                new[]{ InlineKeyboardButton.WithCallbackData("Buyurtmani tasdiqlash","takeorder") }
                });
                    Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, $"Tanlangan {count}", replyMarkup: inline);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex); ;
                }

            }
            else if (e.CallbackQuery.Data == "setLang")
            {

            }
            else if (e.CallbackQuery.Data == "takeorder")
            {
                Bot.AnswerCallbackQueryAsync(callbackQueryId: e.CallbackQuery.Id,
                text: "Done",
                showAlert: false);
                InliniButtonForServices(e);
            }
            else if (e.CallbackQuery.Data == "nothing") { }
            else if (OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(e.CallbackQuery.Data)) == null)
            {
                OrdersService.AddOrUpdate(new Orders() { ChatId = e.CallbackQuery.From.Id, ServiceId = Convert.ToInt32(e.CallbackQuery.Data), DateOrder = DateTime.Now, Position = 1 });
                var service = ServicesssDoService.GetById(Convert.ToInt32(e.CallbackQuery.Data));
                var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("➕", $"+_{service.Id}"),InlineKeyboardButton.WithCallbackData($"{service.Name}", "nothing"),InlineKeyboardButton.WithCallbackData($"➖", $"-_{service.Id}"),},
                new[]{ InlineKeyboardButton.WithCallbackData("Buyurtmani tasdiqlash","takeorder") }
                });
                Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, $"Tanlangan {count}", replyMarkup: inline);
            }
            else
            {
                 count = OrdersService.GetByPositionChatIdService(e.CallbackQuery.From.Id, 1, Convert.ToInt32(e.CallbackQuery.Data)).Count;
                var service = ServicesssDoService.GetById(Convert.ToInt32(e.CallbackQuery.Data));
                var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("➕", $"+_{service.Id}"),InlineKeyboardButton.WithCallbackData($"{service.Name}", "nothing"),InlineKeyboardButton.WithCallbackData($"➖", $"-_{service.Id}"),},
                new[]{ InlineKeyboardButton.WithCallbackData("", "takeorder") }
                });
                Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, $"Tanlangan {count}", replyMarkup: inline);
            }

        }
        private static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            Message(e);
        }

        static async void Message(MessageEventArgs e)
        {

            var chat = ClientService.GetByChatId(e.Message.Chat.Id);
            var order = orders.FirstOrDefault(f => f.ChatId == e.Message.Chat.Id);
            //Stream read = File.OpenRead("dry.mp4");
            if (e.Message.Location != null && chat != null && order != null)
            {
                SendToCompany(e);
            }
            else if (e.Message.Contact != null && chat.Phone == null)
            {
                ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = e.Message.Contact.PhoneNumber, ChatId = e.Message.Chat.Id, IsEdit = false });
                InliniButtonForServices(e);
            }
            else if (e.Message.Text == "/start" && chat == null || chat == null)
            {

                var firstmessage = @"Assalom alekum Dry Car Washing hush kelibsiz.
Biz sizga kim deb murojat qilsak bo'ladi ?
Familiya va ismingizni yuboring

Masalan: Aliyev Vali";
                //Bot.SendVideoAsync(e.Message.Chat.Id, video: read, caption: "Dry car washing");
                ClientService.AddOrUpdate(new Client() { ChatId = e.Message.Chat.Id });
                Bot.SendTextMessageAsync(e.Message.Chat.Id, firstmessage);

            }
            else if (chat.Name == null)
            {
                ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = e.Message.Text, ChatId = e.Message.Chat.Id });
                var chatName = ClientService.GetByChatId(e.Message.Chat.Id).Name;
                var secondmessage = @"Ro'yxatdan o'tish uchun telefon raqamingizni kiriting

Raqamni 901234567 shaklida yuboring.";
                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]// bu yerda location qabul qilish ishlatilvotdi
                        {
                            new KeyboardButton("📱 Contact") { RequestContact = true }
                        });
                RequestReplyKeyboard.ResizeKeyboard = true;

                Bot.SendTextMessageAsync(e.Message.Chat.Id, secondmessage, ParseMode.Default, false, false, 0, RequestReplyKeyboard);

            }
            else if (chat.Phone == null)
            {
                try
                {
                    var phone = Convert.ToInt32(e.Message.Text);
                    ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = e.Message.Text, ChatId = e.Message.Chat.Id, IsEdit = false });
                    InliniButtonForServices(e);
                }
                catch (Exception ex)
                {
                    var secondmessage = @"Telefon raqam noto'g'ri kiritildi

Raqamni 901234567 shaklida yuboring.";
                    Bot.SendTextMessageAsync(e.Message.From.Id, secondmessage);
                }
            }
            else
            {
                InliniButtonForServices(e);
            }
        }

        private static void SendToCompany(MessageEventArgs e)
        {

            var orders = OrdersService.GetByPositionChatIdDate(e.Message.From.Id, 1);
            if (orders != null)
            {
                double allsum = 0;
                var ordersText = @"";
                var client = ClientService.GetByChatId(e.Message.Chat.Id);
                foreach (var order in orders)
                {
                    allsum = allsum + (order.ServiceModel?.Price * order.Count).Value;
                    ordersText = ordersText + @"
" + order.ServiceModel?.Name + @"
   " + order.ServiceModel?.Name + " " + order.Count + " x" + " " + order.ServiceModel?.Price + "=" + (order.ServiceModel?.Price * order.Count) + @"

";
                    OrdersService.AddOrUpdate(new Orders() { Id = order.Id, ChatId = order.ChatId, ServiceId = order.ServiceId, Longitude = e.Message.Location.Longitude, Lotetude = e.Message.Location.Latitude, Position = 2, DateOrder = order.DateOrder, Count = order.Count });
                }
                ordersText = ordersText + @"------
Umumiy: " + allsum + " so'm";
                var companys = CompanyService.GetAll();
                foreach (var item in companys)
                {
                    ordersText = $"Klient- {client.Name} Telefon nomeri- {client.Phone}" + @"
" + ordersText;
                    Bot.SendTextMessageAsync(item.ChatId, ordersText);
                    Bot.SendLocationAsync(item.ChatId, e.Message.Location.Latitude, e.Message.Location.Longitude);
                }
                var inline = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("🧾 Yana Buyurtma berish", "takeorder") } });
                Bot.SendTextMessageAsync(e.Message.Chat.Id, ordersText, replyMarkup: inline);
                
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
        private static void InliniButtonForServices(long chatId)         
        {
            double allsum = 0;
            var ordersText = @"";
            var orders = OrdersService.GetByPositionChatIdDate(chatId, 1);
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
            inlines.Add(new[] { InlineKeyboardButton.WithCallbackData("🧾 Buyurtmani tasdiqlash", "order") });
            var inlineKeyboard = new InlineKeyboardMarkup(inlines);
            if (orders.Count() == 0)
                Bot.SendTextMessageAsync(chatId, "Iltimos xizmat turini tanlang", replyMarkup: inlineKeyboard);
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
                ordersText = ordersText + @"------
Umumiy: " + allsum + " so'm";

                Bot.SendTextMessageAsync(chatId, ordersText, replyMarkup: inlineKeyboard);
            }
        }
        private static void InliniButtonForServices(CallbackQueryEventArgs e)
        {
            InliniButtonForServices(e.CallbackQuery.From.Id);
        }
    }
}
