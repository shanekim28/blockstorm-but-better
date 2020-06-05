using UnityEngine;

public class ClientSend : MonoBehaviour {
	/// <summary>
	/// Sends a packet over TCP
	/// </summary>
	/// <param name="packet">Packet to be sent</param>
	private static void SendTCPData(Packet packet) {
		packet.WriteLength();
		Client.instance.tcp.SendData(packet);
	}
	
	/// <summary>
	/// Sends a packet over UDP
	/// </summary>
	/// <param name="packet">Packet to be sent</param>
	private static void SendUDPData(Packet packet) {
		// Write the length of the packet then send it
		packet.WriteLength();
		Client.instance.udp.SendData(packet);
	}

	#region Packets
	/// <summary>
	/// Creates a packet that notifies the server that the server's welcome message has been received
	/// </summary>
	public static void WelcomeReceived() {
		using (Packet packet = new Packet((int) ClientPackets.welcomeReceived)) {
			// Echo back the client's ID that the server gave it
			packet.Write(Client.instance.myId);
			// Write the desired username
			packet.Write(UIManager.instance.usernameField.text);

			SendTCPData(packet);
		}
	}

	/// <summary>
	/// Creates a packet with player inputs and sends it to the server
	/// </summary>
	/// <param name="inputs">Inputs to be sent</param>
	public static void PlayerMovement(bool[] inputs) {
		using (Packet packet = new Packet((int) ClientPackets.playerMovement)) {
			// Write how many inputs there are
			packet.Write(inputs.Length);
			// Write each boolean to the packet
			foreach (bool input in inputs) {
				packet.Write(input);
			}

			// Sends the client's rotation
			packet.Write(GameManager.players[Client.instance.myId].transform.rotation); // Client-authoritative

			SendUDPData(packet);
		}
	}
	#endregion
}
