using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using ConsoleAppTelegramMimiGamesBot;

var botClient = new TelegramBotClient("5418152505:AAHNycYObFhRkTW4AsOTICqJ8BSnYw4ISrQ");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

BotPlayersStatistic.LoadPlayersStats();

botClient.StartReceiving(
updateHandler: BotHandlers.HandleUpdateAsync,
pollingErrorHandler: BotHandlers.HandlePollingErrorAsync,
receiverOptions: receiverOptions,
cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine("Bot started");
Console.WriteLine($"Start listening for @{me.Username}");
Console.WriteLine("Press enter for stop");
Console.ReadKey();

BotPlayersStatistic.SavePlayersStats();

cts.Cancel();

Console.WriteLine("Bot stopped");