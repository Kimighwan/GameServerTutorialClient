using GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
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

    public static void PlayerMovement(bool[] inputs)
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
