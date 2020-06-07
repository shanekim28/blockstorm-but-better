using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public static UIManager instance; // Singleton

	public GameObject startMenu;
	public InputField usernameField;

    // Init singleton
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying objects");
            Destroy(this);
        }
    }

    // Connects to the server
    public void ConnectToServer() {
        // Hide the stuff
        startMenu.SetActive(false);
        usernameField.interactable = false;

        // Connect to the server via TCP
        Client.instance.ConnectToServer();
	}
}
