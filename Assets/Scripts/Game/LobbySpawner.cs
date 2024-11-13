using System.Collections.Generic;
using UnityEngine;
using Game.Events;
using System;
using GameFramework.Core.Data;

namespace Game
{
    public class LobbySpawner : MonoBehaviour
    {
        // Danh sách các đối tượng LobbyPlayer (có thể là UI hoặc các đối tượng trong game)
        [SerializeField] private List<LobbyPlayer> _players;

        // Đăng ký sự kiện khi đối tượng được kích hoạt
        private void OnEnable()
        {
            // Khi có sự kiện cập nhật Lobby, gọi phương thức OnLobbyUpdated
            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        // Hủy đăng ký sự kiện khi đối tượng bị hủy
        private void OnDisable()
        {
            // Khi sự kiện OnLobbyUpdated không còn sử dụng, bỏ đăng ký
            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
        }

        // Phương thức sẽ được gọi mỗi khi có sự thay đổi trong Lobby (ví dụ: người chơi sẵn sàng hoặc tham gia)
        private void OnLobbyUpdated()
        {
            // Lấy danh sách các dữ liệu người chơi từ GameLobbyManager
            List<LobbyPlayerData> playerDatas = GameLobbyManager.Instance.GetPlayers();

            // Lặp qua danh sách playerDatas để cập nhật thông tin cho từng LobbyPlayer trong danh sách _players
            for (int i = 0; i < playerDatas.Count; i++)
            {
                // Lấy thông tin người chơi tại vị trí i trong danh sách playerDatas
                LobbyPlayerData data = playerDatas[i];

                // Cập nhật thông tin cho LobbyPlayer tương ứng trong danh sách _players
                _players[i].SetData(data);
            }
        }
    }
}
