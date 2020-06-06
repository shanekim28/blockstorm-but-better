using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public PlayerManager player;
    public float sensitivity = 100f;
    public float clampAngle = 85f;

    private float verticalRotation;
    private float horizontalRotation;

    // Start is called before the first frame update
    void Start() {
        // Init rotations
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = player.transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleCursorMode();
		}

        if (Cursor.lockState == CursorLockMode.Locked) {
            Look(); // Apply rotation

		}

        Debug.DrawRay(transform.position, transform.forward * 2, Color.green); // To see direction of player
    }

    /// <summary>
    /// Gets player mouse input to apply rotations locally
    /// </summary>
    private void Look() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        verticalRotation += mouseY * sensitivity * Time.deltaTime;
        horizontalRotation += mouseX * sensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        player.transform.rotation = Quaternion.Euler(0, horizontalRotation, 0);
	}

    private void ToggleCursorMode() {
        Cursor.visible = !Cursor.visible;

        if (Cursor.lockState == CursorLockMode.None) {
            Cursor.lockState = CursorLockMode.Locked;
		} else {
            Cursor.lockState = CursorLockMode.None;
		}
	}
}
