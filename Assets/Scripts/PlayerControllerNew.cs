using UnityEngine;
using System.Collections;
using FMOD.Studio;

public class PlayerControllerNew : MonoBehaviour {

	public float jumpForce;
	public float maxSpeedX;
	public float walkForce;

	public float fallenY;

	public Camera cam;
	public int size;

	bool hurt;
	int hurtTimer;
	public int maxHurtTime;
	int health;
	public int maxHealth;

	bool fallen;

	public float initX, initY;
	Vector3 initialPos;

	float checkDistance = 0.1f;


	enum PlayerState{
		STANDING, 
		JUMPING, 
		FALLING, 
		WALKING
	};
	enum Direction{
		LEFT,
		RIGHT
	};
	
	PlayerState state;
	Direction dir;

	Animator feetAnim, woolAnim, faceAnim;

	Rigidbody2D rb2d;

	bool jumpHeld, leftHeld, rightHeld;

	// Use this for initialization
	void Start () {
		hurt = false;
		health = maxHealth;
		fallen = false;
		initialPos = new Vector3 (initX, initY, 0f);
		state = PlayerState.STANDING;
		dir = Direction.RIGHT;
		rb2d = GetComponent<Rigidbody2D>();

		transform.position = initialPos;
	
		feetAnim = transform.FindChild("Feet").GetComponent<Animator>();
		woolAnim = transform.FindChild("Wool").GetComponent<Animator>();
		faceAnim = transform.FindChild("Face").GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		checkInput ();

		if (!fallen)
			checkFallen ();

		if (hurt) {
			if (hurtTimer < maxHurtTime) {
				++hurtTimer;
			} else {
				hurtTimer = 0;
				hurt = false;
			}
		}

		switch (state) {
		case PlayerState.STANDING:
			if (jumpHeld && checkGround()) {
				enterJump();
			}
			if ((leftHeld && !checkLeft()) || (rightHeld && !checkRight()) ) {
				enterWalk();
			}
			break;
		case PlayerState.FALLING:
			if (checkGround () ) {
				playSound("event:/SFX/Mepo/sfx_mepo_land");
				if (jumpHeld) {
					enterJump();
				} else if (leftHeld || rightHeld) {
					enterWalk();
				} else if (rb2d.velocity.x == 0) {
					enterStand();
				} else if (rb2d.velocity.x != 0) {
					enterWalk ();
				}
			
			}
			break;
		case PlayerState.WALKING:
			if (jumpHeld && checkGround()) {
				enterJump();
			}
			if (rb2d.velocity.x == 0) {
				enterStand ();
			}
			if (!checkGround() ) {
				enterFall ();
			}
			break;
		case PlayerState.JUMPING:
			if (rb2d.velocity.y < 0) {
				enterFall();
			}
			if (rb2d.velocity.y == 0 && checkGround ()) {
				if (rb2d.velocity.x != 0) {
					enterWalk ();
				} else {
					enterStand ();
				}
			}
			break;
		}

		if (state == PlayerState.WALKING || state == PlayerState.JUMPING || state == PlayerState.FALLING) {
			if (leftHeld) {
				if (dir == Direction.RIGHT)
					Flip ();
				moveLeft();
			} else if (rightHeld) {
				if (dir == Direction.LEFT)
					Flip ();
				moveRight();
			}
		}
	}

