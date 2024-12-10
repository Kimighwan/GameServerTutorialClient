using GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

// 패킷을 전송하는 메소드 함수들
// 패킷을 생성하는 메소드 함수들

// 위 두개의 작업을 하는 정의한 클래스

public class ClientSend : MonoBehaviour
{
    // 아래의 Send 메서드들은 패킷을 전송

    // 중요한 정보는 TCP를 이용하고
    // 반대로 중요하지 않거나 빠르게 통신이 필요한 경우는 UDP를 사용
    // 또 지속적으로 데이터를 전송하는 경우 데이터를 잃어도 계속 전송하기에 UDP를 사용

    private static void SendTCPData(Packet packet) // 패킷 전달
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }

    private static void SendUDPData(Packet packet) // 패킷 전달
    {
        packet.WriteLength();
        Client.instance.udp.SendData(packet);
    }

    #region Packet // 패킷에 필요한 데이터를 입력 // 위의 적절한 전송 메서드를 통해서 전송

    public static void WelcomeReceived() // 최초 접속시 클라이언트 ID와 플레이어 이름을 담은 패킷
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.id);
            packet.Write(UIManager.instance.userNameField.text);

            SendTCPData(packet);
        }
    }

    public static void PlayerMovement(bool[] inputs) // 플레이어가 입력한 이동 정보 패킷
    {
        using (Packet packet = new Packet((int)ClientPackets.playerMovement))
        {
            packet.Write(inputs.Length);
            foreach(bool input in inputs)
            {
                packet.Write(input);
            }
            packet.Write(GameManager.players[Client.instance.id].transform.rotation);

            SendUDPData(packet);
        }
    }


    public static void PlayerShoot(Vector3 facing)  // 플레이어가 총을 사용한 정보 패킷
    {
        using (Packet packet = new Packet((int)ClientPackets.playerShoot))
        {
            packet.Write(facing);

            SendTCPData(packet);
        }
    }

    public static void PlayerThrowItem(Vector3 facing) // 플레이어가 슈류탄을 사용한 정보
    {
        using (Packet packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            packet.Write(facing);

            SendTCPData(packet);
        }
    }
    #endregion
}
