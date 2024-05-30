using Mirror;
using UnityEngine;

public class NetworkingManager : NetworkManager
{
    [SerializeField] LoginPopup _loginPopup;
    [SerializeField] ChattingUI _chattingUI;

    //네트워크 매니저의 networkAddress에 호스트 정보 세팅
    public void OnInputValueChanged_SetHostName(string hostName)
    {
        this.networkAddress = hostName;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if(_chattingUI != null)
        {
            _chattingUI.RemoveNameOnServerDisconnect(conn);
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        if(_loginPopup != null)
        {
            _loginPopup.SetUIOnClientDisconnected();
        }
    }
}

