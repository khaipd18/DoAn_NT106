using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Game.Events.LobbyEvents;

namespace Game
{
    // Class này quản lý UI trong Lobby (phòng chờ) của trò chơi
    public class LobbyUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _lobbyCodeText; // Hiển thị mã phòng
        [SerializeField] private Button _readyButton; // Nút "Ready" (Sẵn sàng)
        [SerializeField] private Button _startButton; // Nút "Start" (Bắt đầu trò chơi)

        // Phương thức gọi khi đối tượng được kích hoạt (on enable)
        private void OnEnable()
        {
            // Đăng ký sự kiện cho nút "Ready"
            _readyButton.onClick.AddListener(OnReadyPressed);

            // Nếu người chơi là host (chủ phòng), đăng ký sự kiện cho nút "Start"
            if (GameLobbyManager.Instance.IsHost)
            {
                // Đăng ký sự kiện cho nút "Start"
                _startButton.onClick.AddListener(OnStartButtonClicked);
                // Đăng ký sự kiện "OnLobbyReady" để hiển thị nút Start khi phòng đã sẵn sàng
                Events.LobbyEvents.OnLobbyReady += OnLobbyReady;
            }

            // Đăng ký sự kiện "OnLobbyUpdated" để cập nhật UI khi có sự thay đổi trong lobby
            Events.LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        // Phương thức gọi khi đối tượng bị hủy (on disable)
        private void OnDisable()
        {
            // Hủy đăng ký sự kiện cho nút "Ready" và "Start"
            _readyButton.onClick.RemoveAllListeners();
            _startButton.onClick.RemoveAllListeners();
            // Hủy đăng ký các sự kiện từ Lobby
            Events.LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            Events.LobbyEvents.OnLobbyReady -= OnLobbyReady;
        }

        // Phương thức gọi khi màn hình bắt đầu (Start)
        void Start()
        {
            // Hiển thị mã phòng trong UI
            _lobbyCodeText.text = $"ID: {GameLobbyManager.Instance.GetLobbyCode()}";
        }

        // Phương thức xử lý khi người chơi nhấn nút "Ready"
        private async void OnReadyPressed()
        {
            Debug.Log("Ready button pressed");

            // Gọi GameLobbyManager để thiết lập trạng thái "Ready" cho người chơi
            bool succeeded = await GameLobbyManager.Instance.SetPlayerReady();

            // Nếu thành công, ẩn nút "Ready"
            if (succeeded)
            {
                _readyButton.gameObject.SetActive(false);
            }
        }

        // Phương thức này dường như chưa được triển khai
        private void OnLobbyUpate()
        {
            // Cập nhật UI hoặc thông tin phòng khi có sự thay đổi trong lobby
        }

        // Phương thức xử lý khi phòng chờ đã sẵn sàng (được gọi khi sự kiện OnLobbyReady xảy ra)
        private void OnLobbyReady()
        {
            // Hiển thị nút "Start" khi lobby đã sẵn sàng
            _startButton.gameObject.SetActive(true);
        }

        // Phương thức xử lý khi người chơi nhấn nút "Start"
        private async void OnStartButtonClicked()
        {
            // Gọi GameLobbyManager để bắt đầu trò chơi
            await GameLobbyManager.Instance.StartGame();
        }

        // Phương thức Update này không có nội dung, có thể xóa nếu không sử dụng
        void Update()
        {
            // Có thể sử dụng để cập nhật UI hoặc các trạng thái khác nếu cần
        }
    }
}
