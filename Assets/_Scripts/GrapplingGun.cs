using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour {

	Vector3 grapplePoint;
	Vector3 currentGrapplePosition;

	SpringJoint joint;
	LineRenderer lr;

	[SerializeField] float maxGrappleDist;
	[SerializeField] LayerMask grappleable;
	[SerializeField] Transform gunTip, cam, player;

	private void Awake() {
		lr = GetComponent<LineRenderer>();
	}

	void BeginGrapple() {
		RaycastHit hit;
		if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDist, grappleable)) {
			grapplePoint = hit.point;
			joint = player.gameObject.AddComponent<SpringJoint>();
			joint.autoConfigureConnectedAnchor = false;
			joint.connectedAnchor = grapplePoint;

			float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

			// Sets dist grapple will try to keep from the grapple point
			joint.maxDistance = distanceFromPoint * 0.6f;
			joint.minDistance = distanceFromPoint * 0.25f;

			// Some values to tweak
			joint.spring = 4.5f;
			joint.damper = 7f;
			joint.massScale = 4.5f;

			lr.positionCount = 2;
			currentGrapplePosition = gunTip.position;
		}
	}

	void EndGrapple() {
		lr.positionCount = 0;
		Destroy(joint);
	}

	void DrawRope() {
		if (!joint) return;

		currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 6f);

		lr.SetPosition(0, gunTip.position);
		lr.SetPosition(1, grapplePoint);
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			BeginGrapple();
		}
		if (Input.GetMouseButtonUp(0)) {
			EndGrapple();
		}
	}

	void LateUpdate() {
		DrawRope();
	}

	public bool IsGrappling() {
		return joint != null;
	}

	public Vector3 GetGrapplePoint() {
		return grapplePoint;
	}
}
