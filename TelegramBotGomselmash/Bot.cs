using Telegram.Bot;

namespace TelegramBotGomselmash
{
    public static class Bot
    {
        private static TelegramBotClient? client { get; set; }
        public static TelegramBotClient GetTelegramBot(string botToken)
        {
            if (client != null)
            {
                return client;
            }
            client = new TelegramBotClient(botToken);
            return client;
        }
    }
}
