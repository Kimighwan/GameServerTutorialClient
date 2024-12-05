using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject start; // ��ư�� userNameField �ʵ�ĭ�� ������Ʈ
    public InputField userNameField; // ���� �̸��� ���� �ʵ� ĭ

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

    public void ServerConnect() // ����Ƽ�� ��ư�� �����Ͽ� �۵��ϴ� �Լ�
    {
        start.SetActive(false); // ��Ȱ��ȭ
        userNameField.interactable = false; // �ʵ� ĭ�� �� �̻� �Է��� �ȵǵ��� ó��
        Client.instance.ConnectionServer(); // ������ ���� ����
    }
}