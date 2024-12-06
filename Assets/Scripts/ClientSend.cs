using GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 네트워크를 통해 다양한 패킷을 생성하여 전송을 정의하는 클래스

public class ClientSend : MonoBehaviour
{
    // 아래의 Send 메서드 들은 전송할 패킷을을 준비하는 메소드
    // 클라이언트 버퍼에 전달받은 패킷 데이터를 저장

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

    #region Packet
    public static void WelcomeReceived() // 클라이언트가 환영 메세지를 받으면 서버로 보낼 패킷을 생성
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.id);
            packet.Write(UIManager.instance.userNameField.text);

            SendTCPData(packet);
        }
    }

    public static void PlayerMovement(bool[] inputs) // 플레이어가 입력한 이동 데이터 패킷 생성후 전송
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


    public static void PlayerShoot(Vector3 facing)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerShoot))
        {
            packet.Write(facing);

            SendTCPData(packet);
        }
    }

    public static void PlayerThrowItem(Vector3 facing)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            packet.Write(facing);

            SendTCPData(packet);
        }
    }
    #endregion
}
