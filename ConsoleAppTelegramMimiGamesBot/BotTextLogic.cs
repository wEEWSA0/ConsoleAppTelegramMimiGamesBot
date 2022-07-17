using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleAppTelegramMimiGamesBot
{
    internal class BotTextLogic
    {
        const string emptyMessage = "empty message";
        const string startMessage = "Бот позволяет анонимно играть в азартные игры на внутриигровую валюту со случайными людьми";
        public const string menuMessage = "Возможности:\n" +
            "'Статистика' - показывает вашу текущую статистику (баланс)\n" +
            "'Игры' - предоставляет меню игр на выбор\n" +
            "'Возможности' - показывает данное меню";
        const string findMessage = "Поиск собеседника (вас уведомят, когда он найдется)";
        const string undefinedMessage = "Извините, но команда не распознанна";
        const string foundGroupMessage = "Собеседник нашелся. Вы подключены к нему";
        const string newPlayerMessage = "\nТак как вы новый игрок, вы получаете бонусную 1000k к вашему балансу!";

        public async void RecieveMessage(Message message)
        {
            string messageText = "" + message.Text;
            string textResult = emptyMessage;

            Console.WriteLine($"Received a '{messageText}' message in chat {message.Chat.Id}.");

            switch (messageText.ToLower())
            {
                case "/start":
                    {
                        if (BotPlayersStatistic.AddNewPlayer(message.Chat.Id))
                        {
                            textResult = Mes(startMessage, menuMessage, newPlayerMessage);
                        }
                        else
                        {
                            textResult = Mes(startMessage, menuMessage);
                        }
                    }
                    break;
                case "возможности":
                    {
                        textResult = menuMessage;
                    }
                    break;
                case "статистика":
                    {
                        textResult = BotPlayersStatistic.GetPlayerStatisticByChatId(message.Chat.Id);
                    }
                    break;
                case "поиск":
                    {
                        Finders finder = new Finders();
                        finder.chatId = message.Chat.Id;
                        var newSpeakers = BotHandlers.Manager.GoFind(finder);

                        if (newSpeakers != null)
                        {
                            for (int i = 0; i < newSpeakers.Count(); i++)
                            {
                                if (newSpeakers[i].chatId != finder.chatId)
                                {
                                    await BotMessageManager.SendMessageWithOptions(newSpeakers[i].chatId, foundGroupMessage);
                                }
                            }

                            textResult = foundGroupMessage;
                        }
                        else
                        {
                            textResult = findMessage;
                        }
                    }
                    break;
                case "игры":
                    {
                        InlineKeyboardMarkup inlineKeyboard = new(new[]
                        {
                            // first row
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData(text: "Камень, ножницы, бумага", callbackData: "rps"),
                                //InlineKeyboardButton.WithCallbackData(text: "1.2", callbackData: "12"),
                            },
                        });

                        await BotMessageManager.SendMessageWithOptions(message.Chat.Id, "Все игры", inlineKeyboard);
                        textResult = emptyMessage;
                    }
                    break;
                default:
                    {
                        textResult = undefinedMessage + "\n" + menuMessage;
                    }
                    break;
            }

            if (textResult == null) { throw new ArgumentException("textResult equals null"); }

            if (textResult != emptyMessage)
            {
                await BotMessageManager.SendMessageWithOptions(message.Chat.Id, textResult);
            }
        }

        private string Mes(params string[] str)
        {
            string newStr = str[0];

            for (int i = 1; i < str.Length; i++)
            {
                newStr += "\n" + str[i];
            }

            return newStr;
        }
    }
}
