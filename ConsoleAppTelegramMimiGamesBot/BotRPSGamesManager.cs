using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramGames;

namespace ConsoleAppTelegramMimiGamesBot
{
    internal class BotRPSGamesManager
    {
        struct GroupPlayers
        {
            public Finders[] player;
            public RockPaperScissorsGame game;
        }

        const int playersCountInGroup = 2;
        private static List<GroupPlayers> _players = new List<GroupPlayers>();

        public static async Task<bool> CheckPlayersGroups(long chatId, string message)
        {
            int i = 0, j = 0;
            bool isFound = false;

            for (i = 0; i < _players.Count; i++)
            {
                for (j = 0; j < _players[i].player.Length; j++)
                {
                    if (_players[i].player[j].chatId == chatId)
                    {
                        isFound = true;
                        break;
                    }
                }

                if (isFound) { break; }
            }

            if (isFound)
            {
                for (int g = 0; g < _players[i].player.Length; g++)
                {
                    if (g != j)
                    {
                        await BotMessageManager.SendMessageWithOptions(_players[i].player[g].chatId, message);
                    }
                }

                return true;
            }

            return false;
        }

        public static void AddNewPlayersGroup(Finders[] finders)
        {
            GroupPlayers newPlayersGroup = new GroupPlayers();

            var chatIds = new long[playersCountInGroup];
            newPlayersGroup.player = new Finders[playersCountInGroup];

            for (int i = 0; i < playersCountInGroup; i++)
            {
                newPlayersGroup.player[i] = finders[i];
                chatIds[i] = finders[i].chatId;
            }

            newPlayersGroup.game = new RockPaperScissorsGame(chatIds); // game

            StartGame(newPlayersGroup);

            _players.Add(newPlayersGroup);
        }

        private static void StartGame(GroupPlayers group)
        {
            group.game.StartRound();
        }

        public static int GetNumOfGroupByPlayerChatId(long chatId)
        {
            for (int i = 0; i < _players.Count(); i++)
            {
                for (int j = 0; j < _players[i].player.Count(); j++)
                {
                    var player = _players[i].player[j];
                    if (player.chatId == chatId)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public static void RecievePlayerActionInGroup(int groupNum, long chatId, Cards action)
        {
            _players[groupNum].game.RecievePlayerActionInRound(chatId, action);
        }

        public static void RecievePlayerActionInGroup(long chatId, Cards action)
        {
            _players[GetNumOfGroupByPlayerChatId(chatId)].game.RecievePlayerActionInRound(chatId, action);
        }

        public static void DeleteGroupByPlayerChatId(long chatId)
        {
            _players.Remove(_players[GetNumOfGroupByPlayerChatId(chatId)]);
        }
    }
}
