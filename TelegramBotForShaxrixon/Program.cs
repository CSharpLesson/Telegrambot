using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TelegramBotForShaxrixon
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        public static string DataCon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            GetConnection();
            TelegramBot.Start();
            Console.WriteLine("Bot yoqildi");
            Console.ReadLine();
            Console.WriteLine("Bot Ochdi");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async static  Task GetConnection()
        {
            using (FileStream fs = new FileStream("database.json", FileMode.OpenOrCreate))
            {
                var a = await JsonSerializer.DeserializeAsync<Con>(fs);
                Console.WriteLine(a.con);
                DataCon = a.con;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    class Con
    {
        /// <summary>
        /// 
        /// </summary>
        public string con { get; set; }
    }
}
