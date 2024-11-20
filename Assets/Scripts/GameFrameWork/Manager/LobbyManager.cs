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
    // Class quản lý các hoạt động liên quan đến Lobby (phòng chơi).
    public class LobbyManager : Singleton<LobbyManager>
    {
        private Lobby _lobby;  // Biến chứa thông tin phòng (lobby) hiện tại
        private Coroutine _heartbeatCoroutine;  // Coroutine gửi tín hiệu heartbeat
        private Coroutine _refreshbeatCoroutine; // Coroutine làm mới thông tin lobby

        // Lấy mã phòng của lobby hiện tại
        public string GetLobbyCode()
        {
            return _lobby?.LobbyCode; // Nếu phòng tồn tại thì trả về mã phòng, nếu không trả về null
        }

        // Tạo một lobby mới với thông số maxPlayer, privacy, dữ liệu người chơi (Host) và dữ liệu lobby
        public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> data, Dictionary<string, string> lobbyData)
        {
            // Chuyển đổi dữ liệu người chơi sang dạng PlayerDataObject (
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);

            // Tạo đối tượng player mới
            Unity.Services.Lobbies.Models.Player player = new Unity.Services.Lobbies.Models.Player(AuthenticationService.Instance.PlayerId, connectionInfo: null, playerData);

            // Cấu hình tùy chọn khi tạo lobby
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                Data = SerializeLobbyData(lobbyData),  // Chuyển đổi dữ liệu lobby sang dạng cần thiết
                IsPrivate = isPrivate,  // Quy định xem lobby có riêng tư hay không
                Player = player  // Thông tin người chơi
            };

            try
            {
                // Tạo lobby và lưu vào biến _lobby
                _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);
            }
            catch (System.Exception)
            {
                // Nếu có lỗi xảy ra trong quá trình tạo lobby, trả về false
                return false;
            }

            // Đảm bảo lobby đã được tạo thành công
            Debug.Log(message: $"Lobby created with lobby id {_lobby.Id}");

            // Bắt đầu gửi tín hiệu heartbeat và làm mới thông tin lobby mỗi khoảng thời gian nhất định
            _heartbeatCoroutine = StartCoroutine(HearthbeatLobbyCoroutine(_lobby.Id, waitTimeSeconds: 6f));
            _refreshbeatCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, waitTimeSeconds: 1f));

            return true;  // Trả về true nếu tạo lobby thành công
        }

        // Coroutine gửi tín hiệu heartbeat cho lobby
        private IEnumerator HearthbeatLobbyCoroutine(string lobbyid, float waitTimeSeconds)
        {
            while (true)
            {
                Debug.Log(message: "Heartbeat");  // In ra log để kiểm tra
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyid);  // Gửi tín hiệu heartbeat
                yield return new WaitForSecondsRealtime(waitTimeSeconds);  // Chờ một thời gian trước khi gửi tiếp
            }
        }

        // Coroutine làm mới thông tin lobby mỗi khoảng thời gian nhất định
        private IEnumerator RefreshLobbyCoroutine(string lobbyid, float waitTimeSeconds)
        {
            while (true)
            {
                // Lấy lại thông tin lobby từ dịch vụ Lobby
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyid);
                yield return new WaitUntil(() => task.IsCompleted);  // Chờ cho đến khi task hoàn thành
                Lobby newLobby = task.Result;

                // Nếu thông tin lobby được cập nhật, lưu lại và gọi sự kiện
                if (newLobby.LastUpdated > _lobby.LastUpdated)
                {
                    _lobby = newLobby;
                    LobbyEvents.OnLobbyUpdated?.Invoke(_lobby);  // Gọi sự kiện thông báo có cập nhật mới
                }

                // Chờ một khoảng thời gian trước khi làm mới lại
                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        // Chuyển dữ liệu của người chơi thành dạng PlayerDataObject
        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();
            foreach (var kvp in data)
            {
                playerData.Add(kvp.Key, new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Member,  // Đảm bảo dữ liệu có thể được người chơi khác xem(Menber)
                    value: kvp.Value  // Giá trị dữ liệu
                    ));
            }
            return playerData;  // Trả về dữ liệu đã chuyển đổi
        }

        // Chuyển dữ liệu của lobby thành dạng DataObject
        private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> data) 
        {
            Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>();
            foreach (var kvp in data)
            {
                lobbyData.Add(kvp.Key, new DataObject(
                    visibility: DataObject.VisibilityOptions.Member,  // Đảm bảo dữ liệu có thể được người chơi khác xem
                    value: kvp.Value  // Giá trị dữ liệu
                    ));
            }
            return lobbyData;  // Trả về dữ liệu đã chuyển đổi
        }

        // Xóa lobby khi ứng dụng bị đóng (nếu người chơi là host)
        public void OnApplicationQuit()
        {
            if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);  // Xóa lobby nếu người chơi là host
            }
        }

        // Tham gia một lobby bằng mã lobby
        public async Task<bool> JoinLobby(string code, Dictionary<string, string> playerData)
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
            Unity.Services.Lobbies.Models.Player player = new Unity.Services.Lobbies.Models.Player(AuthenticationService.Instance.PlayerId, connectionInfo: null, SerializePlayerData(playerData));

            options.Player = player;
            try
            {
                // Tham gia lobby và lưu lại thông tin lobby
                _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
            }
            catch (System.Exception)
            {
                // Nếu có lỗi xảy ra khi tham gia, trả về false
                return false;
            }
            _refreshbeatCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, waitTimeSeconds: 1f));
            return true;  // Trả về true nếu tham gia thành công
        }

        // Lấy dữ liệu của tất cả người chơi trong lobby
        public List<Dictionary<string, PlayerDataObject>> GetPlayerData()
        {
            List<Dictionary<string, PlayerDataObject>> data = new List<Dictionary<string, PlayerDataObject>>();

            // Lặp qua từng người chơi và lấy dữ liệu của họ
            foreach (Unity.Services.Lobbies.Models.Player player in _lobby.Players)
            {
                data.Add(player.Data);  // Thêm dữ liệu của người chơi vào danh sách
            }
            return data;  // Trả về danh sách dữ liệu người chơi
        }

        // Cập nhật dữ liệu của người chơi trong lobby
        public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data, string allocationId = default, string connectionData = default)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            UpdatePlayerOptions options = new UpdatePlayerOptions()
            {
                Data = playerData,  // Cập nhật dữ liệu của người chơi. VD: isReady -> true
                AllocationId = allocationId,
                ConnectionInfo = connectionData
            };

            try
            {
                // Cập nhật dữ liệu của người chơi trong lobby
                _lobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerId, options);
            }
            catch (System.Exception)
            {
                return false;  // Nếu có lỗi xảy ra, trả về false
            }
            LobbyEvents.OnLobbyUpdated(_lobby);  // Gọi sự kiện khi lobby được cập nhật
            return true;
        }

        // Cập nhật dữ liệu của lobby
        public async Task<bool> UpdateLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string, DataObject> lobyData = SerializeLobbyData(data);

            UpdateLobbyOptions options = new UpdateLobbyOptions()
            {
                Data = lobyData  // Cập nhật dữ liệu của lobby
            };

            try
            {
                // Cập nhật dữ liệu của lobby
                _lobby = await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, options);
            }
            catch (System.Exception)
            {
                return false;  // Nếu có lỗi xảy ra, trả về false
            }

            LobbyEvents.OnLobbyUpdated(_lobby);  // Gọi sự kiện khi lobby được cập nhật
            return true;
        }

        // Lấy ID của người host trong lobby
        public string GetHostID()
        {
            return _lobby.HostId;  // Trả về ID của người tạo lobby (host)
        }
    }
}
