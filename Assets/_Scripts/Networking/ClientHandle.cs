using System;
using System.Net;
using UnityEngine;

public delegate void ShootEvent();

// TODO: Spawn graphics on player shoot

/// <summary>
/// Handles incoming packets from the server
/// </summary>
public class ClientHandle : MonoBehaviour {
	public static event ShootEvent OnShoot; // Raises the OnShoot event

	/// <summary>
	/// Handles the Welcome packet by getting a message and client ID from the server, then connects via UDP
	/// </summary>
	/// <param name="packet">The received Welcome packet</param>
	public static void Welcome(Packet packet) {
		// Read the message
		string msg = packet.ReadString();
		// Read the ID the server assigned
		int myId = packet.ReadInt();

		// Echo the server's message
		Debug.Log($"Message from server: {msg}");
		// Assign the client ID given by the server
		Client.instance.myId = myId;
		

		// Tell the server the welcome message has been received
		ClientSend.WelcomeReceived();

		// Connect via UDP
		Client.instance.udp.Connect(((IPEndPoint) Client.instance.tcp.socket.Client.LocalEndPoint).Port);
	}

	/// <summary>
	/// Handles the SpawnPlayer packet by telling the GameManager singleton to spawn a player with a given ID, username, position, and rotation
	/// </summary>
	/// <param name="packet">Contains ID, username, position, and rotation</param>
	public static void SpawnPlayer(Packet packet) {
		// Reads the id and username
		int id = packet.ReadInt();
		string username = packet.ReadString();

		// Reads the relevant position and rotation
		Vector3 position = packet.ReadVector3();
		Quaternion rotation = packet.ReadQuaternion();

		// Spawns the player and sets their id and username
		GameManager.instance.SpawnPlayer(id, username, position, rotation);
	}

	/// <summary>
	/// Handles the PlayerPosition packet by setting a player's position by client ID
	/// </summary>
	/// <param name="packet">Contains ID and position</param>
	public static void PlayerPosition(Packet packet) {
		// Read the ID and position
		int id = packet.ReadInt();
		Vector3 position = packet.ReadVector3();

		// Apply it by getting the PlayerManager's transform
		GameManager.players[id].transform.position = position;
	}

	/// <summary>
	/// Handles the PlayerRotation packet by setting a player's rotation by client ID
	/// </summary>
	/// <param name="packet">Contains ID and rotation</param>
	public static void PlayerRotation(Packet packet) {
		// Read the ID and rotation
		int id = packet.ReadInt();
		Quaternion rotation = packet.ReadQuaternion();

		// Apply it by getting the PlayerManager's transform
		GameManager.players[id].transform.rotation = rotation;
	}

	/// <summary>
	/// Handles the PlayerWallrunning packet by calling the CameraController's Wallrun method
	/// </summary>
	/// <param name="packet">Contains ID, wallrun direction, and a vector along the wall</param>
	public static void  PlayerWallrunning(Packet packet) {
		int id = packet.ReadInt();
		int direction = packet.ReadInt();
		Vector3 vectorAlongWall = packet.ReadVector3();

		GameManager.players[id].GetComponentInChildren<CameraController>().Wallrun(direction, vectorAlongWall);
	}

	/// <summary>
	/// Removes a player from the game
	/// </summary>
	/// <param name="packet">Contains ID of player to remove</param>
	public static void PlayerDisconnected(Packet packet) {
		int id = packet.ReadInt();

		Destroy(GameManager.players[id].gameObject);
		GameManager.players.Remove(id);
	}

	/// <summary>
	/// Sets the health of a player
	/// </summary>
	/// <param name="packet">Contains ID and health of player</param>
	public static void PlayerHealth(Packet packet) {
		int id = packet.ReadInt();
		float health = packet.ReadFloat();

		GameManager.players[id].SetHealth(health);
	}

	/// <summary>
	/// Respawns a player by calling the GameManager's Respawn method
	/// </summary>
	/// <param name="packet">Contains ID of player to respawn</param>
	public static void PlayerRespawned (Packet packet) {
		int id = packet.ReadInt();

		GameManager.players[id].Respawn();
	}

	/// <summary>
	/// Animates player movement
	/// </summary>
	/// <param name="packet">Contains ID and animation state</param>
	public static void PlayerMovementAnimation(Packet packet) {
		int id = packet.ReadInt();
		int animationState = packet.ReadInt();
		GameManager.players[id].AnimateMovement(animationState);
	}

	/// <summary>
	/// Animates player shooting
	/// </summary>
	/// <param name="packet">Contains ID</param>
	public static void PlayerShoot(Packet packet) {
		int id = packet.ReadInt();
		int ammo = packet.ReadInt();
		Vector3 direction = packet.ReadVector3();

		// If local player
		if (id == Client.instance.myId)
			GameManager.players[id].AnimateShoot();

		// TODO: Add non-local player animations
		GameManager.players[id].Shoot(id, ammo, direction);
		OnShoot?.Invoke();
	}

	/// <summary>
	/// Animates player reloading
	/// </summary>
	/// <param name="packet">Contains ID</param>
	public static void PlayerReload(Packet packet) {
		int id = packet.ReadInt();
		int ammo = packet.ReadInt();
		float reloadTime = packet.ReadFloat();

		// If local player
		if (id == Client.instance.myId) {
			GameManager.players[id].AnimateReload();
		}

		// TODO: Add non-local player animations

		GameManager.players[id].Reload(ammo, reloadTime);
	}

	/// <summary>
	/// Plays an audio clip at a specified location or player location
	/// </summary>
	/// <param name="packet">Contains ID, audio clip to play, and optional location</param>
	public static void PlayAudio(Packet packet) {
		int id = packet.ReadInt();
		string audioClip = packet.ReadString();
		float volume = packet.ReadFloat();
		float pitch = packet.ReadFloat();
		Vector3? location;

		// If we can read a Vector3 from the packet, we can play a clip at that location
		try {
			location = packet.ReadVector3();
		} catch (Exception e) {
			location = null;
		}

		if (!location.HasValue) {
			AudioManager.instance.PlaySound(audioClip, id, volume, pitch);
		} else {
			AudioManager.instance.PlaySound(audioClip, location ?? Vector3.zero, volume, pitch);
		}
	}
}
