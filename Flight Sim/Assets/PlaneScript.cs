using UnityEngine;
using System.Collections;

public class PlaneScript : MonoBehaviour {

	public float s0 = 50;
	public float speed = 50; //m/s
	public float acceleration = 15; //m/s/s
	public Transform spawn;
	public ParticleSystem explosion;
	public float maxHealth = 10;

	float maxSpeed = 225; //m/s
	float minSpeed = 25; //m/s

	float pitchSpeed = 90; //deg/s
	float rollSpeed = 140; //deg/s

	float throttle = 1;

	float health;

	float muzzleSpeed = 250; //m/s

	public int playerNum = 1;

	public GameObject bullet;

	Transform Gun1;
	Transform Gun2;
	float respawnTime = 5; //seconds
	float timeToAlive = 0;
	Animator anim;

	bool alive = true;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		Gun1 = transform.Find ("Gun1");
		Gun2 = transform.Find ("Gun2");
		health = maxHealth;
	}



	void OnTriggerEnter(Collider c) {
		if (c.CompareTag ("Boundary")) {
			transform.Rotate (transform.up, 180);
		} else if (c.CompareTag("Bullet")) {
			health -= 1;
			if (health <= 0) {
				die ();
			}
		} else {
			die ();
		}
	}


	void die() {
		alive = false;
		timeToAlive = respawnTime;
		anim.SetBool ("Alive", alive);
		explosion.Stop ();
		explosion.transform.position = transform.position;
		explosion.Play ();
	}

	// Update is called once per frame
	void Update () {
		if (alive) {
			float speedFraction = Mathf.Max (speed/maxSpeed, 0.7f);
			Vector3 dR = new Vector3(Input.GetAxis("Vertical"+playerNum) * pitchSpeed * Time.deltaTime * speedFraction,
			                         Input.GetAxis ("Horizontal"+playerNum) * rollSpeed * Time.deltaTime * speedFraction, 0); //change in rotation from user input
			//change in thrust from user input??
			throttle += Input.GetAxis ("Throttle"+playerNum) * Time.deltaTime;
			throttle = Mathf.Clamp(throttle, 0, 1.2f);
			Debug.Log (throttle + ", " + playerNum);
			//change in speed from angle
			speed += transform.up.y * acceleration * Time.deltaTime + (throttle-1) * Time.deltaTime * acceleration; //the plane's "up" is along its length
			speed = Mathf.Clamp (speed, minSpeed, maxSpeed);
			//rotate the plane
			transform.Rotate (dR);
			transform.position = transform.position - transform.up*speed*Time.deltaTime; //adjust position based on "negative up"

			if (Input.GetButton ("Fire"+playerNum)) { //add a bit of a wobble to the firing to be more connical... +- 5 degrees?
				GameObject b = (GameObject)Instantiate (bullet, Gun1.position, Gun1.rotation);
				BulletScript bs = b.GetComponent<BulletScript>();
				bs.Initialize(this, -transform.up*(speed + muzzleSpeed));
				//fire both guns!
				b = (GameObject)Instantiate (bullet, Gun2.position, Gun1.rotation);
				bs = b.GetComponent<BulletScript>();
				bs.Initialize(this, -transform.up*(speed + muzzleSpeed));
			}

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
