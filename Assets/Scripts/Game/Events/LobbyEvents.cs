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
        // Delegate cho sự kiện cập nhật Lobby
        public delegate void LobbyUpdated();

        // Sự kiện khi Lobby được cập nhật, các phương thức sẽ được gọi khi sự kiện này xảy ra
        public static LobbyUpdated OnLobbyUpdated;

        // Delegate cho sự kiện khi Lobby đã sẵn sàng
        public delegate void LobbyReady();

        // Sự kiện khi Lobby đã sẵn sàng, các phương thức sẽ được gọi khi sự kiện này xảy ra
        public static LobbyReady OnLobbyReady;
    }
}