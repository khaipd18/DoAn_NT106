using GameFramework.Core;
using GameFramework.Core.Data;
using GameFramework.Core.GameFramework.Manager;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using System;
using GameFramework.Events;
using static GameFramework.Events.LobbyEvents;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using UnityEditor;
using GameFramework.Core.GameFrameWork.Manager;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Game
{
    // Lớp này xử lý logic liên quan đến quản lý lobby (phòng chờ game).
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        // Dữ liệu người chơi trong lobby
        private List<LobbyPlayerData> _lobbyPlayerDatas = new List<LobbyPlayerData>();
        private LobbyPlayerData _localLobbyPlayerData;
        private LobbyData _lobbyData;
        private int _maxNumberOfPlayers = 2; // Số lượng tối đa người chơi
        private bool _inGame = false; // Trạng thái game, nếu đang chơi thì _inGame = true

        // Kiểm tra xem người chơi hiện tại có phải là host (người tạo lobby) không
        public bool IsHost => _localLobbyPlayerData.Id == LobbyManager.Instance.GetHostID();

        // Đăng ký sự kiện khi lobby được cập nhật
        private void OnEnable()
        {
            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        // Hủy đăng ký sự kiện khi đối tượng bị vô hiệu hóa
        private void OnDisable()
        {
            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
        }

        // Lấy mã lobby hiện tại
        public string GetLobbyCode()
        {
            return LobbyManager.Instance.GetLobbyCode();
        }

        // Tạo lobby mới, gọi phương thức của LobbyManager để tạo lobby
        public async Task<bool> CreateLobby()
        {
            _localLobbyPlayerData = new LobbyPlayerData();
            _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, gamertag: "HostPlayer");
            _lobbyData = new LobbyData();

            bool succeeded = await LobbyManager.Instance.CreateLobby(_maxNumberOfPlayers, isPrivate: true, _localLobbyPlayerData.Serialize(), _lobbyData.Serialize());
            return succeeded;
        }

        // Tham gia một lobby đã có, sử dụng mã lobby để tham gia
        public async Task<bool> JoinLobby(string code)
        {
            _localLobbyPlayerData = new LobbyPlayerData();
            _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, gamertag: "JoinPlayer");
            bool succeeded = await LobbyManager.Instance.JoinLobby(code, _localLobbyPlayerData.Serialize());
            return succeeded;
        }

        // Khi lobby được cập nhật (khi có thay đổi về lobby hoặc người chơi), phương thức này sẽ được gọi
        private async void OnLobbyUpdated(Lobby lobby)
        {
            List<Dictionary<string, PlayerDataObject>> playerData = LobbyManager.Instance.GetPlayerData();
            _lobbyPlayerDatas.Clear();

            int numberOfPlayerReady = 0; // Số lượng người chơi đã nhấn nút ready

            // Duyệt qua tất cả người chơi trong lobby và cập nhật dữ liệu người chơi
            foreach (Dictionary<string, PlayerDataObject> data in playerData)
            {
                LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
                lobbyPlayerData.Initialize(data);

                if (lobbyPlayerData.IsReady)
                {
                    numberOfPlayerReady++;
                }

                // Nếu đây là người chơi local (người chơi hiện tại), lưu thông tin
                if (lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId)
                {
                    _localLobbyPlayerData = lobbyPlayerData;
                }
                _lobbyPlayerDatas.Add(lobbyPlayerData);
            }

            // Cập nhật thông tin về lobby
            _lobbyData = new LobbyData();
            _lobbyData.Initialze(lobby.Data);

            // Kích hoạt sự kiện "LobbyUpdated" để các đối tượng khác trong game nhận được thông tin mới
            Game.Events.LobbyEvents.OnLobbyUpdated?.Invoke();

            // Nếu tất cả người chơi đã sẵn sàng, kích hoạt sự kiện "LobbyReady"
            if (numberOfPlayerReady == lobby.Players.Count)
            {
                Game.Events.LobbyEvents.OnLobbyReady?.Invoke();
            }

            // Nếu có mã RelayJoinCode và chưa vào game, tham gia Relay Server và tải scene
            if (_lobbyData.RelayJoinCode != default && !_inGame)
            {
                await JoinRelayServer(_lobbyData.RelayJoinCode);
                SceneManager.LoadSceneAsync(_lobbyData.SceneName);
            }
        }

        // Lấy danh sách người chơi trong lobby
        public List<LobbyPlayerData> GetPlayers()
        {
            return _lobbyPlayerDatas;
        }

        // Khi người chơi nhấn nút "Ready", cập nhật trạng thái người chơi là ready
        public async Task<bool> SetPlayerReady()
        {
            _localLobbyPlayerData.IsReady = true;
            return await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize());
        }

        // Bắt đầu game, tạo Relay server và cập nhật thông tin lobby
        public async Task StartGame()
        {
            string relayJoinCode = await RelayManager.Instance.CreateRelay(_maxNumberOfPlayers);
            _inGame = true;

            // Cập nhật mã RelayJoinCode vào dữ liệu lobby
            _lobbyData.RelayJoinCode = relayJoinCode;
            await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());

            // Cập nhật dữ liệu người chơi với thông tin kết nối
            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();
            await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);

            // Tải scene cho game
            SceneManager.LoadSceneAsync(_lobbyData.SceneName);
        }

        // Tham gia Relay Server với mã RelayJoinCode
        private async Task<bool> JoinRelayServer(string relayJoinCode)
        {
            _inGame = true;
            await RelayManager.Instance.JoinRelay(relayJoinCode);
            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();
            await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);

            return true;
        }
    }
}
