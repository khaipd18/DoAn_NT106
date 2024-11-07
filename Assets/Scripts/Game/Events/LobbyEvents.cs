using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;

namespace Game.Events
{
    public static class LobbyEvents
    {
        public delegate void LobbyUpdated();

        public static LobbyUpdated OnLobbyUpdated;


        public delegate void LobbyReady();

        public static LobbyReady OnLobbyReady;
    }
}