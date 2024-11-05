using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LobbyUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _lobbyCodeText;
        [SerializeField] private Button _readyButton;

        /*private void OnEnable()
        {
            _readyButton.onClick.AddListener(OnReadyPressed);
        }

        private void OnDisable()
        {
            _readyButton.onClick.RemoveAllListeners();
        }*/

        void Start()
        {
            _lobbyCodeText.text = $"ID: {GameLobbyManager.Instance.GetLobbyCode()}"; //Hien thi code
        }

        private async void OnReadyPressed()
        {
            bool succeeded = await GameLobbyManager.Instance.SetPlayerReady();
            if (succeeded)
            {
                _readyButton.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
