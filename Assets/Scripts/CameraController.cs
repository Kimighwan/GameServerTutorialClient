using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerManager player;
    public float sensitivity = 100f;
    public float clampAngel = 85f;

    private float verticalRoation;
    private float horizontalRoation;

    private void Start()
    {
        verticalRoation = transform.localEulerAngles.x;
        horizontalRoation = transform.localEulerAngles.y;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleCursorMode();

        if (Cursor.lockState == CursorLockMode.Locked)
            Look();
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }

    private void Look()
    {
        float mouseVertical = -Input.GetAxis("Mouse Y");
        float mouseHorizontal = Input.GetAxis("Mouse X");


        verticalRoation += mouseVertical * sensitivity * Time.deltaTime;
        horizontalRoation += mouseHorizontal * sensitivity * Time.deltaTime;

        verticalRoation = Mathf.Clamp(verticalRoation, - clampAngel, clampAngel);

        transform.localRotation = Quaternion.Euler(verticalRoation, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontalRoation, 0f);
    }

    private void ToggleCursorMode()
    {
        Cursor.visible = !Cursor.visible;

        if(Cursor.lockState == CursorLockMode.None)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
}
