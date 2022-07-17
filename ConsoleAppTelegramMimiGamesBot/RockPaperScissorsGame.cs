using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using ConsoleAppTelegramMimiGamesBot;

namespace TelegramGames
{
    enum Cards
    {
        rock, paper, scissors
    }

    struct Player
    {
        public long chatId;
        public int[] cards;

        public Cards lastUsedCard;
        public int round;
        public int winCount;
    }

    internal class RockPaperScissorsGame : IBotGame
    {
        const string waitingOpponentMessage = "Ожидание хода оппонента";
        const int wonValue = 100;

        const int CardsInEnum = 3;
        const int CardsInGame = 3;
        private List<Player> _players = new List<Player>();
        private int _round = 0;

        public RockPaperScissorsGame(params long[] chatId)
        {
            for (int i = 0; i < chatId.Length; i++)
            {
                var newPlayer = new Player();
                newPlayer.chatId = chatId[i];
                newPlayer.cards = new int[CardsInEnum];

                newPlayer.round = 0;
                newPlayer.winCount = 0;

                for (int j = 0; j < CardsInEnum; j++)
                {
                    newPlayer.cards[j] = CardsInGame;
                }

                _players.Add(newPlayer);
            }
        }

        public async void StartRound()
        {
            NextRound();
        }

        public async void NextRound()
        {
            _round++;

            List<InlineKeyboardButton> but;

            for (int i = 0; i < _players.Count; i++)
            {
                string sentText = $"{_round} раунд\n";
                but = new List<InlineKeyboardButton>();

                for (int j = 0; j < CardsInEnum; j++)
                {
                    if (_players[i].cards[j] > 0)
                    {
                        but.Add(InlineKeyboardButton.WithCallbackData(text: $"{(Cards)j}", callbackData: BotInlineButtonsLogic.RockPaperSkissorsGameAction + (Cards)j ));
                        sentText += $"{(Cards)j}: {_players[i].cards[j]}  ";
                    }
                }

                InlineKeyboardMarkup inlineKeyboard = new(new[]
                {
                    but
                });

                await BotMessageManager.SendMessageWithOptions(_players[i].chatId, sentText, inlineKeyboard);
            }
        }

        internal async void RecievePlayerActionInRound(long chatId, Cards card)
        {
            int playerNum = 0;

            for (int i = 1; i < _players.Count; i++)
            {
                if (_players[i].chatId == chatId)
                {
                    playerNum = i;
                    break;
                }
            }

            var pl = _players[playerNum];
            pl.round++;
            pl.cards[(int)card]--;
            pl.lastUsedCard = card;
            _players[playerNum] = pl;

            await BotMessageManager.SendMessageWithOptions(_players[playerNum].chatId, "Вы выбрали " + card);

            int countPlayersActionsInRound = 0;

            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].round == _round)
                {
                    countPlayersActionsInRound++;
                }
            }

            if (countPlayersActionsInRound == _players.Count)
            {
                for (int i = 0; i < _players.Count; i++)
                {
                    int num;
                    if (i == 0)
                    {
                        num = 1;
                    }
                    else
                    {
                        num = 0;
                    }

                    string cardRes = "";
                    if (_players[num].lastUsedCard == Cards.rock) { cardRes = BotMessageManager.rockSticker; }
                    else if (_players[num].lastUsedCard == Cards.paper) { cardRes = BotMessageManager.paperSticker; }
                    else { cardRes = BotMessageManager.scissorsSticker; }

                    await BotMessageManager.SendStickerWithOptions(_players[i].chatId, cardRes);
                }
                GetRoundResult();
            }
            else
            {
                await BotMessageManager.SendMessageWithOptions(_players[playerNum].chatId, waitingOpponentMessage);
            }
        }

        public async void GetRoundResult()
        {
            int first = 0;
            int second = 1;

            if (_round != 9)
            {
                if (_players[first].lastUsedCard == _players[second].lastUsedCard)
                {
                    for (int i = 0; i < _players.Count; i++)
                    {
                        await BotMessageManager.SendMessageWithOptions(_players[i].chatId, $"{_players[i].lastUsedCard} x {_players[i].lastUsedCard}\nНичья! Никто не выиграл и не проиграл в этом раунде");
                    }
                }
                else
                {
                    if (!(_players[first].lastUsedCard < _players[second].lastUsedCard && !(_players[second].lastUsedCard == Cards.scissors && _players[first].lastUsedCard == Cards.rock)) || (_players[first].lastUsedCard > _players[second].lastUsedCard && !(_players[first].lastUsedCard == Cards.scissors && _players[second].lastUsedCard == Cards.rock)))
                    {
                        var pl = _players[first];
                        pl.winCount++;
                        _players[first] = pl;

                        await BotMessageManager.SendMessageWithOptions(pl.chatId, $"{pl.lastUsedCard} x {_players[second].lastUsedCard}\nВы выиграли раунд!");

                        await BotMessageManager.SendMessageWithOptions(_players[second].chatId, $"{_players[second].lastUsedCard} x {pl.lastUsedCard}\nВы проиграли раунд");
                    }
                    else
                    {
                        var pl = _players[second];
                        pl.winCount++;
                        _players[second] = pl;

                        await BotMessageManager.SendMessageWithOptions(pl.chatId, $"{pl.lastUsedCard} x {_players[first].lastUsedCard}\nВы выиграли раунд!");

                        await BotMessageManager.SendMessageWithOptions(_players[first].chatId, $"{_players[first].lastUsedCard} x {pl.lastUsedCard}\nВы проиграли раунд");
                    }
                }

                NextRound();
            }
            else
            {
                if (_players[first].winCount == _players[second].winCount)
                {
                    for (int i = 0; i < _players.Count; i++)
                    {
                        await BotMessageManager.SendMessageWithOptions(_players[i].chatId, "Ничья! Никто не выиграл и не проиграл.");
                    }
                }
                else
                {
                    if (_players[first].winCount > _players[second].winCount)
                    {
                        await BotMessageManager.SendMessageWithOptions(_players[first].chatId, "Вы выиграли!");
                        BotPlayersStatistic.AddBalanceValueToPlayerByChatId(_players[first].chatId, wonValue);

                        await BotMessageManager.SendMessageWithOptions(_players[second].chatId, "Вы проиграли");
                    }
                    else
                    {
                        await BotMessageManager.SendMessageWithOptions(_players[second].chatId, "Вы выиграли!");
                        BotPlayersStatistic.AddBalanceValueToPlayerByChatId(_players[second].chatId, wonValue);

                        await BotMessageManager.SendMessageWithOptions(_players[first].chatId, "Вы проиграли");
                    }
                }

                for (int i = 0; i < _players.Count; i++)
                {
                    var chatId = _players[i].chatId;
                    await BotMessageManager.SendMessageWithOptions(chatId, BotPlayersStatistic.GetPlayerStatisticByChatId(chatId));
                    await BotMessageManager.SendMessageWithOptions(chatId, BotTextLogic.menuMessage);
                }

                BotRPSGamesManager.DeleteGroupByPlayerChatId(_players[first].chatId);
            }
        }
    }
}
