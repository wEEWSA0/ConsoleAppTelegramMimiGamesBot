using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramGames;

namespace ConsoleAppTelegramMimiGamesBot
{
    internal class BotInlineButtonsLogic
    {
        private FindersManager _manager;
        const string RockPaperSkissorsGame = "rps";
        const string RockPaperSkissorsFindingPlayers = "rps_finding";
        const string RockPaperSkissorsGameInfo = "Камень, ножницы, бумага\n" +
            "Игра для двух игроков\n\n" +
            "Правила:\n" +
            "У вас по 3 карты камня, бумаги и ножниц. Когда вы решаете сходить одной из них, их количество уменьшается. Если количество падает до 0, вы больше не можете использовать данный тип карт. Игра идет до тех пор, пока не используются все карты.\n" +
            "Участие: бесплатное\n" +
            "Выигрыш: 100k\n";
        public const string RockPaperSkissorsGameAction = "rps_action";
        const string findGroupMessage = "Поиск соперника (игра начнется, когда он найдется)";

        public async void RecieveMessage(Message message, string data)
        {
            switch (data)
            {
                case RockPaperSkissorsGame:
                    {
                        await BotMessageManager.SendMessageWithOptions(message.Chat.Id, RockPaperSkissorsGameInfo);

                        InlineKeyboardMarkup inlineKeyboard = new(new[]
                        {
                            // first row
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData(text: "Начать поиск соперника", callbackData: RockPaperSkissorsFindingPlayers)
                            },
                        });

                        await BotMessageManager.SendMessageWithOptions(message.Chat.Id, "Камень, ножницы, бумага", inlineKeyboard);
                    }
                    break;
                case RockPaperSkissorsFindingPlayers:
                    {
                        Finders finder = new Finders();
                        finder.chatId = message.Chat.Id;
                        var newPlayers = _manager.GoFind(finder);

                        if (newPlayers == null)
                        {
                            await BotMessageManager.SendMessageWithOptions(message.Chat.Id, findGroupMessage);
                        }
                    }
                    break;

                case RockPaperSkissorsGameAction + nameof(Cards.rock):
                    {
                        await BotMessageManager.SendStickerWithOptions(message.Chat.Id, BotMessageManager.rockSticker);
                        BotRPSGamesManager.RecievePlayerActionInGroup(message.Chat.Id, Cards.rock);
                    }
                    break;
                case RockPaperSkissorsGameAction + nameof(Cards.paper):
                    {
                        await BotMessageManager.SendStickerWithOptions(message.Chat.Id, BotMessageManager.paperSticker);
                        BotRPSGamesManager.RecievePlayerActionInGroup(message.Chat.Id, Cards.paper);
                    }
                    break;
                case RockPaperSkissorsGameAction + nameof(Cards.scissors):
                    {
                        await BotMessageManager.SendStickerWithOptions(message.Chat.Id, BotMessageManager.scissorsSticker);
                        BotRPSGamesManager.RecievePlayerActionInGroup(message.Chat.Id, Cards.scissors);
                    }
                    break;
                default:
                    {
                        throw new Exception("undefined callbackData");
                    }
            }
        }


        public BotInlineButtonsLogic()
        {
            _manager = new FindersManager();
        }
    }
}
