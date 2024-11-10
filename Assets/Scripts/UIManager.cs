using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject start;
    public InputField userNameField;

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

    public void ServerConnect()
    {
        start.SetActive(false); // 비활성화
        userNameField.interactable = false;
        Client.instance.ConnectionServer();
    }
}