using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour {
    public static Client instance; // Singleton
    public static int dataBufferSize = 4096; // Maximum buffer size

    public string ip = "127.0.0.1"; // IP to connect to
    public int port = 11000; // Desired port
    public int myId = 0; // Client ID (to be assigned by server)
    public TCP tcp; // TCP socket
    public UDP udp; // UDP socket

    private bool isConnected = false; // Check if connected

    private delegate void PacketHandler(Packet packet); // A delegate to handle the packets. Passes the packet to its respective ClientHandle method
    private static Dictionary<int, PacketHandler> packetHandlers; // Stores the IDs of packet types

    // Init singleton
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying objects");
            Destroy(this);
        }
    }

    // Init TCP and UDP sockets
    void Start() {
        tcp = new TCP();
        udp = new UDP();
    }

    /// <summary>
    /// Called when the application exits
    /// </summary>
	private void OnApplicationQuit() {
        Disconnect();
	}

    /// <summary>
    /// Connects to the server via TCP
    /// </summary>
	public void ConnectToServer() {
        InitializeClientData();

        isConnected = true;
        tcp.Connect();
	}

    /// <summary>
    /// TCP connection
    /// </summary>
    public class TCP {
        public TcpClient socket; // TCP socket

        private NetworkStream stream; // Datastream
        private Packet receivedData; // Received packet
        private byte[] receiveBuffer; // Received byte buffer

        /// <summary>
        /// Creates a new TcpClient and initializes it, then asynchronously connects
        /// </summary>
        public void Connect() {
            // Init TCP socket
            socket = new TcpClient {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            // Set the receive buffer to the size of the data buffer
            receiveBuffer = new byte[dataBufferSize];
            // Async connect to the destination
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        /// <summary>
        /// Called when the TCP socket finishes connecting.
        /// Preps the TCP stream to receive information
        /// </summary>
        /// <param name="result">The connection result</param>
        private void ConnectCallback(IAsyncResult result) {
            // Finish connect
            socket.EndConnect(result);

            // If the socket couldn't connect, do nothing
            if (!socket.Connected) {
                return;
            }

            // Get the connected socket stream
            stream = socket.GetStream();

            // Initialize the received data with a packet
            receivedData = new Packet();

            // Start reading data
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        /// <summary>
        /// Writes a packet to the stream
        /// </summary>
        /// <param name="packet"></param>
        public void SendData(Packet packet) {
            try {
                if (socket != null) {
                    // Write the packet
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
				}
			} catch (Exception e) {
                Debug.Log($"Error sending data to server via TCP: {e}");
			}
		}

        /// <summary>
        /// Called when the client receives data
        /// </summary>
        /// <param name="result">The receiving result</param>
        private void ReceiveCallback(IAsyncResult result) {
            try {
                // The number of bytes read from the stream
                int byteLength = stream.EndRead(result);

                // Disconnect if we received no data
                if (byteLength <= 0) {
                    instance.Disconnect();
                    return;
                }

                // Write the contents of the received data to a different array
                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                // Reset the data if there's no more data to read
                receivedData.Reset(HandleData(data));

                // Listen again
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            } catch (Exception e) {
                Console.WriteLine("Error receiving TCP data: " + e.ToString());
                Disconnect();
            }
        }

        /// <summary>
        /// Interprets the data read from the stream
        /// </summary>
        /// <param name="data">Data to read</param>
        /// <returns>True if there is no more data to be read</returns>
        private bool HandleData(byte[] data) {
            int packetLength = 0;

            // Prepare the packet to be read
            receivedData.SetBytes(data);
            // If the data has greater than 4 bytes of unread data
            if (receivedData.UnreadLength() >= 4) {
                // Read the length of the packet
                packetLength = receivedData.ReadInt();
                // If the packet has no data to read
                if (packetLength <= 0) {
                    // Reset the packet
                    return true;
				}
			}

            // If there is more data to read
            while (packetLength > 0 && packetLength <= receivedData.UnreadLength()) {
                // Get the data in byte form
                byte[] packetBytes = receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() => {
                    using (Packet packet = new Packet(packetBytes)) {
                        // Read the packet ID
                        int packetId = packet.ReadInt();
                        // Give the packet to the proper ClientHandle method via the packetId
                        packetHandlers[packetId](packet);
					}
                });

                packetLength = 0;
                // Check if the data has more unread data
                if (receivedData.UnreadLength() >= 4) {
                    // Read length of the packet again
                    packetLength = receivedData.ReadInt();
                    // If the packet has no data to read
                    if (packetLength <= 0) {
                        // Reset the packet
                        return true;
                    }
                }
            }
            
            if (packetLength <= 1) {
                return true;
			}

            // Otherwise, don't reset the packet and read through it again
            return false;
		}

        private void Disconnect() {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
		}
    }

    /// <summary>
    /// UDP connection (intialized after TCP)
    /// </summary>
    public class UDP {
        public UdpClient socket; // UDP socket
        public IPEndPoint endpoint; // Target IP

        /// <summary>
        /// Sets the target destination to the Client instance's specified IP and port
        /// </summary>
        public UDP() {
            endpoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
		}

        /// <summary>
        /// Connects to the server via UDP
        /// </summary>
        /// <param name="localPort">Port to connect through</param>
        public void Connect(int localPort) {
            // Creates a socket with a port
            socket = new UdpClient(localPort);
            // Tries to connect to a target IP
            socket.Connect(endpoint);
            // Begin listening
            socket.BeginReceive(ReceiveCallback, null); 

            // Sends an empty packet
            using (Packet packet = new Packet()) {
                SendData(packet);
			}
		}

        /// <summary>
        /// Sends a packet
        /// </summary>
        /// <param name="packet">Packet to be sent</param>
        public void SendData(Packet packet) {
            try {
                // Insert the Client instance ID to the packet
                packet.InsertInt(instance.myId);
                if (socket != null) {
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
				}
			} catch (Exception e) {
                Debug.Log($"Error sending data to server via UDP: {e}");
			}
		}

        /// <summary>
        /// Called when the client receives data
        /// </summary>
        /// <param name="result">The receiving result</param>
        private void ReceiveCallback(IAsyncResult result) {
            try {
                // Stop receiving the datagram
                byte[] data = socket.EndReceive(result, ref endpoint);
                // Start listening again
                socket.BeginReceive(ReceiveCallback, null);

                // Disconnect if we didn't receive a full 4 bytes of data
                if (data.Length < 4) {
                    instance.Disconnect();
                    return;
				}

                // Interpret the data
                HandleData(data);
			} catch (Exception e) {
                Console.WriteLine($"Error receiving UDP data: {e}");
                Disconnect();
			}
		}

        /// <summary>
        /// Interprets the data read from the datagram
        /// </summary>
        /// <param name="data">Data to read</param>
        /// <returns>True if there is no more data to be read</returns>
        private void HandleData(byte[] data) {
            // Creates a packet from the incoming data
            using (Packet packet = new Packet(data)) {
                // Read the length of the packet
                int packetLength = packet.ReadInt();
                // Read that amount of bytes
                data = packet.ReadBytes(packetLength);
			}


            ThreadManager.ExecuteOnMainThread(() => {
                // Create another new packet from the data
                using (Packet packet = new Packet(data)) {
                    // Reads the packet ID
                    int packetId = packet.ReadInt();
                    // Give the packet to its respective ClientHandle method via the packetId
                    packetHandlers[packetId](packet);
                }
            });
        }

        private void Disconnect() {
            instance.Disconnect();

            endpoint = null;
            socket = null;
		}
	}

    /// <summary>
    /// Initializes the packet handlers that the client will receive
    /// </summary>
    private void InitializeClientData() {
        packetHandlers = new Dictionary<int, PacketHandler>() {
            { (int) ServerPackets.welcome, ClientHandle.Welcome },
            { (int) ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },            
            { (int) ServerPackets.playerPosition, ClientHandle.PlayerPosition },            
            { (int) ServerPackets.playerRotation, ClientHandle.PlayerRotation},
        };

        Debug.Log("Initialized packets.");
	}

    /// <summary>
    /// Closes TCP and UDP sockets
    /// </summary>
    private void Disconnect() {
        if (isConnected) {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected from server.");
		}
	}
}
