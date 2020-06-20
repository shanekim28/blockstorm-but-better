using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour {
    [Tooltip("How quickly the laser dissolves (higher is faster)")]
    [Range(0.5f, 15f)]
    public float dissolveSpeed = 10f;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Awake() {
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    // Update is called once per frame
    void Update() {
        // Destroy the laser after the width becomes 0
        if (lineRenderer.startWidth <= 0) {
            Destroy(this.gameObject);
		}

        // Decrease the laser width
        lineRenderer.startWidth -= dissolveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Sets the positions of the laser
    /// </summary>
    /// <param name="start">Beginning point of the laser</param>
    /// <param name="end">End point of the laser</param>
    public void SetPositions(Vector3 start, Vector3 end) {
        lineRenderer.SetPositions(new Vector3[] { start, end });
	}
}
