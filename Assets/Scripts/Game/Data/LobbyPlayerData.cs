using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GameFramework.Core.Data
{
    public class LobbyPlayerData
    {
        private string _id;
        private string _gamertag;
        private bool _isReady;

        public string Id => _id;
        public string Gamertag => _gamertag;
        public bool IsReady
        {
            get => _isReady; 
            set => _isReady = value;
        }
        public void Initialize(string id, string gametag)
        {
            _id = id;
            _gamertag = gametag;
        }


        public void Initialize(Dictionary<string, PlayerDataObject> playerData)
        {
            UpdateState(playerData);
        }

        public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
        {
            if (playerData.ContainsKey("Id"))
            {
                _id = playerData["Id"].Value;
            }
            if (playerData.ContainsKey("Gamertag"))
            {
                _gamertag = playerData["Gamertag"].Value;
            }
            if (playerData.ContainsKey("IsReady"))
            {
                _isReady = playerData["IsReady"].Value == "True";
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>()
            {
                {"Id", _id},
                {"GamerTag", _gamertag},
                {"IsReady", _isReady.ToString()},
            };

        }
    }
}
