using GameFramework.Core;
using GameFramework.Core.GameFramework.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameLobbyManager : Singleton<GameLobbyManager>
{
     
    public async Task<bool> CreateLobby()
    {
        Dictionary<string, string> playerDatas = new Dictionary<string, string>()
        {
            {"GamerTag", "HostPlayer"}
        };
        bool succeeded = await LobbyManager.Instance.CreateLobby(maxPlayers: 2, isPrivate: true, playerDatas);
        return succeeded;
    }

    public string GetLobbyCode()
    {
        return LobbyManager.Instance.GetLobbyCode();
    }
}
