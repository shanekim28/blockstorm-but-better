using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGun : MonoBehaviour {
	[SerializeField] GrapplingGun grapple;

	Quaternion desiredRotation;
	float rotationSpeed = 5f;

	void Update() {
		if (!grapple.IsGrappling()) {
			desiredRotation = transform.parent.rotation;
		} else {
			desiredRotation = Quaternion.LookRotation(grapple.GetGrapplePoint() - transform.position);
		}

		transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
	}
}
