using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramGames
{
    internal interface IBotGame
    {
        async void StartRound() { }
    }
}
