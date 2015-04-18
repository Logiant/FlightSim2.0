using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlaneScript : MonoBehaviour {

	public float s0 = 50;
	public float speed = 50; //m/s
	public float acceleration = 15; //m/s/s
	public Transform spawn;
	public ParticleSystem explosion;
	public float maxHealth = 10;
	public Text text;

	float points = 0;

	float maxSpeed = 225; //m/s
	float minSpeed = 25; //m/s

	float pitchSpeed = 90; //deg/s
	float rollSpeed = 180; //deg/s
	float yawSpeed = 30; //deg/s

	float throttle = 1;
	float throttleSensitivity = 0.5f; //stuffs

	float health;

	float muzzleSpeed = 250; //m/s

	public int playerNum = 1;

	public GameObject bullet;

	Transform Gun1;
	Transform Gun2;
	float respawnTime = 5; //seconds
	float timeToAlive = 0;
	Animator anim;

	int bulletsFired;
	float bulletCD = 0.1f/2f; //(bullets per second)/2
	float bulletTime = 0;
	float reloadTime = 1; //secinds
	float currentReload = 0;
	bool reloading = false;


	int clipSize = 250;


	bool alive = true;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		Gun1 = transform.Find ("Gun1");
		Gun2 = transform.Find ("Gun2");
		health = maxHealth;

	}


	public void addPoint() {
		points++;
	}

	void OnTriggerEnter(Collider c) {
		if (c.CompareTag ("Boundary")) {
			transform.Rotate (transform.up, 180);
		} else if (c.CompareTag("Bullet")) {
			BulletScript bs = c.GetComponent<BulletScript>();
			if (bs.parent != this) {
				health -= 1;
				if (health <= 0 && alive) {
					die ();
					bs.parent.addPoint ();
				}
			}
		} else if(alive) {
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
		throttle = 1;
		bulletsFired = 0;
		reloading = false;
		points --;
	}

	// Update is called once per frame
	void Update () {
		if (alive) {
			float speedFraction = Mathf.Max (speed/maxSpeed, 0.7f);
			Vector3 dR = new Vector3(Input.GetAxis("Vertical"+playerNum) * pitchSpeed * Time.deltaTime * speedFraction,
			                         Input.GetAxis ("Horizontal"+playerNum) * rollSpeed * Time.deltaTime * speedFraction,
			                         Input.GetAxis ("Yaw"+playerNum) * yawSpeed * Time.deltaTime * speedFraction); //change in rotation from user input
			//change in thrust from user input??
			throttle += Input.GetAxis ("Throttle"+playerNum) * Time.deltaTime * throttleSensitivity;
			throttle = Mathf.Clamp(throttle, 0, 1.2f);
			//change in speed from angle
			speed += transform.up.y * acceleration * Time.deltaTime + (throttle-1) * Time.deltaTime * acceleration; //the plane's "up" is along its length
			speed = Mathf.Clamp (speed, minSpeed, maxSpeed);
			//rotate the plane
			transform.Rotate (dR);
			transform.position = transform.position - transform.up*speed*Time.deltaTime; //adjust position based on "negative up"

			bulletTime -= Time.deltaTime;
			if (reloading) {
				if ((currentReload -= Time.deltaTime) <= 0) {
					reloading = false;
					bulletsFired = 0;
				}
			}


			if (Input.GetAxis ("Fire"+playerNum) > 0.5) { //add a bit of a wobble to the firing to be more connical... +- 5 degrees?
				if (bulletsFired >= clipSize) {
					if (!reloading) {
						reloading = true;
						currentReload = reloadTime;
					}
				}
				else if (bulletTime <= 0) {
					bulletTime = bulletCD;
					bulletsFired ++;
					float speedVariance = Random.Range (-0.2f, 0.2f) + 1;
					float t1Var = Random.Range (-15.0f, 15.0f);
					float t2Var = Random.Range (-15.0f, 15.0f);
					Quaternion orientation = Quaternion.Euler (Gun1.rotation.eulerAngles.x + t1Var, Gun1.rotation.eulerAngles.y + t2Var, Gun1.rotation.eulerAngles.z);
					if (bulletsFired % 2 == 0) {
						GameObject b = (GameObject)Instantiate (bullet, Gun1.position, orientation);
						BulletScript bs = b.GetComponent<BulletScript>();
						bs.Initialize(this, -transform.up*(speed + muzzleSpeed)*speedVariance);
					} else {
						GameObject b = (GameObject)Instantiate (bullet, Gun2.position, orientation);
						BulletScript bs = b.GetComponent<BulletScript>();
						bs.Initialize(this, -transform.up*(speed + muzzleSpeed)*speedVariance);
					}
				}
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
		updateText ();
	}

	void updateText() {
		text.text = "Throttle: " + (int)(throttle*100) + "%\nAltitude: " + (int)transform.position.y + "m\nSpeed: " + (int)speed + "m/s\nClip: " + (clipSize - bulletsFired) + "\nScore: " + points;


	}
}
