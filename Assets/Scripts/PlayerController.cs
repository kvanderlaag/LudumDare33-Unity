using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float gravity = 0.01f;
	public float maxJumpSpeed = 0.5f;
	public float walkSpeed = 0.1f;
	public float jumpSpeedMod = 1.8f;
	public float maxFallSpeed;

	bool jumpHeld;
	int jumpTimer;
	int fallTimer;
	public int maxJump;

	public float checkDistance;


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

	// Use this for initialization
	void Start () {
		state = PlayerState.STANDING;
		jumpTimer = 0;
		jumpHeld = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		switch (state) {
		case PlayerState.STANDING:
			doStand();
			break;
		case PlayerState.JUMPING:
			doJump();
			break;
		case PlayerState.WALKING:
			doWalk();
			break;
		case PlayerState.FALLING:
			doFall();
			break;
		}
		checkInput ();

	}


	// Do jumping stuff
	void doJump() {
		if (jumpTimer < maxJump && jumpHeld && checkAbove() == 0) {
			transform.Translate (Vector3.up * (maxJumpSpeed - (maxJumpSpeed / maxJump * jumpTimer)));
			++jumpTimer;
		} else {
			state = PlayerState.FALLING;
			enterFall ();
		}
	}

	// Do standing stuff
	void doStand() {
		if (checkGround () == 0) {
			state = PlayerState.FALLING;
			enterFall ();
		} 
		Debug.Log ("In standing state.");
	}

	// Do falling stuff
	void doFall() {
		float fallSpeed = fallTimer * gravity;
		if (checkGround () != 0) {
			state = PlayerState.STANDING;
			enterStand ();
		} else {
			transform.Translate(Vector3.down * (fallSpeed));
			if (fallTimer < maxFallSpeed)
				++fallTimer;
		}
		Debug.Log ("In falling state.");
	}

	// Do walking stuff
	void doWalk() {


	}

	float checkGround() {
		float fallSpeed = fallTimer * gravity;
		if (fallSpeed == 0)
			fallSpeed = checkDistance;
		Collider2D col = GetComponent<Collider2D>();
		Vector2 centre = new Vector2 (col.bounds.center.x, col.bounds.center.y - col.bounds.extents.y);
		Vector2 left = new Vector2 (col.bounds.center.x - col.bounds.extents.x * 0.95f, col.bounds.center.y - col.bounds.extents.y);
		Vector2 right = new Vector2 (col.bounds.center.x + col.bounds.extents.x * 0.95f, col.bounds.center.y - col.bounds.extents.y);
		Vector2 feetC = centre - new Vector2(0.0f, fallSpeed);
		Vector2 feetL = left - new Vector2 (0.0f, fallSpeed * 0.95f);
		Vector2 feetR = right - new Vector2 (0.0f, fallSpeed * 0.95f);
		RaycastHit2D hitC = Physics2D.Linecast (centre, feetC, 1 << LayerMask.NameToLayer ("Platforms"));
		RaycastHit2D hitL = Physics2D.Linecast (left, feetL, 1 << LayerMask.NameToLayer ("Platforms"));
		RaycastHit2D hitR = Physics2D.Linecast (right, feetR, 1 << LayerMask.NameToLayer ("Platforms"));
		if (hitC) {
			Debug.Log ("Collided with ground.");
			Debug.Log (col.bounds.min.y - hitC.collider.bounds.max.y);
			return col.bounds.min.y - hitC.collider.bounds.max.y;
		} else if (hitL) {
			Debug.Log ("Collided with ground.");
			return col.bounds.min.y - hitL.collider.bounds.max.y;
		} else if (hitR) {
			Debug.Log ("Collided with ground.");
			return col.bounds.min.y - hitR.collider.bounds.max.y;
		}
		return 0;
	}

	float checkAbove() {
		float jumpSpeed = (maxJumpSpeed - (maxJumpSpeed / maxJump * jumpTimer));
		if (jumpSpeed == 0)
			jumpSpeed = checkDistance;
		Collider2D col = GetComponent<Collider2D>();
		Vector2 centre = new Vector2 (col.bounds.center.x, col.bounds.center.y + col.bounds.extents.y);
		Vector2 left = new Vector2 (col.bounds.center.x - col.bounds.extents.x, col.bounds.center.y + col.bounds.extents.y);
		Vector2 right = new Vector2 (col.bounds.center.x + col.bounds.extents.x, col.bounds.center.y + col.bounds.extents.y);
		Vector2 feetC = centre + new Vector2(0.0f, jumpSpeed);
		Vector2 feetL = left + new Vector2 (0.0f, jumpSpeed);
		Vector2 feetR = right + new Vector2 (0.0f, jumpSpeed);
		RaycastHit2D hitC = Physics2D.Linecast (centre, feetC, 1 << LayerMask.NameToLayer ("Platforms"));
		RaycastHit2D hitL = Physics2D.Linecast (left, feetL, 1 << LayerMask.NameToLayer ("Platforms"));
		RaycastHit2D hitR = Physics2D.Linecast (right, feetR, 1 << LayerMask.NameToLayer ("Platforms"));
		if (hitC) {
			Debug.Log ("Collided with ground.");
			Debug.Log (col.bounds.min.y - hitC.collider.bounds.max.y);
			return col.bounds.min.y - hitC.collider.bounds.max.y;
		} else if (hitL) {
			Debug.Log ("Collided with ground.");
			return col.bounds.min.y - hitL.collider.bounds.max.y;
		} else if (hitR) {
			Debug.Log ("Collided with ground.");
			return col.bounds.min.y - hitR.collider.bounds.max.y;
		}
		return 0;
	}

	bool checkAboveOld() {
		Collider2D col = GetComponent<Collider2D>();
		Vector2 centre = new Vector2 (col.bounds.center.x, col.bounds.center.y + col.bounds.extents.y);
		Vector2 left = new Vector2 (col.bounds.center.x - col.bounds.extents.x, col.bounds.center.y + col.bounds.extents.y);
		Vector2 right = new Vector2 (col.bounds.center.x + col.bounds.extents.x, col.bounds.center.y + col.bounds.extents.y);
		Vector2 headC = centre + new Vector2(0.0f, checkDistance);
		Vector2 headL = left + new Vector2 (0.0f, checkDistance);
		Vector2 headR = right + new Vector2 (0.0f, checkDistance);
		bool hit = Physics2D.Linecast (centre, headC, 1 << LayerMask.NameToLayer ("Platforms")) || Physics2D.Linecast (left, headL, 1 << LayerMask.NameToLayer ("Platforms")) || Physics2D.Linecast (right, headR, 1 << LayerMask.NameToLayer ("Platforms"));
		if (hit) {
			Debug.Log ("Collided from below.");
			return true;
		}
		return false;
	}

	bool checkLeft() {
		Collider2D col = GetComponent<Collider2D>();
		Vector2 centre = new Vector2 (col.bounds.center.x - col.bounds.extents.x, col.bounds.center.y);
		Vector2 top = new Vector2 (col.bounds.center.x - col.bounds.extents.x, col.bounds.center.y + col.bounds.extents.y);
		Vector2 bottom = new Vector2 (col.bounds.center.x - col.bounds.extents.x, col.bounds.center.y - col.bounds.extents.y);
		Vector2 edgeC = centre - new Vector2(checkDistance, 0.0f);
		Vector2 edgeT = top - new Vector2 (checkDistance, 0.0f);
		Vector2 edgeB = bottom - new Vector2 (checkDistance, 0.0f);
		bool hit = Physics2D.Linecast (centre, edgeC, 1 << LayerMask.NameToLayer ("Platforms")) || Physics2D.Linecast (top, edgeT, 1 << LayerMask.NameToLayer ("Platforms")) || Physics2D.Linecast (bottom, edgeB, 1 << LayerMask.NameToLayer ("Platforms"));
		if (hit) {
			Debug.Log ("Collided to left.");
			return true;
		}
		return false;
	}

	bool checkRight() {
		Collider2D col = GetComponent<Collider2D>();
		Vector2 centre = new Vector2 (col.bounds.center.x + col.bounds.extents.x, col.bounds.center.y);
		Vector2 top = new Vector2 (col.bounds.center.x + col.bounds.extents.x, col.bounds.center.y + col.bounds.extents.y);
		Vector2 bottom = new Vector2 (col.bounds.center.x + col.bounds.extents.x, col.bounds.center.y - col.bounds.extents.y);
		Vector2 edgeC = centre + new Vector2(checkDistance, 0.0f);
		Vector2 edgeT = top + new Vector2 (checkDistance, 0.0f);
		Vector2 edgeB = bottom + new Vector2 (checkDistance, 0.0f);
		bool hit = Physics2D.Linecast (centre, edgeC, 1 << LayerMask.NameToLayer ("Platforms")) || Physics2D.Linecast (bottom, edgeT, 1 << LayerMask.NameToLayer ("Platforms")) || Physics2D.Linecast (bottom, edgeB, 1 << LayerMask.NameToLayer ("Platforms"));
		if (hit) {
			Debug.Log ("Collided to right.");
			return true;
		}
		return false;
	}

	void enterFall() {
		fallTimer = 0;
		Debug.Log ("Entered falling state.");
	}

	void enterStand() {
		//if (checkGround () != 0) {
		//	transform.Translate (new Vector3(0.0f, -checkGround () * 0.95f, 0.0f));
		//}
		Debug.Log ("Entered standing state.");
	}

	void enterJump() {
		Debug.Log ("Entered jumping state.");
		jumpTimer = 0;

	}

	void moveLeft() {
		if (!checkLeft ()) {
			if (state == PlayerState.JUMPING || state == PlayerState.FALLING) {
				transform.Translate (new Vector3(-walkSpeed * jumpSpeedMod, 0.0f, 0.0f));
			} else {
				transform.Translate (new Vector3(-walkSpeed, 0.0f, 0.0f));
			}
		}
	}

	void moveRight() {
		if (!checkRight ()) {
			if (state == PlayerState.JUMPING || state == PlayerState.FALLING) {
				transform.Translate (new Vector3(walkSpeed * jumpSpeedMod, 0.0f, 0.0f));
			} else {
				transform.Translate (new Vector3(walkSpeed, 0.0f, 0.0f));
			}
		}
	}

	void checkInput() {
		if (Input.GetKey (KeyCode.RightArrow)) {
			moveRight();
			Debug.Log ("Right pressed.");
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			Debug.Log ("Left pressed.");
			moveLeft();
		}
		if (Input.GetKey (KeyCode.Space)) {
			jumpHeld = true;
			if (state == PlayerState.STANDING || state == PlayerState.WALKING) {
				state = PlayerState.JUMPING;
				enterJump ();
			}
			Debug.Log ("Space pressed.");
		} else {
			jumpHeld = false;
			Debug.Log("Space released.");
		}
	}

	
}