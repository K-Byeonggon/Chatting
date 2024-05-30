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
        //InputField�� onValueChanged�̺�Ʈ�� �߻��� ������ ȣ��� �Լ��� �߰��Ѵ�.
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
        //��Ʈ��ũ �ּҰ� ���� ���, ����Ʈ ����
        //NetworkManager.singleton.networkAddress�� �ּ�. Ŭ���̾�Ʈ�� ������ ������ �� ����.
        if (string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = "localhost";
        }

        //��Ʈ��ũ �ּ� �������� ����� ��츦 ����� ���� ��Ʈ��ũ �ּ� ����
        _originNetworkAddress = NetworkManager.singleton.networkAddress;
    }

    // Ȯ���Ѵ�. ��Ʈ��ũ �ּҰ� ��ȿ����. Update()����
    // �α��� �˾� ��Ʈ��ũ �ּ� ���� ���� ó��
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

    //Ŭ�󿬰��� ��������, UI����
    public void SetUIOnClientDisconnected()
    {
        //Ŭ�󿬰��� ����� �α���â�� ���.
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

    //InputField�� onValueChanged�̺�Ʈ�� �߻��� �� ȣ��� �Լ��̴�.
    public void OnValueChanged_ToggleButton(string userName)
    {
        //���� �̸� ��ĭ�� �ƴҶ��� ��ȿ�ϵ���. ��ȿ�ϸ� ��ư�� ������.
        bool isUserNameValid = !string.IsNullOrWhiteSpace(userName);
        Btn_StartAsHostServer.interactable = isUserNameValid;
        Btn_StartAsClient.interactable = isUserNameValid;
    }

    public void OnClick_StartAsHost()
    {
        if (_netManager == null)
            return;

        _netManager.StartHost();
        //ȣ��Ʈ �Ǹ� �α���â�� ���������
        this.gameObject.SetActive(false);
    }

    public void OnClick_StartAsClient()
    {
        if (_netManager == null)
            return;

        _netManager.StartClient();
        //Ŭ�󿡼��� ä�� ���۵Ǹ� �α���â ���������
        this.gameObject.SetActive(false);
    }

}
