using GameFramework.Core.Data;
using GameFramework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        //Lay ma phong
        public string GetLobbyCode()
        {
            return _lobby?.LobbyCode; //Neu ton tai -> tra ve ma phong
        }   


        public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<String, String> data, Dictionary<string,string> lobbyData)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);

            Unity.Services.Lobbies.Models.Player player = new Unity.Services.Lobbies.Models.Player(AuthenticationService.Instance.PlayerId, connectionInfo: null, playerData);

            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                Data = SerializeLobbyData(lobbyData),
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
                    LobbyEvents.OnLobbyUpdated?.Invoke(_lobby);
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

        private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>();
            foreach (var kvp in data)
            {
                lobbyData.Add(kvp.Key, new DataObject(
                    visibility: DataObject.VisibilityOptions.Member,
                    value: kvp.Value
                    ));
            }
            return lobbyData;
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

        public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data, string allocationId = default, string connectionData = default)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            UpdatePlayerOptions options = new UpdatePlayerOptions()
            {
                Data = playerData,
                AllocationId = allocationId,
                ConnectionInfo = connectionData
            };

            try
            {
              _lobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerId, options);

            } catch(System.Exception)
            {
                return false;
            }
            LobbyEvents.OnLobbyUpdated(_lobby);

            return true;
        }

        public async Task<bool> UpdateLobbyData(Dictionary<string,string> data)
        {
            Dictionary<string, DataObject> lobyData = SerializeLobbyData(data);

            UpdateLobbyOptions options = new UpdateLobbyOptions()
            {
                Data = lobyData
            };

            try
            {
                await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, options);
            } catch (System.Exception)
            {
                return false;
            }

            LobbyEvents.OnLobbyUpdated(_lobby);

            return true;
        }

        public string GetHostID()
        {
            return _lobby.HostId;
        }
    }
}
