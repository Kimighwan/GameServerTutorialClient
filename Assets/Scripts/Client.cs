using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System;
using GameServer;

public class Client : MonoBehaviour
{
    public static Client instance; // 싱글턴 패턴
    public static int bufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int id = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake() // 초기화 생명 주기 함수
    {
        if (instance == null) // 아직 만들어지지 않았다면 현재 객체 할당
        {
            instance = this;
        }
        else if (instance != this) // 이미 존재한다면 현재 객체 삭제
        {
            Destroy(this);
        }
    }


    void Start() // 시작시 동작하는 생명 주기 함수
    {
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit() // 게임 종료시 작동하는 함수
    {
        Disconnect();
    }

    public void ConnectionServer()
    {
        InitClientData();

        isConnected = true; // 연결 상태..
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receiveData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient()
            {
                ReceiveBufferSize = bufferSize,
                SendBufferSize = bufferSize
            };

            receiveBuffer = new byte[bufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket);
        }

        private void ConnectCallBack(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
                return;

            stream = socket.GetStream();

            receiveData = new Packet();

            stream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveCallBack, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch(Exception e)
            {
                Debug.Log($"Error Sending data To Server TCP : {e}");
            }
        }


        private void ReceiveCallBack(IAsyncResult _result)
        {
            try
            {
                int byteLength = stream.EndRead(_result);
                if (byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                receiveData.Reset(HandleData(data));
                stream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveCallBack, null);
            }
            catch (Exception e)
            {
                Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0;

            receiveData.SetBytes(data);

            if (receiveData.UnreadLength() >= 4)
            {
                packetLength = receiveData.ReadInt();
                if (packetLength <= 0)
                    return true;
            }

            while (packetLength > 0 && packetLength <= receiveData.UnreadLength())
            {
                byte[] packetBytes = receiveData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        packetHandlers[packetId](packet);
                    }
                });

                packetLength = 0;
                if (receiveData.UnreadLength() >= 4)
                {
                    packetLength = receiveData.ReadInt();
                    if (packetLength <= 0)
                        return true;
                }
            }

            if(packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        private void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receiveData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int localPort)
        {
            socket = new UdpClient(localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet packet = new Packet())
            {
                SendData(packet);
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(instance.id);
                if(socket != null)
                {
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error sending data to server using UDP : {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                byte[] data = socket.EndReceive(result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if(data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(data);
            }
            catch (Exception ex)
            {
                Disconnect();
            }
        }

        private void HandleData(byte[] data)
        {
            using (Packet packet = new Packet(data))
            {
                int packetLength = packet.ReadInt();
                data = packet.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(data))
                {
                    int packetId = packet.ReadInt();
                    packetHandlers[packetId](packet);
                }
            });
        }

        private void Disconnect()
        {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }

    private void InitClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome, ClientHandle.Welcome },
            {(int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            {(int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
            {(int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            {(int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
            {(int)ServerPackets.playerHP, ClientHandle.PlayerHP },
            {(int)ServerPackets.playerReSpawned, ClientHandle.PlayerReSpawned },
            {(int)ServerPackets.createItemSpawner , ClientHandle.CreateItemSpawner },
            {(int)ServerPackets.itemSpawned , ClientHandle.ItemSpawned },
            {(int)ServerPackets.itemPickedUp , ClientHandle.ItemPickedUp },
            {(int)ServerPackets.spawnProjectile , ClientHandle.SpawnProjectile },
            {(int)ServerPackets.projectilePostion , ClientHandle.ProjectilePosition},
            {(int)ServerPackets.projectileExploded , ClientHandle.ProjectileExploded},
        };
        Debug.Log("Init Packet");
    }

    private void Disconnect()
    {
        if (isConnected) // 이미 종료되어 실행됐을 때 예외 처리
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected from server.");
        }
    }
}