using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPopup : MonoBehaviour
{
    [SerializeField] private InputField Input_NetworkAdress;    //SerializeField�� �빮�ڷ� �����ϰ� ������Ʈ�̸�(����)�� �ִ°��� ����.
    [SerializeField] private InputField Input_UserName;

    [SerializeField] private Button Btn_StartAsHostServer;  //������ ��Ȯ�� �ǹ̸� ������ ���� ����.
    [SerializeField] private Button Btn_StartAsClient;

    [SerializeField] private Text Text_Error;

    private string _originNetworkAdress;    //�������� ����� �ٿ��� ������.
    // private string m_originNetworkAdress; ȸ�縶�� �ٸ�.
    // private string originNetworkAddress; ���������� ���� �ҹ��ڷ� ������. ���������� �����ϴ� ��Ģ�� �־����.

    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        
    }

    public void OnClick_StartAsHost()   //OnClick���� �����ҰŶ� �̷� �̸�.
    {

    }

    public void OnClick_StartAsClient()
    {

    }

    public void OnValueChanged_ToggleButton(string userName)
    {

    }
}
