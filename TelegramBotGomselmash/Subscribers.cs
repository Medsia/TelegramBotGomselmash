using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotGomselmash
{
    internal class Subscribers
    {
        public static HashSet<long> subscribers = new HashSet<long>();
        static string subscribersFilePath = "subscribers.txt";
        public static void LoadSubscribers()
        {
            if (System.IO.File.Exists(subscribersFilePath))
            {
                var lines = System.IO.File.ReadAllLines(subscribersFilePath);
                foreach (var line in lines)
                {
                    if (long.TryParse(line, out var chatId))
                    {
                        subscribers.Add(chatId);
                    }
                }
            }
        }

        public static void SaveSubscribers()
        {
            System.IO.File.WriteAllLines(subscribersFilePath, subscribers.Select(chatId => chatId.ToString()));
        }
        public static void AddSubscriber(long chatId)
        {
            subscribers.Add(chatId);
        }
    }
}
