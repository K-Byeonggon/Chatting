using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LoginPopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] internal InputField Input_NetworkAddress;
    [SerializeField] internal InputField Input_UserName;

    [SerializeField] internal Button Btn_StartAsHostServer;
    [SerializeField] internal Button Btn_StartAsClient;

    [SerializeField] internal Text Text_Error;

    [SerializeField] NetworkingManager _netManager;

    public static LoginPopup Instance { get; private set; }

    private string _originNetworkAddress;

    private void Awake()
    {
        Instance = this;
        Text_Error.gameObject.SetActive(false);
    }

    private void Start()
    {
        SetDefaultNetworkAddress();
    }

    private void OnEnable()
    {
        //InputField의 onValueChanged이벤트가 발생할 때마다 호출될 함수를 추가한다.
        Input_UserName.onValueChanged.AddListener(OnValueChanged_ToggleButton);
    }

    private void OnDisable()
    {
        Input_UserName.onValueChanged.RemoveListener(OnValueChanged_ToggleButton);
    }

    private void Update()
    {
        CheckNetworkAddressValidOnUpdate();
    }

    private void SetDefaultNetworkAddress()
    {
        //네트워크 주소가 없는 경우, 디폴트 세팅
        //NetworkManager.singleton.networkAddress는 주소. 클라이언트가 서버에 연결할 때 쓰는.
        if (string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = "localhost";
        }

        //네트워크 주소 공란으로 변경될 경우를 대비해 기존 네트워크 주소 보관
        _originNetworkAddress = NetworkManager.singleton.networkAddress;
    }

    // 확인한다. 네트워크 주소가 유효한지. Update()에서
    // 로그인 팝업 네트워크 주소 변경 감지 처리
    private void CheckNetworkAddressValidOnUpdate()
    {
        if (string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = _originNetworkAddress;
        }

        if (Input_NetworkAddress.text != NetworkManager.singleton.networkAddress)
        {
            Input_NetworkAddress.text = NetworkManager.singleton.networkAddress;
        }
    }

    //클라연결이 끊겼을때, UI세팅
    public void SetUIOnClientDisconnected()
    {
        //클라연결이 끊기면 로그인창이 뜬다.
        this.gameObject.SetActive(true);
        Input_UserName.text = string.Empty;
        Input_UserName.ActivateInputField();
    }

    public void SetUIOnAuthValueChanged()
    {
        Text_Error.text = string.Empty;
        Text_Error.gameObject.SetActive(false);
    }

    public void SetUIOnAuthError(string msg)
    {
        Text_Error.text = msg;
        Text_Error.gameObject.SetActive(true);
    }

    //InputField의 onValueChanged이벤트가 발생할 때 호출될 함수이다.
    public void OnValueChanged_ToggleButton(string userName)
    {
        //유저 이름 빈칸이 아닐때만 유효하도록. 유효하면 버튼이 눌려짐.
        bool isUserNameValid = !string.IsNullOrWhiteSpace(userName);
        Btn_StartAsHostServer.interactable = isUserNameValid;
        Btn_StartAsClient.interactable = isUserNameValid;
    }

    public void OnClick_StartAsHost()
    {
        if (_netManager == null)
            return;

        _netManager.StartHost();
        //호스트 되면 로그인창은 사라져야지
        this.gameObject.SetActive(false);
    }

    public void OnClick_StartAsClient()
    {
        if (_netManager == null)
            return;

        _netManager.StartClient();
        //클라에서도 채팅 시작되면 로그인창 사라져야지
        this.gameObject.SetActive(false);
    }

}
