using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject start; // 버튼과 userNameField 필드칸의 오브젝트
    public InputField userNameField; // 유저 이름을 적는 필드 칸

    private void Awake()
    {
        if (instance == null) // 아직 만들어지지 않았다면 현재 객체 할당
        {
            instance = this;
        }
        else if (instance != this) // 이미 존재한다면 현재 객체 삭제
        {
            Destroy(this);
        }
    }

    public void ServerConnect() // 유니티의 버튼에 연결하여 작동하는 함수
    {
        start.SetActive(false); // 비활성화
        userNameField.interactable = false; // 필드 칸에 더 이상 입력이 안되도록 처리
        Client.instance.ConnectionServer(); // 서버와 연결 시작
    }
}