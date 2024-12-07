using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerManager player;
    
    public float sensitivity = 150f; // ����
    public float clampAngel = 100f;

    private float verticalRoation;
    private float horizontalRoation;

    private void Start()
    {
        verticalRoation = transform.localEulerAngles.x;
        horizontalRoation = transform.localEulerAngles.y;
    }

    private void Update()
    {
        Look();
        Cursor.lockState = CursorLockMode.Locked;   // ���콺 Ŀ���� ������ ���߾ӿ� ����, Ŀ���� ������ �ʰ� ����

        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }

    private void Look()
    {
        float mouseVertical = -Input.GetAxis("Mouse Y");
        float mouseHorizontal = Input.GetAxis("Mouse X");


        verticalRoation += mouseVertical * sensitivity * Time.deltaTime * 7;
        horizontalRoation += mouseHorizontal * sensitivity * Time.deltaTime * 7;

        verticalRoation = Mathf.Clamp(verticalRoation, - clampAngel, clampAngel);

        transform.localRotation = Quaternion.Euler(verticalRoation, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontalRoation, 0f);
    }
}
