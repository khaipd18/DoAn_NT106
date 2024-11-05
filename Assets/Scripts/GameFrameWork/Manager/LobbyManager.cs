using Game.Events;
using GameFramework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GameFramework.Core.GameFramework.Manager
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        private Lobby _lobby;
        private Coroutine _heartbeatCoroutine;
        private Coroutine _refreshbeatCoroutine;

        public string GetLobbyCode()
        {
            return _lobby?.LobbyCode;
        }   
        public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<String, String> data)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);

            Unity.Services.Lobbies.Models.Player player = new Unity.Services.Lobbies.Models.Player(AuthenticationService.Instance.PlayerId, connectionInfo: null, playerData);

            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsPrivate = isPrivate,
                Player = player
            };

            try
            {
                _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);
            } 
            catch (System.Exception)
            {
                return false; 
            }

            _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);

            Debug.Log(message: $"Lobby create with lobby id {_lobby.Id}");

            _heartbeatCoroutine = StartCoroutine(HearthbeatLobbyCoroutine(_lobby.Id, waitTimeSeconds: 6f));
            _refreshbeatCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, waitTimeSeconds: 1f));
            
            return true;
        }

        private IEnumerator HearthbeatLobbyCoroutine(string lobbyid, float waitTimeSeconds)
        {
            while (true)
            {
                Debug.Log(message: "Heartbeat");
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyid);
                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        private IEnumerator RefreshLobbyCoroutine(string lobbyid, float waitTimeSeconds)
        {
            while (true)
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyid);
                yield return new WaitUntil(() => task.IsCompleted);
                Lobby newLobby = task.Result;
                if (newLobby.LastUpdated > _lobby.LastUpdated)
                {
                    _lobby = newLobby;
                            
                }

                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();
            foreach (var kvp in data)
            {
                playerData.Add(kvp.Key, new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Member,
                    value: kvp.Value
                    ));
            }
            return playerData;
        }
        public void OnApplicationQuit()
        {
            if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
            }
        }

        public async Task<bool> JoinLobby(string code, Dictionary<string, string> playerData)
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
            Unity.Services.Lobbies.Models.Player player = new Unity.Services.Lobbies.Models.Player(AuthenticationService.Instance.PlayerId, connectionInfo: null, SerializePlayerData(playerData));

            options.Player = player;
            try
            {
                _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
            }
            catch (System.Exception)
            {
                return false;
            }
            _refreshbeatCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, waitTimeSeconds: 1f));
            return true;
        }

        public List<Dictionary<string, PlayerDataObject>> GetPlayerData()
        {
            List<Dictionary<string, PlayerDataObject>> data = new List<Dictionary<string, PlayerDataObject>>();

            foreach (Unity.Services.Lobbies.Models.Player player in _lobby.Players)
            {
                data.Add(player.Data);
            }
            return data;
        }
    }
}
