﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public static void Start()
        {
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.StartReceiving();
        }

        private static void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
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
                showAlert: false);
            }

            var findOrder = orders.FirstOrDefault(f => f.ChatId == e.CallbackQuery.From.Id);
            if (e.CallbackQuery.Data == "order")
            {
                InliniButtonForServices(e);
            }
            else if (e.CallbackQuery.Data == "changeClientName")
            {
                var client = ClientService.GetByChatId(e.CallbackQuery.From.Id);
                client.IsEdit = true;
                client.Name = null;
                ClientService.AddOrUpdate(client);
                Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, "Iltimos Ismingizni yozing");

            }
            else if (e.CallbackQuery.Data == "changeClientNumber")
            {
                var client = ClientService.GetByChatId(e.CallbackQuery.From.Id);
                client.IsEdit = true;
                client.Phone = null;
                ClientService.AddOrUpdate(client);
                Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, "Iltimos telefon raqamingizni kiriting. Masalan 901234567");
            }
            else if (e.CallbackQuery.Data == "setting")
            {
                Settings(e);
            }
            else if (e.CallbackQuery.Data == "setLang") 
            {

            }
            else if (e.CallbackQuery.Data == "main")
            {
                Main(e);
            }
            else if (findOrder == null)
            {
                orders.Add(new Orders() { ChatId = e.CallbackQuery.From.Id, ServiceId = Convert.ToInt32(e.CallbackQuery.Data) });

                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]// bu yerda location qabul qilish ishlatilvotdi
                        {
                            new KeyboardButton("Location") { RequestLocation = true } //keyboard bilan locationi qabul qilinvotdi
                        });
                Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, "Locationi tanlang");
                Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "⬇️⬇️⬇️", ParseMode.Default, false, false, 0, RequestReplyKeyboard);
            }

        }


        private static void Settings(CallbackQueryEventArgs e)
        {
            var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("Ism o'zgartirish", "changeClientName")},
                new[] { InlineKeyboardButton.WithCallbackData("Telefon raqamni o'zgartirish", "changeClientNumber") },
                new[] { InlineKeyboardButton.WithCallbackData("Tilni tanlash", "setLang") },
                new[] { InlineKeyboardButton.WithCallbackData("Asosiy menyuga qaytish", "main") }
            });
            Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, "Tanlang", replyMarkup: inline);
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
            else if (e.Message.Text == "/admin")
            {
                CompanyService.AddOrUpdate(new Company() { ChatId = e.Message.Chat.Id, Name = "Test1" });
            }
            else if (e.Message.Text == "/start" && chat == null || chat == null)
            {
                //Bot.SendVideoAsync(e.Message.Chat.Id, video: read, caption: "Dry car washing");
                ClientService.AddOrUpdate(new Client() { ChatId = e.Message.Chat.Id });
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Assalom alekum Dry Car Washing hush kelibsiz. ");
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Iltimos Ismingizni yozing");
            }
            else if (chat.Name == null)
            {
                ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = e.Message.Text, ChatId = e.Message.Chat.Id });
                var chatName = ClientService.GetByChatId(e.Message.Chat.Id).Name;
                if (chat.IsEdit != true)
                    Bot.SendTextMessageAsync(e.Message.From.Id, $"{chatName} iltimos telefon raqamingizni kiriting. Masalan 901234567 ");
                else
                {
                    var inline = new InlineKeyboardMarkup(new[] {
                          new[] { InlineKeyboardButton.WithCallbackData("Asosiy menyuga qaytish", "main") }
                     });

                    Bot.EditMessageTextAsync(e.Message.Chat.Id, messageId: e.Message.MessageId, "Xizmat yakunlandi", replyMarkup: inline);
                }
            }
            else if (chat.Phone == null)
            {
                try
                {
                    var phone = Convert.ToInt32(e.Message.Text);
                    ClientService.AddOrUpdate(new Client() { Id = chat.Id, Name = chat.Name, Phone = e.Message.Text, ChatId = e.Message.Chat.Id, IsEdit = false });
                    Main(e);
                }
                catch (Exception ex)
                {
                    Bot.SendTextMessageAsync(e.Message.From.Id, $"Iltimos telefon raqamingizni to'g'ri kiriting.  Masalan 901234567");
                }
            }
            else
            {
                Main(e); 
            }
        }

        private static void SendToCompany(MessageEventArgs e)
        {

            var order = orders.FirstOrDefault(f => f.ChatId == e.Message.Chat.Id);
            var service = new Servicess(); 
            if (order != null)
            {

                service = ServicesssDoService.GetById(order.ServiceId);
                var client = ClientService.GetByChatId(e.Message.Chat.Id);
                OrdersService.AddOrUpdate(new Orders() { ChatId = order.ChatId, ServiceId = order.ServiceId, Longitude = e.Message.Location.Longitude, Lotetude = e.Message.Location.Latitude });
                var companys = CompanyService.GetAll();
                foreach (var item in companys)
                {
                    Bot.SendTextMessageAsync(item.ChatId, $"Klient- {client.Name} Telefon nomeri- {client.Phone} Hizmat- {service.Name}  Hizmat narxi - {service.Price}");
                    Bot.SendLocationAsync(item.ChatId, e.Message.Location.Latitude, e.Message.Location.Longitude);
                }
                var inline = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Yana so'rov qoldirish", "order") } });
                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Klient- {client.Name} Telefon nomeri- {client.Phone} Hizmat- {service.Name}  Hizmat narxi - {service.Price}" + " Tez orada sizga qo'ng'iroq qilishadi", replyMarkup: inline);
                orders.Remove(order);
            }
            else
            {
                InliniButtonForServices(e);
            }



        }
        private static void Main(MessageEventArgs e)
        {
            var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("Buyurtma berish", "order")},
                new[] { InlineKeyboardButton.WithCallbackData("Sozlamalar", "setting")}
            });
            Bot.SendTextMessageAsync(e.Message.Chat.Id, "Tanlang", replyMarkup: inline);
        }

        private static void Main(CallbackQueryEventArgs e)
        { 
            var inline = new InlineKeyboardMarkup(new[] {
                new[] { InlineKeyboardButton.WithCallbackData("Buyurtma berish", "order")},
                new[] { InlineKeyboardButton.WithCallbackData("Sozlamalar", "setting")}
            });
            Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Tanlang", replyMarkup: inline);

        }
        private static void InliniButtonForServices(MessageEventArgs e)
        {
            var services = ServicesssDoService.GetAll();
            var inlines = new List<InlineKeyboardButton[]>();
            for (int i = 0; i < services.Count; i++)
                inlines.Add(new[] { InlineKeyboardButton.WithCallbackData($"{ services[i].Name } - {services[i].Price}", services[i].Id.ToString()) });

            var inlineKeyboard = new InlineKeyboardMarkup(inlines);
            Bot.EditMessageTextAsync(e.Message.Chat.Id, messageId: e.Message.MessageId, "Iltimos xizmat turini tanlang", replyMarkup: inlineKeyboard);
        }
        private static void InliniButtonForServices(CallbackQueryEventArgs e)
        {
            var services = ServicesssDoService.GetAll();
            var inlines = new List<InlineKeyboardButton[]>();
            for (int i = 0; i < services.Count; i++)
                inlines.Add(new[] { InlineKeyboardButton.WithCallbackData($"{ services[i].Name } - {services[i].Price}", services[i].Id.ToString()) });

            var inlineKeyboard = new InlineKeyboardMarkup(inlines);
            Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, messageId: e.CallbackQuery.Message.MessageId, "Iltimos xizmat turini tanlang", replyMarkup: inlineKeyboard);
        }
    }
}
