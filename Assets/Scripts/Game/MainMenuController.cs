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
        
        //Lang nghe cac nut khi nhan
        void OnEnable()
        {
            Host_bt.onClick.AddListener(OnHostClicked);
            Join_bt.onClick.AddListener(ONJoinClicked);
            submitCode_bt.onClick.AddListener(OnSubmitCodeClicked);
        }

        //Lang nghe cac nut khi khong nhan 
        void OnDisable()
        {
            Host_bt.onClick.RemoveListener(OnHostClicked);
            Join_bt.onClick.RemoveListener(ONJoinClicked);
            submitCode_bt.onClick.RemoveListener(OnSubmitCodeClicked);
        }

        //Su kien nhan nut Host
        private async void OnHostClicked()
        {
            bool succeeded = await GameLobbyManager.Instance.CreateLobby();
            if (succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }

        //Su kien an nut Join
        private void ONJoinClicked()
        {
            mainScreen.SetActive(false);
            joinScreen.SetActive(true);
        }

        // SU kien nhan nut Join o Lobyy
        private async void OnSubmitCodeClicked()
        {
            string code = codeText.text;
            code = code.Substring(0, code.Length - 1);

            bool succeedeed = await GameLobbyManager.Instance.JoinLobby(code);
            if (succeedeed)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }
    }
}
