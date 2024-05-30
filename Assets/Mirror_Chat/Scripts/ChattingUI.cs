using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ChattingUI : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] Text Text_ChatHistory;
    [SerializeField] Scrollbar ScrollBar_Chat;
    [SerializeField] InputField Input_ChatMsg;
    [SerializeField] Button Btn_Send;

    internal static string _localPlayerName;

    // �����¸� - ����� �÷��̾�� �̸�
    // internal: ������ ����� �������� ���� ����. �Ƹ� �ٸ� Ŭ�󿡼� ������ ���� ���ϵ�.
    // ä��UI�� ����� �÷��̾� ������ ������ �����̳�(��ųʸ�). �÷��̾�� �̸��� ������.
    internal static readonly Dictionary<NetworkConnectionToClient, string> _connectedNameDic = new Dictionary<NetworkConnectionToClient, string>();

    public void SetLocalPlayerName(string userName)
    {
        _localPlayerName = userName;
    }

    public override void OnStartServer()
    {
        this.gameObject.SetActive(true);
        _connectedNameDic.Clear();
    }

    public override void OnStartClient()
    {
        this.gameObject.SetActive(true);
        Text_ChatHistory.text = string.Empty;
    }

    //Command: Ŭ�󿡼� ȣ�⸸�ϰ� �������� ����.
    //(requiresAuthority = false): ������ ���� Ŭ�󿡼��� ȣ��ɼ� �ִٴ� ��. 
    //����Ʈ NetworkConnectionToClient sender = null������,
    //Mirror�� ����� ���� Ŭ���� NetworkConnectionToClient��ü�� sender�� �ڵ����� �����Ѵ�
    [Command(requiresAuthority = false)]
    void CommandSendMsg(string msg, NetworkConnectionToClient sender = null)
    {
        //sender�� playerName�� Dictionary�� ����
        if(!_connectedNameDic.ContainsKey(sender))
        {
            var player = sender.identity.GetComponent<ChatUser>();
            var playerName = player.PlayerName;
            _connectedNameDic.Add(sender, playerName);
        }

        if (!string.IsNullOrWhiteSpace(msg))
        {
            var senderName = _connectedNameDic[sender];
            OnRecvMessage(senderName, msg.Trim());
        }
    }

    //������ ������ ��, �̸� ����.
    public void RemoveNameOnServerDisconnect(NetworkConnectionToClient conn)
    {
        _connectedNameDic.Remove(conn);
    }

    //�������� ��� Ŭ���̾�Ʈ�� Ư�� �Լ��� �����ų �� �ֵ��� [ClientRpc] ����.
    //CommandSendMsg���� OnRecvMessage �Լ� ȣ���� ��ε�ĳ���� �κ� �߰�?
    [ClientRpc]
    void OnRecvMessage(string senderName, string msg)
    {
        //���� �÷��̸� senderName�� ���������� ������.
        string formatedMsg = (senderName == _localPlayerName) ?
            $"<color=red>{senderName}:</color> {msg}"
            : $"<color=blue>{senderName}:</color> {msg}";

        AppendMessage(formatedMsg);
    }

    //======================================[UI]======================================
    void AppendMessage(string msg)
    {
        StartCoroutine(AppendAndScroll(msg));
    }

    IEnumerator AppendAndScroll(string msg)
    {
        //ä�� ��Ͽ� �޼��� �߰�. �ϳ��� �ؽ�Ʈ�� ���ุ �߰��ؼ� �޼������� ���� �ű���.
        Text_ChatHistory.text += msg + "\n";

        //�� ���� �������� ��ٸ��� �ؼ� UI������Ʈ�� �Ϸ�ǰ� �ϱ�.
        //Unity�� UI�ý����� ������ ������ ������Ʈ �Ǳ� ������ �ڷ�ƾ���� ����� ���ŵǰ� �Ѵ�.
        //UI������Ʈ�� �ڷ�ƾ���� �񵿱� ó���ϸ�, �÷��̾ ä���� ġ�� ������ ����ų� ���� ����.
        yield return null;
        yield return null;

        //��ũ�ѹٸ� ���� �Ʒ��� �����ؼ� ���� �ֱ� �޼��� ���̰� �ϱ�.
        ScrollBar_Chat.value = 0;
    }
    
    //================================================================================

    public void OnClick_SendMsg()
    {
        var currentChatMsg = Input_ChatMsg.text;
        if (!string.IsNullOrWhiteSpace(Input_ChatMsg.text))
        {
            CommandSendMsg(currentChatMsg.Trim());
        }
    }

    public void OnClick_Exit()
    {
        NetworkManager.singleton.StopHost();
    }

    //InputField���� OnValueChanged, OnSubmit, OnEndEdit �̺�Ʈ�� �ִ�.
    //OnValueChanged: �Է��ʵ��� ������ ����� ������ ȣ��
    //OnSubmit: �Է��ʵ忡 �ؽ�Ʈ �Է��� �Ϸ��ϰ� ������ �� ȣ��
    //OnEndEdit: �Է��� �Ϸ��ϰ� �Է� �ʵ忡�� ��Ŀ���� ��� �� ȣ��
    
    public void OnValueChanged_ToggleButton(string input)
    {
        Btn_Send.interactable = !string.IsNullOrWhiteSpace(input);
    }

    //OnEndEdit�� Enter�� ������ �߻��Ѵ�. â�� ����� ���� OnEndEdit�� �߻��Ѵ�.
    //OnEndEdit�� �߻��ϴ���, Enter�� ������ ���� �޼����� �����ؾ��ϱ� ������ �Ʒ��� ���� �ڵ带 �ۼ�.
    public void OnEndEdit_SendMsg(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetButtonDown("Submit"))
        {
            OnClick_SendMsg();
        }
    }
}
