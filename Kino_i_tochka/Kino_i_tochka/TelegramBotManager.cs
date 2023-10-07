using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Kino_i_tochka
{
    public class TelegramBotManager
    {
        ITelegramBotClient _bot;
        SearchManager _searchManager;

        public TelegramBotManager()
        {
            string token = @"6012933060:AAGmh8j54PNkoqyDWKiKrxe52v5eHSB7R9A";
            _bot = new TelegramBotClient(token);
            _searchManager = new SearchManager();

            Console.WriteLine("Бот запущен");
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };

            _bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync(update.Message.Chat.Id + " " + update.Message.Chat.FirstName + " " + update.Message.Text);

            if (update.Message.Text == "/start")
            {
                botClient.SendTextMessageAsync(update.Message.Chat.Id, $"Привет, " + update.Message.Chat.FirstName + " Я телеграмм бот менеджер поиска фильмов" + "\n" + "Напиши название фильма и я найду ссылку для его просмотра");
            }
            else
            {
                string urlImage = await _searchManager.DownloadUrlImgFilm(update.Message.Text);
                if (urlImage != null)
                {
                    botClient.SendPhotoAsync(
                                                chatId: update.Message.Chat.Id,
                                                photo: InputFile.FromUri(urlImage));
                    await botClient.SendTextMessageAsync(
                                                    chatId: update.Message.Chat.Id,
                                                    text: await _searchManager.SearchNameFilm(update.Message.Text),
                                                    replyMarkup: new InlineKeyboardMarkup(
                                                        InlineKeyboardButton.WithUrl(
                                                            text: "Смотреть",
                                                            url: await _searchManager.SearchUrlFilm(update.Message.Text))));
                }
                else
                {
                    botClient.SendTextMessageAsync(update.Message.Chat.Id, "Ничего не найдено");
                }
            }
            
            

        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {

        }

    }
}
