using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using GameServer;

public class Client : MonoBehaviour
{
    public static Client instance; // �̱��� ����
    public static int bufferSize = 4096;

    public string ip = "127.0.0.1"; // ���� host�� ���� ip
    public int port = 33374; // �ش� ��Ʈ�� ������ ���⿡ ��Ʈ��ȣ ����
    public int id = 0;
    public TCP tcp; // tcp ���� Ŭ���� �Ʒ����� ���� ����
    public UDP udp; // udp ���� Ŭ���� �Ʒ����� ���� ����

    private bool isConnected = false;
    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake() // ����Ƽ ������ ���� �ֱ� �Լ� -> ���� ����� �ʱ⿡ �� �� ����Ǵ� �Լ�
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


    void Start() // ���۽� �����ϴ� ���� �ֱ� �Լ�
    {
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit() // ���� ����� �۵��ϴ� ����Ƽ ������ �Լ�
    {
        Disconnect();
    }

    public void ConnectionServer() // ������ ���� ����
    {
        InitClientData();

        isConnected = true; // ���� ���·� ����
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket; // tcp ����

        private NetworkStream stream;
        private Packet receiveData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient()
            {
                ReceiveBufferSize = bufferSize, // �۽� ���� ũ�� ����
                SendBufferSize = bufferSize     // ���� ���� ũ�� ����
            };

            receiveBuffer = new byte[bufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket); // Ŭ���̾�Ʈ�� �������� ���� ��û ����
        }

        private void ConnectCallBack(IAsyncResult _result) // ���� �� ����Ǵ� �ݹ� �Լ�
        {
            socket.EndConnect(_result);

            if (!socket.Connected) // ������ �ȵǸ� �ٷ� ����
                return;

            stream = socket.GetStream(); // ������ ���� ��Ʈ�� �б�

            receiveData = new Packet();

            stream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveCallBack, null); // ��Ʈ���� ���� ���� �����͸� ���ۿ� ����
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null); // ��Ŷ�� �����͸� ��Ʈ������ ����
                }
            }
            catch(Exception e)
            {
                Debug.Log($"TCP�� ���� ������ ������ ���� : {e}");
            }
        }


        private void ReceiveCallBack(IAsyncResult _result) // ������ ReceiveCallBack�� ����
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

                receiveData.Reset(HandleData(data)); // ��Ʈ�� �б����� ���� �����͸� �缳���Ѵ�
                                                     // ���ŵ� ������ ���Ŀ� �°� ��Ŷ ���ۿ� �д´�.
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

            receiveData.SetBytes(data); // ��Ʈ������ ���� ����Ʈ�� ��Ŷ ũ�⸦ �缳��

            if (receiveData.UnreadLength() >= 4)
            {
                packetLength = receiveData.ReadInt();
                if (packetLength <= 0)
                    return true;
            }

            while (packetLength > 0 && packetLength <= receiveData.UnreadLength()) // ���� ���ο� ó���� �� �ִ� �����Ͱ� ��������
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
        public UdpClient socket; // udp ����
        public IPEndPoint endPoint; // ������ �ּҿ� ��Ʈ ��ȣ
                                    // IPEndPoint�� ip  + ��Ʈ ��ȣ�� ��Ÿ���� Ÿ���̴�.

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port); // ip�� port ��ȣ�� ���ο� IPEndPoint ����
                                                                                    // ���� IPEndPoint�� �ȴ�
        }

        public void Connect(int localPort)
        {
            socket = new UdpClient(localPort);

            socket.Connect(endPoint); // udp Ŭ���̾�Ʈ�� ���� ��Ʈ�� ���ε�
            socket.BeginReceive(ReceiveCallback, null); // udp ���� �޼��� ȣ��

            using (Packet packet = new Packet())
            {
                SendData(packet); // �������� ���� ����
                                  // ���� ��Ʈ�� ���� Ŭ���̾�Ʈ�� �޼����� ���� �� �ִ� ���°� �ȴ�
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(instance.id); // ��Ŷ�� Ŭ���̾�Ʈ ID ����
                                               // �������� �� ���� ����Ͽ� ���� ���� ������ Ȯ���� �����ϴ�.
                if(socket != null)
                {
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null); // ��Ŷ�� �޼��� ����
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"udp �����Ͱ� �������� ���۵��� ���� / {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                byte[] data = socket.EndReceive(result, ref endPoint); // ������ ���� �����͸� �о�帲
                socket.BeginReceive(ReceiveCallback, null); // �ٽ� �������� ������ �Ѵ�.

                if(data.Length < 4) // ��Ŷ�� ����� ���� ������ ���� ����
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
                int packetLength = packet.ReadInt(); // ������ ����Ʈ ����
                data = packet.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(data))
                {
                    int packetId = packet.ReadInt();
                    packetHandlers[packetId](packet); // ID�� �а� ������ ȣ��� ���� �� ��Ŷ�� ����
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
        if (isConnected) // ���� �����̸� ����
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("������ ���� ����.");
        }
    }
}