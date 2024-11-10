using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button Host_bt;
        [SerializeField] private Button Join_bt;
        [SerializeField] private GameObject mainScreen;
        [SerializeField] private GameObject joinScreen;

        [SerializeField] private Button submitCode_bt;
        [SerializeField] private TextMeshProUGUI codeText;
        // Start is called before the first frame update

        void OnEnable()  // Kết nối các nút với sự kiện tương ứng khi màn hình được kích hoạt
        {
            Host_bt.onClick.AddListener(OnHostClicked);
            Join_bt.onClick.AddListener(ONJoinClicked);
            submitCode_bt.onClick.AddListener(OnSubmitCodeClicked);
        }

        void OnDisable() // Hủy các listener khi đối tượng bị vô hiệu hóa
        {
            Host_bt.onClick.RemoveListener(OnHostClicked);
            Join_bt.onClick.RemoveListener(ONJoinClicked);
            submitCode_bt.onClick.RemoveListener(OnSubmitCodeClicked);
        }

        
        private async void OnHostClicked() // Tạo phòng mới và chuyển sang lobby nếu thành công
        {
            bool succeeded = await GameLobbyManager.Instance.CreateLobby();// Chờ thực hiện xong hàm...
            if (succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }
        
        private void ONJoinClicked() // Hiển thị màn hình nhập mã phòng khi nhấn nút Join
        {
            mainScreen.SetActive(false);
            joinScreen.SetActive(true);
        }

        private async void OnSubmitCodeClicked() // Tham gia phòng với mã code đã nhập và chuyển sang lobby nếu thành công
        {
            string code = codeText.text; // Lấy mã phòng từ TextMeshProUGUI
            code = code.Substring(0, code.Length - 1);

            bool succeedeed = await GameLobbyManager.Instance.JoinLobby(code); // Tham gia phòng với mã code
            if (succeedeed)
            {
                SceneManager.LoadSceneAsync("Lobby"); // Chuyển tới lobby sau khi tham gia phòng thành công
            }
        }
    }
}
