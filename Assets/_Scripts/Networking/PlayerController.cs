﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    /// <summary>
    /// Gets input from player and sends it to the server
    /// </summary>
    void SendInputToServer() {
        bool[] inputs = new bool[] {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space)
        };

        ClientSend.PlayerMovement(inputs);
    }

    // Update is called once per frame
    void FixedUpdate() {
        SendInputToServer();
    }
}
