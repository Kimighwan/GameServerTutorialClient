using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using GameServer;

// 클라이언트 관리 클래스
// 클라이언트 정보를 저장하는 클래스

// 서버 파일에 있는 Client 클래스와 유사합니다.
// 서버 파일에 있는 스크립트들을 먼저 보시면 더 보기 편합니다.

public class Client : MonoBehaviour
{
    public static Client instance; // 싱글턴 패턴
    public static int bufferSize = 4096;

    public string ip = "127.0.0.1"; // 로컬 host인 서버 ip
    public int port = 33374; // 해당 포트로 서버를 열기에 포트번호 지정
    public int id = 0;
    public TCP tcp; // tcp 소켓 클래스 아래에서 직접 생성
    public UDP udp; // udp 소켓 클래스 아래에서 직접 생성

    private bool isConnected = false;   // 연결되어 있는가?
    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;   // 패킷 컨테이너

    private void Awake() // 유니티 엔진의 생명 주기 함수 -> 게임 실행시 초기에 한 번 실행되는 함수
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

    private void OnApplicationQuit() // 게임 종료시 작동하는 유니티 엔진의 함수
    {
        Disconnect();
    }

    public void ConnectionServer() // 서버와 연결 시작
    {
        tcp = new TCP();
        udp = new UDP();

        InitClientData();

        isConnected = true; // 연결 상태로 변경
        tcp.Connect();
    }

    public class TCP    // TCP 클래스
    {
        public TcpClient socket; // tcp 소켓

        private NetworkStream stream;
        private Packet receiveData;
        private byte[] receiveBuffer;   // 수신받을 데이터를 바이트 단위로 받는 버퍼

        public void Connect() // 연결 준비
        {
            socket = new TcpClient()
            {
                ReceiveBufferSize = bufferSize, // 송신 버퍼 크기 설정
                SendBufferSize = bufferSize     // 수신 버퍼 크기 설정
            };

            receiveBuffer = new byte[bufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket); // 클라이언트가 서버에게 연결 요청 시작
        }

        private void ConnectCallBack(IAsyncResult _result) // 연결 후 실행되는 콜백 함수
        {
            socket.EndConnect(_result);

            if (!socket.Connected) // 연결이 안되면 바로 종료
                return;

            stream = socket.GetStream(); // 소켓을 통해 스트림 읽기

            receiveData = new Packet();

            stream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveCallBack, null); // 스트림을 통해 읽은 데이터를 버퍼에 저장
        }

        public void SendData(Packet packet) // 패킷 읽기
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null); // 패킷의 데이터를 스트림으로 전달
                }
            }
            catch(Exception e)
            {
                Debug.Log($"TCP를 통해 데이터 보내기 실패 : {e}");
            }
        }


        private void ReceiveCallBack(IAsyncResult _result) // 서버의 ReceiveCallBack와 동일
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

                receiveData.Reset(HandleData(data)); // 스트림 읽기전에 수신 데이터를 재설정한다
                                                     // 수신된 데이터 형식에 맞게 패킷 버퍼에 읽는다.
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

            receiveData.SetBytes(data); // 스트림에서 읽은 바이트로 패킷 크기를 재설정

            if (receiveData.UnreadLength() >= 4)
            {
                packetLength = receiveData.ReadInt();
                if (packetLength <= 0)
                    return true;
            }

            while (packetLength > 0 && packetLength <= receiveData.UnreadLength()) // 아직 내부에 처리할 수 있는 데이터가 남아있음
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

        private void Disconnect()   // 연결 해제
        {
            instance.Disconnect();

            stream = null;
            receiveData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP    // UDP 클래스
    {
        public UdpClient socket; // udp 소켓
        public IPEndPoint endPoint; // 목적지 주소와 포트 번호
                                    // IPEndPoint는 ip  + 포트 번호를 나타내는 타입이다.

        public UDP()    // 생성자
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port); // ip와 port 번호로 새로운 IPEndPoint 생성
                                                                                    // 서버 IPEndPoint가 된다
        }

        public void Connect(int localPort)  // 연결 준비
        {
            socket = new UdpClient(localPort);

            socket.Connect(endPoint); // udp 클라이언트를 로컨 포트에 바인딩
            socket.BeginReceive(ReceiveCallback, null); // udp 수신 메서드 호출

            using (Packet packet = new Packet())
            {
                SendData(packet); // 서버와의 연결 시작
                                  // 로컬 포트를 열어 클라이언트가 메세지를 받을 수 있는 상태가 된다
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(instance.id); // 패킷에 클라이언트 ID 삽입
                                               // 서버에서 이 값을 사용하여 누가 보낸 것인지 확인이 가능하다.
                if(socket != null)
                {
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null); // 패킷에 메세지 전송
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"udp 데이터가 서버에게 전송되지 않음 / {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                byte[] data = socket.EndReceive(result, ref endPoint); // 소켓을 통해 데이터를 읽어드림
                socket.BeginReceive(ReceiveCallback, null); // 다시 소켓으로 수신을 한다.

                if(data.Length < 4) // 패킷이 제대로 오지 않으면 연결 끊기
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
                int packetLength = packet.ReadInt(); // 수신한 바이트 길이
                data = packet.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(data))
                {
                    int packetId = packet.ReadInt();
                    packetHandlers[packetId](packet); // ID를 읽고 적절한 호출로 통해 새 패킷을 생성
                }
            });
        }

        private void Disconnect()   // 연결 해제
        {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }

    private void InitClientData()  // 패킷들 준비
    {
        packetHandlers = new Dictionary<int, PacketHandler>()   // 패킷을 받음
        {
            {(int)ServerPackets.init, ClientHandle.Init },
            {(int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            {(int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            {(int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
            {(int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
            {(int)ServerPackets.playerHP, ClientHandle.PlayerHP },
            {(int)ServerPackets.playerReSpawned, ClientHandle.PlayerReSpawned },
            {(int)ServerPackets.createItemSpawner , ClientHandle.CreateItemSpawner },
            {(int)ServerPackets.itemSpawned , ClientHandle.ItemSpawned },
            {(int)ServerPackets.itemPickedUp , ClientHandle.ItemPickedUp },
            {(int)ServerPackets.spawnProjectile , ClientHandle.SpawnProjectile },
            {(int)ServerPackets.projectilePostion , ClientHandle.ProjectilePosition},
            {(int)ServerPackets.projectileExploded , ClientHandle.ProjectileExploded},
            {(int)ServerPackets.playerCheck , ClientHandle.PlayerCheck},
            {(int)ServerPackets.playerDieCount , ClientHandle.PlayerDieCount},
        };
        Debug.Log("Init Packet");
    }

    private void Disconnect()   // 연결 해제
    {
        if (isConnected) // 연결 상태이면 종료
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("서버로 부터 종료.");
        }
    }
}