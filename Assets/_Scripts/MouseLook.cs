using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

	float mouseSensitivity = 150f;

	public Transform playerBody;
	public PlayerMovement movement;
	PlayerMovement.PlayerState currentPlayerState;

	float xRotation = 0f;
	float yRotation = 0f;

	float lerpSpeed = 10f;

	void Start() {
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update() {
		currentPlayerState = movement.currentState;

		// Get mouse input values

		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		// Clamp X rotation

		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		// Apply rotation based on mouse movement
		// TODO - this monstrosity absolutely destroys performance. should fix.
		if (currentPlayerState == PlayerMovement.PlayerState.WallrunLeft) {
			yRotation += mouseX;
			yRotation = Mathf.Clamp(yRotation, -15f, 45f);
			transform.localRotation = Quaternion.Slerp(Quaternion.Euler(xRotation, yRotation, transform.localEulerAngles.z), Quaternion.Euler(xRotation, yRotation, -10f), Time.deltaTime * lerpSpeed);
		} else if (currentPlayerState == PlayerMovement.PlayerState.WallrunRight) {
			yRotation += mouseX;
			yRotation = Mathf.Clamp(yRotation, -45f, 15f);
			transform.localRotation = Quaternion.Slerp(Quaternion.Euler(xRotation, yRotation, transform.localEulerAngles.z), Quaternion.Euler(xRotation, yRotation, 10f), Time.deltaTime * lerpSpeed);
		} else {
			yRotation = 0;
			transform.localRotation = Quaternion.Slerp(Quaternion.Euler(xRotation, transform.localEulerAngles.y, transform.localEulerAngles.z), Quaternion.Euler(xRotation, 0f, 0f), Time.deltaTime * lerpSpeed);
			playerBody.Rotate(Vector3.up * mouseX);
		}
		// brb, gotta go throw up
	}
}
