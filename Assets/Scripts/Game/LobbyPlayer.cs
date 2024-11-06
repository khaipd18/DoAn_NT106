using GameFramework.Core.Data;
using TMPro;
using UnityEngine;

namespace Game
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private SpriteRenderer _isReadySpriteRenderer;

        private LobbyPlayerData _data;

        private void Start()
        {
        }

        public void SetData(LobbyPlayerData data)
        {
            _data = data;
            if (_playerName != null)
            {
                _playerName.text = _data.Gamertag;
                Debug.Log("Player name set to: " + _data.Gamertag); // Kiểm tra tên người chơi
            }
            else
            {
                Debug.LogError("Player name TextMeshProUGUI is not assigned.");
            }
            if (_data.IsReady)
            {
                if (_isReadySpriteRenderer != null)
                {
                    _isReadySpriteRenderer.color = Color.green;
                    Debug.Log("Player is ready. Setting color to green.");
                }
            }

            gameObject.SetActive(true);
        }
    }
}
