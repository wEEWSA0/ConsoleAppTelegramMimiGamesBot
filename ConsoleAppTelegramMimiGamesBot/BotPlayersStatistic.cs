using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTelegramMimiGamesBot
{
    static internal class BotPlayersStatistic
    {
        struct PlayerStats
        {
            public long chatId;
            public int balance;
        }

        const int outOfRange = -5;
        const string savesFileName = "PlayersStatistic.tbs";
        const int startBalance = 1000;
        private static List<PlayerStats> _playersStats;
        
        public static bool AddNewPlayer(long chatId)
        {
            if (_playersStats == null) { throw new ArgumentNullException("PlayersStats not loaded"); }

            if (IsPlayerExistsByChatId(chatId)) { return false; }

            PlayerStats newPlayer = new PlayerStats();

            newPlayer.chatId = chatId;
            newPlayer.balance = startBalance;

            _playersStats.Add(newPlayer);

            return true;
        }

        public static string GetPlayerStatisticByChatId(long chatId)
        {
            if (!IsPlayerExistsByChatId(chatId)) { throw new Exception("Player's chatId not exists"); }

            int index = GetPlayerNumByChatId(chatId);
            string statisticStr = $"Статистика:\nБаланс: {_playersStats[index].balance}";

            return statisticStr;
        }

        private static bool IsPlayerExistsByChatId(long chatId)
        {
            return GetPlayerNumByChatId(chatId) != outOfRange;
        }

        private static int GetPlayerNumByChatId(long chatId)
        {
            for (int i = 0; i < _playersStats.Count; i++)
            {
                if (_playersStats[i].chatId == chatId)
                {
                    return i;
                }
            }

            return outOfRange;
        }

        public static void AddBalanceValueToPlayerByChatId(long chatId, int value)
        {
            int num = GetPlayerNumByChatId(chatId);
            var playerStats = _playersStats[num];
            playerStats.balance += value;
            _playersStats[num] = playerStats;
        }
        #region SaveLoad Methods
        public static void LoadPlayersStats()
        {
            if (File.Exists(savesFileName))
            {
                StreamReader reader = new StreamReader(savesFileName);
                int length = int.Parse(reader.ReadLine());
                _playersStats = new List<PlayerStats>();

                for (int i = 0; i < length; i++)
                {
                    string streamLine = reader.ReadLine();
                    PlayerStats playerStats = new PlayerStats();

                    playerStats.chatId = long.Parse(streamLine.Split('~')[0]);
                    playerStats.balance = int.Parse(streamLine.Split('~')[1]);
                    _playersStats.Add(playerStats);
                }

                reader.Close();
            }
            else
            {
                _playersStats = new List<PlayerStats>();
            }
        }

        public static void SavePlayersStats()
        {
            StreamWriter writer = new StreamWriter(savesFileName);
            writer.WriteLine(_playersStats.Count);

            for (int i = 0; i < _playersStats.Count; i++)
            {
                writer.WriteLine(_playersStats[i].chatId + "~" + _playersStats[i].balance);
            }

            writer.Close();
        }
        #endregion
    }
}
