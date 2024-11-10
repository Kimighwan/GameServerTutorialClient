using GameServer;
using System.Collections;
using System.Collections.Generic;
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
    }
}
