using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPopup : MonoBehaviour
{
    [SerializeField] private InputField Input_NetworkAdress;    //SerializeField는 대문자로 시작하고 컴포넌트이름(약자)를 넣는것이 좋음.
    [SerializeField] private InputField Input_UserName;

    [SerializeField] private Button Btn_StartAsHostServer;  //변수명 명확한 의미를 가지는 것이 좋다.
    [SerializeField] private Button Btn_StartAsClient;

    [SerializeField] private Text Text_Error;

    private string _originNetworkAdress;    //전역변수 언더바 붙여서 선언함.
    // private string m_originNetworkAdress; 회사마다 다름.
    // private string originNetworkAddress; 지역변수는 보통 소문자로 시작함. 전역변수랑 구분하는 규칙이 있어야함.

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

    public void OnClick_StartAsHost()   //OnClick에서 참조할거라서 이런 이름.
    {

    }

    public void OnClick_StartAsClient()
    {

    }

    public void OnValueChanged_ToggleButton(string userName)
    {

    }
}
