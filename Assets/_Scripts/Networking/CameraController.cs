using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public PlayerManager player;
    public GameObject viewModel;

    public float swayAmount = 2f;
    public float swaySpeed = 5f;
	public float maxSway = 0.06f;

    public float sensitivity = 100f;
    public float clampAngle = 85f;

    private float verticalRotation;
    private float horizontalRotation;
    private float tilt;
    private float wallrunHorizontalRotation;

    private Vector3 viewModelInitialPosition;

    private int wallrunning = 0;
	private Vector3 vectorAlongWall;

	// Start is called before the first frame update
	void Start() {
        // Init rotations
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = player.transform.eulerAngles.y;

        viewModelInitialPosition = viewModel.transform.localPosition;
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

        float angleOfWall;

        switch (wallrunning) {
            case -1:
                // Set the camera z-tilt angle
                tilt = Mathf.Lerp(GetSignedAngle(transform.localEulerAngles.z), -10, Time.deltaTime * 10);

                // Get the global angle of the wall in terms of Vector3.forward
                angleOfWall = Vector3.SignedAngle(Vector3.forward, vectorAlongWall + player.transform.right * 0.2f, Vector3.up) + 360; // Adding player.transform.right * 0.2f adjusts for the angle produced by keeping the player on the wall
                // Wrap the angle from 0 to 360
                angleOfWall = Mathf.Repeat(angleOfWall, 360);

                // Get the mouse input during the wallrun and clamp it
                wallrunHorizontalRotation += mouseX * sensitivity * Time.deltaTime;
                wallrunHorizontalRotation = Mathf.Clamp(wallrunHorizontalRotation, -5, 50);

                // Set the player's rotation to the angle of the wall
                horizontalRotation = Mathf.Lerp(horizontalRotation, angleOfWall, Time.deltaTime * 10);

                // If the player rotation is away from the wall
                if (horizontalRotation > angleOfWall)
                    // Add rotation away from the wall to the camera rotation
                    wallrunHorizontalRotation += (horizontalRotation - angleOfWall) * Time.deltaTime * 10;

                break;

            case 1:
                // Set the camera z-tilt angle
                tilt = Mathf.Lerp(GetSignedAngle(transform.localEulerAngles.z), 10, Time.deltaTime * 10);

                // Get the global angle of the wall in terms of Vector3.forward
                angleOfWall = Vector3.SignedAngle(Vector3.forward, vectorAlongWall - player.transform.right * 0.2f, Vector3.up) + 360;
                // Wrap the angle from 0 to 360
                angleOfWall = Mathf.Repeat(angleOfWall, 360);

                // Get the mouse input during the wallrun and clamp it
                wallrunHorizontalRotation += mouseX * sensitivity * Time.deltaTime;
                wallrunHorizontalRotation = Mathf.Clamp(wallrunHorizontalRotation, -50, 5);

                // Set the player's rotation to the angle of the wall
                horizontalRotation = Mathf.Lerp(horizontalRotation, angleOfWall, Time.deltaTime * 10);

                // If the player rotation is away from the wall
                if (horizontalRotation < angleOfWall)
                    // Add rotation away from the wall to the camera rotation
                    wallrunHorizontalRotation += (horizontalRotation - angleOfWall) * Time.deltaTime * 10;

                break;

            default:
                // Get mouse input and wrap it from 0 to 360
                horizontalRotation += mouseX * sensitivity * Time.deltaTime;
                horizontalRotation = Mathf.Repeat(horizontalRotation, 360);

                // Lerp the the camera rotation to 0
                wallrunHorizontalRotation = Mathf.Lerp(wallrunHorizontalRotation, 0, Time.deltaTime * 10);

                // Maintain the camera's direction by adding it to the mouse input
                horizontalRotation += wallrunHorizontalRotation * Time.deltaTime * 10;

                // Reset the camera z-tilt to 0
                tilt = Mathf.Lerp(GetSignedAngle(transform.localEulerAngles.z), 0, Time.deltaTime * 10);
                break;
		}

        // Get vertical mouse input and clamp it
        verticalRotation += mouseY * sensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        // Set the rotations
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(verticalRotation, wallrunHorizontalRotation, tilt), Time.deltaTime * sensitivity);
        player.transform.rotation = Quaternion.Slerp(player.transform.localRotation, Quaternion.Euler(0, horizontalRotation, 0), Time.deltaTime * sensitivity);

        float swayX = -mouseX;
        float swayY = mouseY;

        swayX = Mathf.Clamp(swayX, -maxSway, maxSway);
        swayY = Mathf.Clamp(swayY, -maxSway, maxSway);

        Vector3 viewModelSwayPosition = new Vector3(swayX, swayY, 0) * swayAmount / 100;
        viewModel.transform.localPosition = Vector3.Lerp(viewModel.transform.localPosition, viewModelInitialPosition + viewModelSwayPosition, Time.deltaTime * swaySpeed);

    }

    /// <summary>
    /// Gets a signed angle from -180 to 180
    /// </summary>
    /// <param name="angle">Angle</param>
    /// <returns>Angle from -180 to 180</returns>
    private float GetSignedAngle(float angle) {
        return (angle > 180) ? angle - 360 : angle;
	}

    public void Wallrun(int direction, Vector3 vectorAlongWall) {
        wallrunning = direction;
        this.vectorAlongWall = vectorAlongWall;
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
