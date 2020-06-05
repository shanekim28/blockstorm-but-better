﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour {
	private static void SendTCPData(Packet packet) {
		packet.WriteLength();
		Client.instance.tcp.SendData(packet);
	}
	
	private static void SendUDPData(Packet packet) {
		packet.WriteLength();
		Client.instance.udp.SendData(packet);
	}

	#region Packets
	public static void WelcomeReceived() {
		using (Packet packet = new Packet((int) ClientPackets.welcomeReceived)) {
			packet.Write(Client.instance.myId);
			packet.Write(UIManager.instance.usernameField.text);

			SendTCPData(packet);
		}
	}

	public static void PlayerMovement(bool[] inputs) {
		using (Packet packet = new Packet((int) ClientPackets.playerMovement)) {
			packet.Write(inputs.Length);
			//Debug.Log("Send movement data");
			foreach (bool input in inputs) {
				packet.Write(input);
			}

			packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

			SendUDPData(packet);
		}
	}
	#endregion
}
