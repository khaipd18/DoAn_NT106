using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;

namespace GameFramework.Events 
{
    public static class LobbyEvents
    {
        public delegate void LobbyUpdated(Lobby lobby);

        public static LobbyUpdated OnLobbyUpdated;
    }
}
