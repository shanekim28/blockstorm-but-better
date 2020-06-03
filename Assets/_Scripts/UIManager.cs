using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public static UIManager instance;

	public GameObject startMenu;
	public InputField usernameField;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying objects");
            Destroy(this);
        }
    }

    public void ConnectToServer() {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
	}
}
