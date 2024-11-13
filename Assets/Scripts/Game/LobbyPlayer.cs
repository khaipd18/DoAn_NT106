using GameFramework.Core.Data;
using TMPro;
using UnityEngine;

namespace Game
{
    // Class này đại diện cho một người chơi trong lobby (phòng chơi)
    public class LobbyPlayer : MonoBehaviour
    {
        // Các đối tượng UI trong Unity để hiển thị tên người chơi và trạng thái sẵn sàng
        [SerializeField] private TextMeshProUGUI _playerName; // UI để hiển thị tên người chơi
        [SerializeField] private SpriteRenderer _isReadySpriteRenderer; // UI để hiển thị trạng thái sẵn sàng (ví dụ: biểu tượng màu xanh khi sẵn sàng)

        private LobbyPlayerData _data;  // Dữ liệu của người chơi trong lobby

        
        private void Start()
        {
            
        }

        // Phương thức này nhận vào dữ liệu của người chơi và cập nhật UI
        public void SetData(LobbyPlayerData data)
        {
            _data = data; // Lưu trữ dữ liệu của người chơi

            // Kiểm tra và cập nhật tên người chơi trên UI
            if (_playerName != null)
            {
                _playerName.text = _data.Gamertag; // Đặt tên người chơi vào TextMeshPro
                Debug.Log("Player name set to: " + _data.Gamertag); // In ra log để kiểm tra tên người chơi
            }
            else
            {
                // Nếu _playerName chưa được gán, in ra lỗi
                Debug.LogError("Player name TextMeshProUGUI is not assigned.");
            }

            // Kiểm tra xem người chơi có sẵn sàng hay không và thay đổi màu sắc nếu có
            if (_data.IsReady)
            {
                // Nếu trạng thái sẵn sàng, thay đổi màu sắc biểu tượng thành xanh lá cây
                if (_isReadySpriteRenderer != null)
                {
                    _isReadySpriteRenderer.color = Color.green;  // Đặt màu xanh lá cây khi người chơi sẵn sàng
                    Debug.Log("Player is ready. Setting color to green."); // In ra log khi người chơi sẵn sàng
                }
            }

            // Kích hoạt đối tượng trong scene để nó hiển thị
            gameObject.SetActive(true);
        }
    }
}
