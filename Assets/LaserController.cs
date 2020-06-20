using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour {
    [Range(0, 15f)]
    public float dissolveSpeed = 10f;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Awake() {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        Destroy(this, 0.7f);
    }

    // Update is called once per frame
    void Update() {
        if (lineRenderer.startWidth > 0) {
            lineRenderer.startWidth -= Time.deltaTime * dissolveSpeed;
		}
    }

    public void SetPositions(Vector3 start, Vector3 end) {
        lineRenderer.SetPositions(new Vector3[] { start, end });
	}
}
