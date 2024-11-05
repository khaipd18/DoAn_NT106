using GameFramework.Core.Data;
using TMPro;
using UnityEngine;

namespace Game
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        //[SerializeField] private 

        private LobbyPlayerData _data;

        public void SetData(LobbyPlayerData data)
        {
            _data = data;
            _playerName.text = _data.Gamertag;
            gameObject.SetActive(true);
        }
    }
}