	void checkInput() {
		if (Input.GetKey (KeyCode.Space) || Input.GetButton("Jump") ){
			jumpHeld = true;
		} else {
			jumpHeld = false;
		}
		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetAxis ("Horizontal") < 0) {
			leftHeld = true;
			rightHeld = false;
		} else {
			leftHeld = false;
		}
		if (Input.GetKey (KeyCode.RightArrow) || Input.GetAxis ("Horizontal") > 0 ) {
			rightHeld = true;
			leftHeld = false;
		} else {
			rightHeld = false;
		}
		if (Input.GetButton("Quit")) {
			Application.Quit ();
		}
	}


	bool checkGround() {
		Collider2D col = GetComponent<Collider2D> ();
		Vector2 centre = new Vector2 (col.transform.position.x, col.transform.position.y);
		Vector2 left = new Vector2 (col.transform.position.x - (col.bounds.extents.x *0.90f), col.transform.position.y);
		Vector2 right = new Vector2 (col.transform.position.x + (col.bounds.extents.x * 0.90f), col.transform.position.y);

		Vector2 feetC = centre - new Vector2 (0, col.bounds.extents.y + checkDistance);
		Vector2 feetL = left - new Vector2 (0, col.bounds.extents.y + checkDistance);
		Vector2 feetR = right - new Vector2 (0, col.bounds.extents.y + checkDistance);
		bool rayC, rayL, rayR;
		rayC = Physics2D.Linecast (centre, feetC, 1 << LayerMask.NameToLayer ("Platforms"));
		rayL = Physics2D.Linecast (left, feetL, 1 << LayerMask.NameToLayer ("Platforms"));
		rayR = Physics2D.Linecast (right, feetR, 1 << LayerMask.NameToLayer ("Platforms"));
		if (rayC || rayL || rayR) {
			Debug.Log ("Collided with ground");
			return true;
		}
		return false;
	}

	bool checkLeft() {
		Collider2D col = GetComponent<Collider2D> ();
		Vector2 centre = new Vector2 (col.transform.position.x, col.transform.position.y);
		Vector2 topLeft = new Vector2 (col.transform.position.x , col.transform.position.y + (col.bounds.extents.y * 0.9f));
		Vector2 bottomLeft = new Vector2 (col.transform.position.x, col.transform.position.y - (col.bounds.extents.y * 0.9f));
		
		Vector2 edgeC = centre - new Vector2 (col.bounds.extents.x + checkDistance, 0);
		Vector2 edgeT = topLeft - new Vector2 (col.bounds.extents.x + checkDistance, 0);
		Vector2 edgeB = bottomLeft - new Vector2 (col.bounds.extents.x + checkDistance, 0);
		bool rayC, rayL, rayR;
		rayC = Physics2D.Linecast (centre, edgeC, 1 << LayerMask.NameToLayer ("Platforms"));
		rayL = Physics2D.Linecast (topLeft, edgeT, 1 << LayerMask.NameToLayer ("Platforms"));
		rayR = Physics2D.Linecast (bottomLeft, edgeB, 1 << LayerMask.NameToLayer ("Platforms"));
		if (rayC || rayL || rayR) {
			Debug.Log ("Collided left");
			return true;
		}
		return false;
	}

	bool checkRight() {
		Collider2D col = GetComponent<Collider2D> ();
		Vector2 centre = new Vector2 (col.transform.position.x, col.transform.position.y);
		Vector2 top = new Vector2 (col.transform.position.x, col.transform.position.y + (col.bounds.extents.y * 0.9f));
		Vector2 bottom = new Vector2 (col.transform.position.x, col.transform.position.y - (col.bounds.extents.y * 0.9f));
		
		Vector2 edgeC = centre + new Vector2 (col.bounds.extents.x + checkDistance, 0);
		Vector2 edgeT = top + new Vector2 (col.bounds.extents.x + checkDistance, 0);
		Vector2 edgeB = bottom + new Vector2 (col.bounds.extents.x + checkDistance, 0);
		bool rayC, rayL, rayR;
		rayC = Physics2D.Linecast (centre, edgeC, 1 << LayerMask.NameToLayer ("Platforms"));
		rayL = Physics2D.Linecast (top, edgeT, 1 << LayerMask.NameToLayer ("Platforms"));
		rayR = Physics2D.Linecast (bottom, edgeB, 1 << LayerMask.NameToLayer ("Platforms"));
		if (rayC || rayL || rayR) {
			Debug.Log ("Collided left");
			return true;
		}
		return false;
	}

	void enterJump() {
		rb2d.AddForce (Vector2.up * jumpForce);

		playSound("event:/SFX/Mepo/sfx_mepo_jump");

		Debug.Log ("Entered jumping state.");
		feetAnim.CrossFade ("Feet_Jump", 0);
		woolAnim.CrossFade ("Wool_Jump", 0);
		faceAnim.CrossFade ("Face_Jump", 0);
		state = PlayerState.JUMPING;     
	}

	void enterStand() {
		state = PlayerState.STANDING;
		feetAnim.CrossFade ("Feet_Stand", 0);
		woolAnim.CrossFade ("Wool_Stand", 0);
		faceAnim.CrossFade ("Face_Stand", 0);
		Debug.Log ("Entered standing state.");
	}

	void enterFall() {
		state = PlayerState.FALLING;
		feetAnim.CrossFade ("Feet_Fall", 0);
		woolAnim.CrossFade ("Wool_Fall", 0);
		faceAnim.CrossFade ("Face_Fall", 0);
		Debug.Log ("Entered falling state.");
	}

	void enterWalk() {
		Debug.Log ("Entered walking state.");
		feetAnim.CrossFade ("Feet_Walk", 0);
		woolAnim.CrossFade ("Wool_Walk", 0);
		faceAnim.CrossFade ("Face_Walk", 0);
		state = PlayerState.WALKING;
	}

	void moveLeft() {
		dir = Direction.LEFT;
		if (rb2d.velocity.x > -maxSpeedX) {
			rb2d.AddForce (Vector2.left * walkForce);
		}
	}

	void moveRight() {
		dir = Direction.RIGHT;
		if (rb2d.velocity.x < maxSpeedX) {
			rb2d.AddForce (Vector2.right * walkForce);
		}
	}


	void OnCollisionEnter2D(Collision2D col) {
		switch (col.gameObject.tag) {
		case "Enemy":
			Debug.Log ("Collided with enemy.");
			if (col.gameObject.GetComponent<BugController>().size <= size && (state == PlayerState.JUMPING || state == PlayerState.FALLING)) {
				doFood ();
				Destroy(col.gameObject);
			} else {
				if (!hurt) {
					rb2d.AddForce ((-col.relativeVelocity.normalized * 1000) + (Vector2.up * (jumpForce / 3)));
					doHurt ();
				}
			}
			break;
		case "Food":
			Debug.Log ("Collided with food.");
			Destroy (col.gameObject);
			doFood();
			break;
		case "Spikes":
			if (!hurt) {
				rb2d.AddForce ((-col.relativeVelocity.normalized * 1000) + (Vector2.up * (jumpForce / 3)));
				doHurt ();
			}
			break;
		}
	}

	void Flip()
	{
		// Multiply the player's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void checkFallen() {
		if (transform.position.y <= fallenY) {
			playSound("event:/SFX/Mepo/sfx_mepo_death");
			rb2d.isKinematic = true;
			fallen = true;
			Invoke("reset", 1);

		}
	}

	void doHurt() {
		--health;
		hurt = true;
		playSound ("event:/SFX/Mepo/sfx_mepo_damage");
		Debug.Log (health);
		if (health <= 0) {
			rb2d.isKinematic = true;
			playSound ("event:/SFX/Mepo/sfx_mepo_death");
			Invoke ("reset", 0.25f);
		}
	}

	void doFood() {
		playSound("event:/SFX/Mepo/sfx_mepo_bite");
		if (health < maxHealth)
			++health;
	}

	void reset() {
		transform.position = initialPos;
		hurt = false;
		rb2d.isKinematic = false;

		health = maxHealth;
		fallen = false;

	}

	void playSound(string path) {
		FMOD_StudioSystem.instance.PlayOneShot (
			path, cam.transform.position);
	}


	public int getHealth() {
		return health;
	}
}
