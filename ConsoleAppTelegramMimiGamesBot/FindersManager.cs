using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ConsoleAppTelegramMimiGamesBot
{
    struct Finders
    {
        public long chatId;
    }

    internal class FindersManager
    {
        private int _groupSize = 2;
        private LinkedList<Finders> _finders = new LinkedList<Finders>();

        public Finders[] GoFind(Finders finder)
        {
            foreach (Finders finder2 in _finders)
            {
                if (finder2.chatId == finder.chatId)
                {
                    return null;
                }
            }

            _finders.AddLast(finder);

            if (_finders.Count >= _groupSize)
            {
                Finders[] sentList = new Finders[_groupSize];
                Finders[] findersArray = _finders.ToArray();

                for (int i = 0; i < _groupSize; i++)
                {
                    sentList[i] = findersArray[i];
                    _finders.Remove(findersArray[i]);
                }

                BotRPSGamesManager.AddNewPlayersGroup(sentList);

                return sentList;
            }
            else { return null; }
        }
    }
}
