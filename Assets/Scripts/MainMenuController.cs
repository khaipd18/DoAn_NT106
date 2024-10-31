using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button Host_bt;
    [SerializeField] private Button Join_bt;

    // Start is called before the first frame update
    void Start()
    {
        Host_bt.onClick.AddListener(OnHostClicked);
        Join_bt.onClick.AddListener(ONJoinClicked);
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
        Debug.Log(message: "Join");
    }


}
