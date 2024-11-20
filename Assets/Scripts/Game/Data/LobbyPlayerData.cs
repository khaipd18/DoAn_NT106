using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GameFramework.Core.Data
{
    public class LobbyPlayerData
    {
        // Các trường dữ liệu lưu thông tin người chơi trong lobby
        private string _id; // ID của người chơi
        private string _gamertag; // Tên hiển thị (gamertag) của người chơi
        private bool _isReady; // Trạng thái sẵn sàng (true nếu người chơi đã nhấn nút "Ready" trong lobby)

        // Các thuộc tính công khai để truy xuất giá trị của các trường trên
        public string Id => _id;
        public string Gamertag => _gamertag;

        // Thuộc tính để truy xuất và cập nhật trạng thái sẵn sàng của người chơi
        public bool IsReady
        {
            get => _isReady;
            set => _isReady = value;
        }

        // Phương thức khởi tạo người chơi với ID và gamertag
        public void Initialize(string id, string gamertag) //Được sử dụng khởi tạo thông tin nhân vật khi Host/Join 
        {
            _id = id;
            _gamertag = gamertag;
        }

        // Phương thức khởi tạo từ dữ liệu người chơi được lấy từ dictionary
        public void Initialize(Dictionary<string, PlayerDataObject> playerData)
        {
            UpdateState(playerData);
        }

        // Cập nhật trạng thái của người chơi từ dữ liệu dictionary
        public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
        {
            // Kiểm tra và cập nhật giá trị ID, Gamertag và trạng thái sẵn sàng từ dữ liệu người chơi
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
                // Cập nhật trạng thái sẵn sàng (True nếu "IsReady" có giá trị "True")
                _isReady = playerData["IsReady"].Value == "True";
            }
        }

        // Phương thức để chuyển đối tượng thành một dictionary có thể lưu trữ
        public Dictionary<string, string> Serialize()
        {
            // Trả về dictionary chứa các giá trị của người chơi dưới dạng chuỗi
            return new Dictionary<string, string>()
            {
                {"Id", _id},
                {"Gamertag", _gamertag},
                {"IsReady", _isReady.ToString()},
            };
        }
    }
}
