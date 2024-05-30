using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatUser : NetworkBehaviour
{
    //SyncVar: ���� ������ ��� Ŭ�� �ڵ� ����ȭ�ϴµ� ����
    //Ŭ�� ���� �����ϸ� �ȵǰ�, �������� �����ؾ���.
    [SyncVar]
    public string PlayerName;

    //ȣ��Ʈ �Ǵ� ���������� ȣ��Ǵ� �Լ�
    public override void OnStartServer()
    {
        //�������̵� ���۽� playerName�� �ش� ���� ����
        PlayerName = (string)connectionToClient.authenticationData;
    }

    //���� �÷��̾� �ΰ�� ���� ó��
    public override void OnStartLocalPlayer()
    {
        var objChatUI = GameObject.Find("ChattingUI");
        if(objChatUI != null)
        {
            var chattingUI = objChatUI.GetComponent<ChattingUI>();
            if(chattingUI != null)
            {
                chattingUI.SetLocalPlayerName(PlayerName);
            }
        }
    }
}
