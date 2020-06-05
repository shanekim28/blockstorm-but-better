using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    // Start is called before the first frame update
    void SendInputToServer() {
        bool[] inputs = new bool[] {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.D)
        };

        ClientSend.PlayerMovement(inputs);
    }

    // Update is called once per frame
    void FixedUpdate() {
        SendInputToServer();
    }
}
