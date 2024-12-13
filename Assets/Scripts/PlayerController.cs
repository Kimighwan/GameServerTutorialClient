using UnityEngine;

// 플레이어 컨트롤러

public class PlayerController : MonoBehaviour
{
    public Transform camTransform;  // 카메라 위치

    private void Update()   // 마우스 입력 감지
    {
        if (!GameManager.instance.gameStart) return;

        if(Input.GetKeyDown(KeyCode.Mouse0))    // 왼클릭시 총알 발사
        {
            ClientSend.PlayerShoot(camTransform.forward);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))   // 우클릭시 수류탄 사용
        {
            ClientSend.PlayerThrowItem(camTransform.forward);
        }
    }
    private void FixedUpdate()
    {
        if (!GameManager.instance.gameStart) return;

        SendInputToServer();
    }

    private void SendInputToServer()    // 입력을 받아 해당 정보 패킷 전송
    {
        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space)
        };

        ClientSend.PlayerMovement(inputs);
    }
}
