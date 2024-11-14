using GameServer;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string m = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"Message from Server : {m}");
        Client.instance.id = id;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void UDPTest(Packet packet)
    {
        string m = packet.ReadString();

        Debug.Log($"Received packet using UDP. Contatis message : {m}");
        ClientSend.UDPTestReceived();
    }
}
