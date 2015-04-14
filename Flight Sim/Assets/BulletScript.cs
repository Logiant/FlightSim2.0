using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	public PlaneScript parent;
	Vector3 velocity;

	public void Initialize(PlaneScript parent, Vector3 velocity) {
		this.parent = parent;
		this.velocity = velocity;
	}

	// Update is called once per frame
	void Update () {
		velocity.y += Physics.gravity.y * Time.deltaTime;
		transform.position = transform.position + velocity*Time.deltaTime;

		if (transform.position.y <= 0) {
			Destroy (this.gameObject);
		}
	}
}
