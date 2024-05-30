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

    // 서버온리 - 연결된 플레이어들 이름
    // internal: 동일한 어셈블리 내에서만 접근 가능. 아마 다른 클라에서 접근을 막는 거일듯.
    // 채팅UI에 연결된 플레이어 정보를 관리할 컨테이너(딕셔너리). 플레이어들 이름을 관리함.
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

    //Command: 클라에선 호출만하고 서버에서 동작.
    //(requiresAuthority = false): 권한이 없는 클라에서도 호출될수 있다는 뜻. 
    //디폴트 NetworkConnectionToClient sender = null이지만,
    //Mirror는 명령을 보낸 클라의 NetworkConnectionToClient객체를 sender에 자동으로 전달한다
    [Command(requiresAuthority = false)]
    void CommandSendMsg(string msg, NetworkConnectionToClient sender = null)
    {
        //sender와 playerName을 Dictionary에 보관
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

    //서버가 끊겼을 때, 이름 제거.
    public void RemoveNameOnServerDisconnect(NetworkConnectionToClient conn)
    {
        _connectedNameDic.Remove(conn);
    }

    //서버에서 모든 클라이언트에 특정 함수를 실행시킬 수 있도록 [ClientRpc] 붙임.
    //CommandSendMsg에서 OnRecvMessage 함수 호출해 브로드캐스팅 부분 추가?
    [ClientRpc]
    void OnRecvMessage(string senderName, string msg)
    {
        //로컬 플레이면 senderName을 빨간색으로 보여줌.
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
        //채팅 기록에 메세지 추가. 하나의 텍스트에 개행만 추가해서 메세지들을 띄우는 거구나.
        Text_ChatHistory.text += msg + "\n";

        //두 번의 프레임을 기다리게 해서 UI업데이트가 완료되게 하기.
        //Unity의 UI시스템이 프레임 단위로 업데이트 되기 때문에 코루틴으로 제대로 갱신되게 한다.
        //UI업데이트를 코루틴으로 비동기 처리하면, 플레이어가 채팅을 치고 있을때 끊기거나 하지 않음.
        yield return null;
        yield return null;

        //스크롤바를 가장 아래로 설정해서 가장 최근 메세지 보이게 하기.
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

    //InputField에는 OnValueChanged, OnSubmit, OnEndEdit 이벤트가 있다.
    //OnValueChanged: 입력필드의 내용이 변경될 때마다 호출
    //OnSubmit: 입력필드에 텍스트 입력을 완료하고 제출할 때 호출
    //OnEndEdit: 입력을 완료하고 입력 필드에서 포커스를 벗어날 때 호출
    
    public void OnValueChanged_ToggleButton(string input)
    {
        Btn_Send.interactable = !string.IsNullOrWhiteSpace(input);
    }

    //OnEndEdit은 Enter를 눌러도 발생한다. 창을 벗어났을 때도 OnEndEdit이 발생한다.
    //OnEndEdit이 발생하더라도, Enter를 눌렀을 때만 메세지를 전송해야하기 때문에 아래와 같은 코드를 작성.
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
