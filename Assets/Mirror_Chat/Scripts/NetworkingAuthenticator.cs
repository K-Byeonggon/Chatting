
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Authenticator: 인증하는 사람이라는 뜻.
public partial class NetworkingAuthenticator : NetworkAuthenticator
{
    //연결 여부 관리 컨테이너
    readonly HashSet<NetworkConnection> _connectionsPendingDisconnect = new HashSet<NetworkConnection>();
    internal static readonly HashSet<string> _playerNames = new HashSet<string>();

    //Req = Request
    // AuthReqMsg 구조체는 클라이언트가 서버로 인증 요청을 보낼 때 사용된다.
    public struct AuthReqMsg : NetworkMessage
    {
        public string authUserName;
    }

    //Res = Respond
    // AuthResMsg 구조체는 서버가 클라이언트로 인증 응답을 보낼 때 사용된다.
    public struct AuthResMsg : NetworkMessage
    {
        public byte code;
        public string message;
    }

    #region ServerSide
    [UnityEngine.RuntimeInitializeOnLoadMethod]
    static void ResetStatics()
    {
    }

    //OnStartServer: 서버가 시작될 때 호출된다.
    public override void OnStartServer()
    {
        // 클라로부터 인증 요청 처리를 위한 핸들러 연결
        NetworkServer.RegisterHandler<AuthReqMsg>(OnAuthRequestMessage, false);
    }

    //OnStopServer: 서버가 중지될 때 호출된다.
    public override void OnStopServer()
    {
        // 등록된 핸들러 해제.
        NetworkServer.UnregisterHandler<AuthResMsg>();
    }

    //OnServerAuthenticate: 클라가 서버에 연결할 때 호출된다.
    public override void OnServerAuthenticate(NetworkConnectionToClient conn)
    {

    }

    public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthReqMsg msg)
    {
        // 클라 인증 요청 메세지 도착 시 처리

        Debug.Log($"인증 요청 : {msg.authUserName}");

        if (_connectionsPendingDisconnect.Contains(conn))
            return;

        //웹서버, DB, Playfab API 등을 호출해 인증 확인
        if (!_playerNames.Contains(msg.authUserName))
        {
            _playerNames.Add(msg.authUserName);

            // 대입한 인증 값은 Player.OnStartServer 시점에서 읽음
            conn.authenticationData = msg.authUserName;

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 100,
                message = "Auth Success"
            };

            conn.Send(authResMsg);
            ServerAccept(conn);
        }
        else
        {
            //중복이면 연결해제할 컨테이너에 추가.
            _connectionsPendingDisconnect.Add(conn);

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 200,
                message = "User Name alreadt in use! Try again!"
            };

            conn.Send(authResMsg);
            conn.isAuthenticated = false;
            //미인증 처리된 클라는 접속해제
            StartCoroutine(DelayedDisconnect(conn, 1f));
        }
    }

    //일정 시간 후 클라를 연결 해제하는 코루틴
    IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ServerReject(conn);

        yield return null;
        _connectionsPendingDisconnect.Remove(conn);
    }

    #endregion
}
