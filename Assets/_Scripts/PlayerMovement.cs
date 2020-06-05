using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public Rigidbody rb;

	[SerializeField] float movementSpeed;
	[SerializeField] float jumpSpeed;
	[SerializeField] float wallRunDist;

	float x;
	float z;
	bool jumpButtonPressed;

	float wallrunCooldown = 0f;

	bool grounded;
	Vector3 globalWalkVector;
	
	public enum PlayerState {
		Idle, Walking, Falling, Airwalking, Jumping,
		WallrunLeft, WallrunRight, WalljumpFromLeft, WalljumpFromRight,
		Grappling };

	// TODO - split walking into walking and strafing (same with airwalking)

	// Idle - grounded and doing nothing
	// Walking - grounded and walking

	// Falling - airborne and doing nothing
	// Airwalking - airborne and walking
	// Jumping - jumping

	// WallrunLeft - wallrunning on a wall to the left
	// WallrunRight - wallrunning on a wall to the right
	// WalljumpFromLeft - jumping off a wall to the left
	// WalljumpFromRight - jumping off a wall to the right

	// Grappling - tethered to a grapple point

	public PlayerState currentState = PlayerState.Idle;
	private PlayerState lastState;

	private bool isGrounded() {
		return (currentState == PlayerState.Idle || currentState == PlayerState.Walking);
	}

	private void UpdateMovementInput() {
		// Store input axes
		x = Input.GetAxis("Horizontal");
		z = Input.GetAxis("Vertical");

		// Store walk input as a world-space vector for setting rigidbody velocity
		globalWalkVector = ((transform.forward * z) + (transform.right * x)) * movementSpeed;

		// Check whether we're trying to jump
		jumpButtonPressed = (Input.GetButtonDown("Jump"));
	}
	
	private void AssignPlayerStates () {
		// Store the last player state
		lastState = currentState;

		// Set base states to prevent state carry-over
		if (grounded) {
			currentState = PlayerState.Idle;
		} else {
			currentState = PlayerState.Falling;
		}

		// If we're attempting to apply horizontal input...
		if (globalWalkVector != Vector3.zero) {
			if (grounded) {
				// If we're grounded, we're walking
				currentState = PlayerState.Walking;
			} else {
				// Otherwise, we're airwalking
				currentState = PlayerState.Airwalking;
			}
		}

		// Check this before jump, so a jump will override it.
		if (!grounded && wallrunCooldown == 0) {
			CheckWallRun();
		}

		// If we try to jump
		if (jumpButtonPressed) {
			// If we're grounded, jump
			if (grounded) {
				currentState = PlayerState.Jumping;
			}
			// If we're wallrunning, jump off the wall
			if (currentState == PlayerState.WallrunLeft) {
				currentState = PlayerState.WalljumpFromLeft;
			}
			if (currentState == PlayerState.WallrunRight) {
				currentState = PlayerState.WalljumpFromRight;
			}
		}

		// If there was a state change...
		if (lastState != currentState) {
			if (lastState == PlayerState.WallrunLeft || lastState == PlayerState.WallrunRight) {
				wallrunCooldown = 0.5f;
			}
		}
	}

	private void ApplyMovementInput() {
		// Walk
		if (currentState == PlayerState.Walking) {
			// If we're grounded and trying to walk,
			// walk
			rb.velocity = new Vector3(globalWalkVector.x, rb.velocity.y, globalWalkVector.z);
		} else if (currentState == PlayerState.Airwalking) {
			// If we're falling and trying to walk,
			// airwalk
			rb.AddForce(new Vector3(globalWalkVector.x, 0, globalWalkVector.z)/4, ForceMode.Acceleration);
		}

		// Jump
		else if (currentState == PlayerState.Jumping) {
			rb.AddForce(transform.up * jumpSpeed, ForceMode.VelocityChange);
			currentState = PlayerState.Falling;
		} else if (currentState == PlayerState.WalljumpFromLeft) {
			rb.AddForce((transform.up + transform.right).normalized * jumpSpeed, ForceMode.VelocityChange);
			currentState = PlayerState.Falling;
		} else if (currentState == PlayerState.WalljumpFromRight) {
			rb.AddForce((transform.up - transform.right).normalized * jumpSpeed, ForceMode.VelocityChange);
			currentState = PlayerState.Falling;
		}
	}

	private void CheckWallRun() {
		// Raycast to see if we should be wallrunning
		// TODO - fix this terrible structure

		// Check right raycast
		// Check left raycast

		// if exactly one ray hit...
		// ...if it was left, do the left stuff
		// ... else do the right stuff
		// else if both rays hit...
		// ...if left is closer, do the left stuff
		// ...if right is closer, do the right stuff
		// else...
		// ...we aren't wallrunning

		RaycastHit leftHit;
		RaycastHit rightHit;

		bool wallToLeft = false;
		bool wallToRight = false;
	

		if (Physics.Raycast(transform.position, -transform.right, out leftHit, wallRunDist) && Vector3.Dot(leftHit.normal, Vector3.up) == 0) {
			wallToLeft = true;
		}

		if (Physics.Raycast(transform.position, transform.right, out rightHit, wallRunDist) && Vector3.Dot(rightHit.normal, Vector3.up) == 0) {
			wallToRight = true;
		}

		// Okay, this is a little better
		// (Optimized for readability, not necessarily efficiency)
		if (wallToLeft ^ wallToRight) {
			// If we have exactly one wallrunning option,
			// find out which one it is and wallrun on that side.
			if (wallToLeft) {
				currentState = PlayerState.WallrunLeft;
				WallRun(-Vector3.Cross(Vector3.up, leftHit.normal));
			} else {
				currentState = PlayerState.WallrunRight;
				WallRun(Vector3.Cross(Vector3.up, rightHit.normal));
			}
		} else if (wallToLeft && wallToRight) {
			// If we have two wallrunning options,
			// wallrun on the side that's closer
			if (leftHit.distance < rightHit.distance) {
				currentState = PlayerState.WallrunLeft;
				WallRun(-Vector3.Cross(Vector3.up, leftHit.normal));
			} else {
				currentState = PlayerState.WallrunRight;
				WallRun(Vector3.Cross(Vector3.up, rightHit.normal));
			}
		} /*else if (currentState == PlayerState.WallrunLeft || currentState == PlayerState.WallrunRight) {
			// If we are wallrunning but have no wallrunning options,
			// Stop wallrunning
			currentState = PlayerState.Falling;
		}*/
	}

	private void WallRun(Vector3 vectorAlongWall) {
		Debug.DrawLine(transform.position, transform.position + vectorAlongWall);
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vectorAlongWall, transform.up), Time.deltaTime * 10f);
		rb.velocity = vectorAlongWall * movementSpeed;
	}

	void Update() {
		if (wallrunCooldown > 0) {
			wallrunCooldown -= Time.deltaTime;
		}

		if (wallrunCooldown < 0) {
			wallrunCooldown = 0;
		}

		UpdateMovementInput();
		AssignPlayerStates();
		ApplyMovementInput();
	}

	private void OnGUI() {
		GUI.TextField(new Rect(new Vector2(10, 10), new Vector2(110, 20)), currentState.ToString() + " " + wallrunCooldown);
	}

	private void OnTriggerStay(Collider other) {
		grounded = true;
	}

	private void OnTriggerExit(Collider other) {
		grounded = false;
	}
}
