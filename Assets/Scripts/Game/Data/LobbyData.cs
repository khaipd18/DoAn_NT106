using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;

namespace GameFramework.Core.Data
{
    public class LobbyData
    {
        private string _relayJoinCode;
        private string _sceneName = "InGame";
     
        public string SceneName
        {
            get => _sceneName;
            set => _sceneName = value;
        }

        public string RelayJoinCode
        {
            get => _relayJoinCode;
            set => _relayJoinCode = value;
        }
        public void Initialze(Dictionary<string, DataObject> lobbyData)
        {
            UpdateState(lobbyData);
        }

        public void UpdateState(Dictionary<string, DataObject> lobbyData)
        {
            if (lobbyData.ContainsKey("RelayJoinCode"))
            {
                _relayJoinCode = lobbyData["RelayJoinCode"].Value;
            }

            if (lobbyData.ContainsKey("SceneName"))
            {
                _sceneName = lobbyData["SceneName"].Value;
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>()
            {
                {"RelayJoinCode", _relayJoinCode},
                {"SceneName", _sceneName}
            };
        }

       
    }
}
