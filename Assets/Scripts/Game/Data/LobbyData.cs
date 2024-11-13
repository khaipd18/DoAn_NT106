using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;

namespace GameFramework.Core.Data
{
    // Lớp này quản lý dữ liệu của một lobby trong trò chơi, bao gồm mã tham gia relay và tên scene.
    public class LobbyData
    {
        private string _relayJoinCode;  // Mã tham gia relay để kết nối với phòng.
        private string _sceneName = "InGame";  // Tên của scene (mặc định là "InGame").

        // Property cho tên scene, cho phép truy cập và thay đổi giá trị.
        public string SceneName
        {
            get => _sceneName;
            set => _sceneName = value;
        }

        // Property cho mã relay join code, cho phép truy cập và thay đổi giá trị.
        public string RelayJoinCode
        {
            get => _relayJoinCode;
            set => _relayJoinCode = value;
        }

        // Phương thức khởi tạo dữ liệu lobby từ một Dictionary.
        public void Initialze(Dictionary<string, DataObject> lobbyData)
        {
            // Cập nhật trạng thái của lobby dựa trên dữ liệu được truyền vào.
            UpdateState(lobbyData);
        }

        // Phương thức cập nhật các giá trị của relayJoinCode và sceneName từ dữ liệu lobby.
        public void UpdateState(Dictionary<string, DataObject> lobbyData)
        {
            // Kiểm tra xem dictionary có chứa key "RelayJoinCode" không và nếu có, cập nhật giá trị.
            if (lobbyData.ContainsKey("RelayJoinCode"))
            {
                _relayJoinCode = lobbyData["RelayJoinCode"].Value;
            }

            // Kiểm tra xem dictionary có chứa key "SceneName" không và nếu có, cập nhật giá trị.
            if (lobbyData.ContainsKey("SceneName"))
            {
                _sceneName = lobbyData["SceneName"].Value;
            }
        }

        // Phương thức chuyển hóa dữ liệu lobby thành Dictionary để dễ dàng lưu trữ hoặc truyền tải.
        public Dictionary<string, string> Serialize()
        {
            // Trả về dictionary với các giá trị của relayJoinCode và sceneName.
            return new Dictionary<string, string>()
            {
                {"RelayJoinCode", _relayJoinCode},
                {"SceneName", _sceneName}
            };
        }
    }
}
