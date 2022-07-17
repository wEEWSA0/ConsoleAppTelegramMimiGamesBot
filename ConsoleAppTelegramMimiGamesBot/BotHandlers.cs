using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace ConsoleAppTelegramMimiGamesBot
{
    internal class BotHandlers
    {
        struct MessageSender
        {
            public ITelegramBotClient botClient;
            public CancellationToken cancellationToken;
        }

        private static FindersManager _manager = new FindersManager();
        private static BotInlineButtonsLogic _inlineLogic = new BotInlineButtonsLogic();
        private static BotTextLogic _textLogic = new BotTextLogic();
        private static BotMessageManager _sender = new BotMessageManager();

        public static FindersManager Manager { get { return _manager; } }

        public async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (!_sender.IsInitialize)
            {
                _sender = new BotMessageManager(botClient, update, cancellationToken);
            }
            Console.WriteLine("sddsfdgf");

            if (update.CallbackQuery != null)
            {
                if (update.CallbackQuery.Data != null)
                {/*
                    var chatId = update.CallbackQuery.Message.Chat.Id;
                    */
                    _inlineLogic.RecieveMessage(update.CallbackQuery.Message, update.CallbackQuery.Data);

                    return;
                }
            }

            if (update.Message is not { } message)
                return;

            if (message.Text != null)
            {
                var chatId = message.Chat.Id;

                bool isSpeaker = await BotRPSGamesManager.CheckPlayersGroups(chatId, message.Text);
                if (!isSpeaker)
                {
                    _textLogic.RecieveMessage(message);
                }
            }
        }
        #region Errors Handler
        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        #endregion
    }
}
