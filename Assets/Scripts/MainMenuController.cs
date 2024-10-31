using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button Host_bt;
    [SerializeField] private Button Join_bt;
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject joinScreen;

    [SerializeField] private Button submitCode_bt;
    [SerializeField] private TextMeshProUGUI codeText;
    // Start is called before the first frame update
    void OnEnable()
    {
        Host_bt.onClick.AddListener(OnHostClicked);
        Join_bt.onClick.AddListener(ONJoinClicked);
        submitCode_bt.onClick.AddListener(OnSubmitCodeClicked);
    }

    void OnDisable()
    {
        Host_bt.onClick.RemoveListener(OnHostClicked);
        Join_bt.onClick.RemoveListener(ONJoinClicked);
        submitCode_bt.onClick.RemoveListener(OnSubmitCodeClicked);
    }

    private async void OnHostClicked() {
       bool succeeded = await GameLobbyManager.Instance.CreateLobby();
        if (succeeded)
        {
            SceneManager.LoadSceneAsync("Lobby");
        }
    }

    private void ONJoinClicked()
    {
        mainScreen.SetActive(false);
        joinScreen.SetActive(true);
    }

    // CLicked join button in JoinScreen
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
