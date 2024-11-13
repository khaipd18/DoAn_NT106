using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button Host_bt; // Nút để tạo phòng mới (host game)
        [SerializeField] private Button Join_bt; // Nút để tham gia vào phòng đã có
        [SerializeField] private GameObject mainScreen; // Màn hình chính của menu
        [SerializeField] private GameObject joinScreen; // Màn hình nhập mã phòng để tham gia

        [SerializeField] private Button submitCode_bt; // Nút để gửi mã phòng đã nhập
        [SerializeField] private TextMeshProUGUI codeText; // Đối tượng TextMeshPro hiển thị mã phòng

        // Start is called before the first frame update

        void OnEnable()  // Gọi khi màn hình menu được kích hoạt, dùng để kết nối các nút với các sự kiện
        {
            Host_bt.onClick.AddListener(OnHostClicked); // Kết nối sự kiện khi nhấn nút "Host"
            Join_bt.onClick.AddListener(ONJoinClicked); // Kết nối sự kiện khi nhấn nút "Join"
            submitCode_bt.onClick.AddListener(OnSubmitCodeClicked); // Kết nối sự kiện khi nhấn nút "Submit Code"
        }

        void OnDisable() // Gọi khi đối tượng bị vô hiệu hóa, dùng để hủy các sự kiện listener
        {
            Host_bt.onClick.RemoveListener(OnHostClicked); // Hủy sự kiện "Host"
            Join_bt.onClick.RemoveListener(ONJoinClicked); // Hủy sự kiện "Join"
            submitCode_bt.onClick.RemoveListener(OnSubmitCodeClicked); // Hủy sự kiện "Submit Code"
        }


        private async void OnHostClicked() // Xử lý khi nhấn nút "Host" để tạo phòng mới
        {
            bool succeeded = await GameLobbyManager.Instance.CreateLobby(); // Gọi hàm tạo phòng từ GameLobbyManager, đợi hoàn tất
            if (succeeded) // Nếu tạo phòng thành công
            {
                SceneManager.LoadSceneAsync("Lobby"); // Chuyển sang scene "Lobby"
            }
        }

        private void ONJoinClicked() // Xử lý khi nhấn nút "Join" để hiển thị màn hình nhập mã phòng
        {
            mainScreen.SetActive(false); // Ẩn màn hình chính
            joinScreen.SetActive(true); // Hiển thị màn hình nhập mã phòng
        }

        private async void OnSubmitCodeClicked() // Xử lý khi nhấn nút "Submit Code" để tham gia phòng với mã đã nhập
        {
            string code = codeText.text; // Lấy mã phòng từ đối tượng TextMeshProUGUI
            code = code.Substring(0, code.Length - 1); // Loại bỏ ký tự cuối cùng trong chuỗi mã (nếu cần)

            bool succeeded = await GameLobbyManager.Instance.JoinLobby(code); // Gọi hàm tham gia phòng với mã code từ GameLobbyManager
            if (succeeded) // Nếu tham gia thành công
            {
                SceneManager.LoadSceneAsync("Lobby"); // Chuyển tới scene "Lobby"
            }
        }
    }
}
