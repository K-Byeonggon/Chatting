using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingManager : NetworkManager
{
    [SerializeField] LoginPopup _loginPopup;
    [SerializeField] ChattingUI _chattingUI;

    public void OnInputValueChanged_SetHostName(string hostName)
    {
        //OnInputValueChanged가 불릴때마다 networkAddress를 hostName으로 바꿔주세요.
        this.networkAddress = hostName;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
    }

}
