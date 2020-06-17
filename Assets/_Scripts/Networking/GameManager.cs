using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    /// <summary>
    /// Tracks PlayerManager components linked to a client ID
    /// </summary>
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab, playerPrefab;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying objects");
            Destroy(this);
        }
    }

    /// <summary>
    /// Spawns a player at a certain position and rotation and sets the spawned player's information in the PlayerManager
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <param name="username">Username</param>
    /// <param name="position">Position</param>
    /// <param name="rotation">Rotation</param>
    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation) {
        GameObject player;

        // If local player
        if (id == Client.instance.myId) {
            // Spawn local player prefab
            player = Instantiate(localPlayerPrefab, position, rotation);
		} else {
            // Spawn remote player prefab
            player = Instantiate(playerPrefab, position, rotation);
		}


        // Assigns information to the PlayerManager
        player.GetComponent<PlayerManager>().Initialize(id, username);

        // Adds a PlayerManager with the client ID as the key
        players.Add(id, player.GetComponent<PlayerManager>());
    }
}
