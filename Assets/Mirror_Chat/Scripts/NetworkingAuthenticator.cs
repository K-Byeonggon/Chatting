
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Authenticator: �����ϴ� ����̶�� ��.
public partial class NetworkingAuthenticator : NetworkAuthenticator
{
    //���� ���� ���� �����̳�
    readonly HashSet<NetworkConnection> _connectionsPendingDisconnect = new HashSet<NetworkConnection>();
    internal static readonly HashSet<string> _playerNames = new HashSet<string>();

    //Req = Request
    // AuthReqMsg ����ü�� Ŭ���̾�Ʈ�� ������ ���� ��û�� ���� �� ���ȴ�.
    public struct AuthReqMsg : NetworkMessage
    {
        public string authUserName;
    }

    //Res = Respond
    // AuthResMsg ����ü�� ������ Ŭ���̾�Ʈ�� ���� ������ ���� �� ���ȴ�.
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

    //OnStartServer: ������ ���۵� �� ȣ��ȴ�.
    public override void OnStartServer()
    {
        // Ŭ��κ��� ���� ��û ó���� ���� �ڵ鷯 ����
        NetworkServer.RegisterHandler<AuthReqMsg>(OnAuthRequestMessage, false);
    }

    //OnStopServer: ������ ������ �� ȣ��ȴ�.
    public override void OnStopServer()
    {
        // ��ϵ� �ڵ鷯 ����.
        NetworkServer.UnregisterHandler<AuthResMsg>();
    }

    //OnServerAuthenticate: Ŭ�� ������ ������ �� ȣ��ȴ�.
    public override void OnServerAuthenticate(NetworkConnectionToClient conn)
    {

    }

    public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthReqMsg msg)
    {
        // Ŭ�� ���� ��û �޼��� ���� �� ó��

        Debug.Log($"���� ��û : {msg.authUserName}");

        if (_connectionsPendingDisconnect.Contains(conn))
            return;

        //������, DB, Playfab API ���� ȣ���� ���� Ȯ��
        if (!_playerNames.Contains(msg.authUserName))
        {
            _playerNames.Add(msg.authUserName);

            // ������ ���� ���� Player.OnStartServer �������� ����
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
            //�ߺ��̸� ���������� �����̳ʿ� �߰�.
            _connectionsPendingDisconnect.Add(conn);

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 200,
                message = "User Name alreadt in use! Try again!"
            };

            conn.Send(authResMsg);
            conn.isAuthenticated = false;
            //������ ó���� Ŭ��� ��������
            StartCoroutine(DelayedDisconnect(conn, 1f));
        }
    }

    //���� �ð� �� Ŭ�� ���� �����ϴ� �ڷ�ƾ
    IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ServerReject(conn);

        yield return null;
        _connectionsPendingDisconnect.Remove(conn);
    }

    #endregion
}
