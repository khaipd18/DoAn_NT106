using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Game.Events.LobbyEvents;

namespace Game
{
    public class LobbyUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _lobbyCodeText;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _startButton;

        private void OnEnable()
        {
            _readyButton.onClick.AddListener(OnReadyPressed);
            if (GameLobbyManager.Instance.IsHost)
            {
                Events.LobbyEvents.OnLobbyReady += OnLobbyReady;
            }
            Events.LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        private void OnDisable()
        {
            _readyButton.onClick.RemoveAllListeners();
            Events.LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            Events.LobbyEvents.OnLobbyReady -= OnLobbyReady;
        }

        void Start()
        {
            _lobbyCodeText.text = $"ID: {GameLobbyManager.Instance.GetLobbyCode()}"; //Hien thi code
        }

        private async void OnReadyPressed()
        {
            Debug.Log("Ready button pressed");
            bool succeeded = await GameLobbyManager.Instance.SetPlayerReady();
            if (succeeded)
            {
                _readyButton.gameObject.SetActive(false);
            }
        }

        private void OnLobbyUpate()
        {

        }

        private void OnLobbyReady()
        {
            _startButton.gameObject.SetActive(true);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
