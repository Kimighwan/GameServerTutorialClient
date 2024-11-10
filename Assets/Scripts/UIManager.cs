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
        if (instance == null) // ���� ��������� �ʾҴٸ� ���� ��ü �Ҵ�
        {
            instance = this;
        }
        else if (instance != this) // �̹� �����Ѵٸ� ���� ��ü ����
        {
            Destroy(this);
        }
    }

    public void ServerConnect()
    {
        start.SetActive(false); // ��Ȱ��ȭ
        userNameField.interactable = false;
        Client.instance.ConnectionServer();
    }
}