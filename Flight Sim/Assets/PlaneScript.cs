using UnityEngine;
using System.Collections;

public class PlaneScript : MonoBehaviour {

	public float s0 = 50;
	public float speed = 50; //m/s
	public float acceleration = 15; //m/s/s
	public Transform spawn;
	public ParticleSystem explosion;

	public int playerNum = 1;

	float respawnTime = 5; //seconds

	float timeToAlive = 0;
	Animator anim;

	bool alive = true;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}



	void OnTriggerEnter(Collider c) {
		if (alive) {
			alive = false;
			timeToAlive = respawnTime;
			anim.SetBool ("Alive", alive);
			explosion.Stop ();
			explosion.transform.position = transform.position;
			explosion.Play ();
		}
	}

	// Update is called once per frame
	void Update () {
		if (alive) {
			Vector3 dR = new Vector3(Input.GetAxis("Vertical"+playerNum), Input.GetAxis ("Horizontal"+playerNum), 0); //change in rotation from user input
			//change in thrust from user input??

			//change in speed from angle
			speed += transform.up.y * acceleration * Time.deltaTime; //the plane's "up" is along its length
			speed = Mathf.Clamp (speed, 5f, 100f);
			//rotate the plane
			transform.Rotate (dR);
			transform.position = transform.position - transform.up*speed*Time.deltaTime; //adjust position based on "negative up"
		} else {
			timeToAlive -= Time.deltaTime;
			explosion.transform.position = transform.position; //follow me explosion!
			if (transform.position.y > Terrain.activeTerrain.SampleHeight (transform.position) + 0.5f) {
				transform.position = new Vector3(transform.position.x, transform.position.y + Physics.gravity.y*Time.deltaTime, transform.position.z);

			}
			if (timeToAlive <= 0) {
				transform.position = spawn.position;
				transform.rotation = spawn.rotation;
				alive = true;
				speed = s0;
				anim.SetBool ("Alive", alive);
			}
		}
	}
}
