using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public Transform cam;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 desiredPos = transform.position + transform.up * 10f + transform.forward * 5f; //get camera desired position from model coordinates
		float bias = 0.96f; //rubberband factor for camera
		cam.position = cam.position * bias + desiredPos * (1 - bias); //move 4% of the way to target position
		cam.LookAt (transform.position - transform.up * 2); //look just in front of the plane
	}
}
