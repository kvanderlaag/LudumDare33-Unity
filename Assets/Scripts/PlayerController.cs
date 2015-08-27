using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (PlayerAnimController))]
[RequireComponent (typeof (HurtController))]
public class PlayerController : MonoBehaviour {

	public float hurtForce;
	public MeshRenderer level;

	public int maxHealth;
	int health;
	bool fallen;

	float initX, initY;

	public enum PlayerState {
		STANDING,
		WALKING,
		JUMPING,
		FALLING,
		HURT
	}

	public bool facingRight = true;

	public float jumpTime, jumpHeight;
	float jumpVelocity, gravity;

	Vector3 velocity;
	public float moveSpeed = 10f;
	public float maxGravity = 10f;

	public float hurtTime = 0.5f;
	float justHurt;
	float hurtTimer = 0;

	public PlayerAnimController anim;


	public PlayerState playerState;

	Controller2D controller;
	HurtController hurtControl;

	// Use this for initialization
	void Start () {
		initX = transform.position.x;
		initY = transform.position.y;
		health = maxHealth;
		hurtControl = GetComponent<HurtController> ();
		anim = GetComponent<PlayerAnimController> ();
		controller = GetComponent<Controller2D> ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (jumpTime, 2) / 100f;
		jumpVelocity = Mathf.Abs(gravity) * jumpTime;
	}
	
	// Update is called once per frame
	void Update () {

		checkFallen ();

		if (playerState == PlayerState.HURT) {
			hurtTimer -= Time.deltaTime;
			if (hurtTimer <= 0) {
				anim.ExitHurt();
				playerState = PlayerState.STANDING;
				justHurt = hurtTime * 0.6f;
			}
		}
		if (playerState != PlayerState.HURT) {
			if (!controller.collisions.below && velocity.y > 0) {
				if (!(playerState == PlayerState.JUMPING)) {
					playerState = PlayerState.JUMPING;
					anim.EnterJump();
				}
			}
			else if (!controller.collisions.below && velocity.y < 0) {
				if (!(playerState == PlayerState.FALLING)) {
					playerState = PlayerState.FALLING;
					anim.EnterFall();
				}
			} else if (controller.collisions.below && velocity.x != 0 ) {//&& Input.GetAxisRaw ("Horizontal") != 0) {
				if (!(playerState == PlayerState.WALKING)) {
					if (playerState == PlayerState.FALLING || playerState == PlayerState.JUMPING)
						anim.playSound ("event:/SFX/Mepo/sfx_mepo_land");
					playerState = PlayerState.WALKING;
					anim.EnterWalk ();
					if (velocity.x < 0 && facingRight) {
						anim.Flip();
						facingRight = false;
					} else if (velocity.x > 0 && !facingRight) {
						anim.Flip();
						facingRight = true;
					}
				}
			} else if (controller.collisions.below && velocity.x == 0) {
				if (!(playerState == PlayerState.STANDING)) {
					if (playerState == PlayerState.FALLING || playerState == PlayerState.JUMPING)
						anim.playSound ("event:/SFX/Mepo/sfx_mepo_land");
					playerState = PlayerState.STANDING;
					anim.EnterStand ();
				}
			}
		}

		if (playerState == PlayerState.WALKING) {
			if (velocity.x < 0 && facingRight) {
				anim.Flip ();
				facingRight = false;
			} else if (velocity.x > 0 && !facingRight) {
				anim.Flip ();
				facingRight = true;
			}
		}

		if (Input.GetButton("Cancel"))
		    Application.Quit ();

		if (controller.collisions.below || controller.collisions.above)
			velocity.y = 0;

		/* Hurt state debug code
		if (Input.GetKey (KeyCode.H)) {
			anim.EnterHurt ();
			playerState = PlayerState.HURT;
			hurtTimer = hurtTime;
		}
		*/


		
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		if (playerState != PlayerState.HURT) {
			velocity.x = input.x * moveSpeed * Time.deltaTime; // * moveSpeed;
			//velocity.x = Mathf.Clamp (velocity.x, -moveSpeed, moveSpeed); 
		} else {
			if (velocity.y != 0 && velocity.x == 0) {
				velocity.x = input.x * moveSpeed;
			}
			velocity.x += Mathf.Sign (velocity.x) * (gravity / 2) * Time.deltaTime;
		}

		if (Input.GetAxisRaw ("Horizontal") == 0 && playerState != PlayerState.HURT) {
			velocity.x = 0;
		}

		velocity.y += gravity * Time.deltaTime;
		if (Mathf.Sign (velocity.y) == -1 ) {
			velocity.y = Mathf.Clamp (Mathf.Abs (velocity.y), 0, maxGravity) * -1f;
		}

		if (Input.GetKey (KeyCode.Space) && ( (playerState == PlayerState.STANDING || playerState == PlayerState.WALKING) )) {
			velocity.y = jumpVelocity;
		}

		checkHurt ();
		if (!fallen) {
			controller.Move (velocity);
		}
	}

	void checkHurt() {
		hurtControl.HurtUpdate ();
		if (!(playerState == PlayerState.HURT)) {
			if (hurtControl.collisions.above || hurtControl.collisions.below || hurtControl.collisions.left || hurtControl.collisions.right) {
				if (justHurt == 0) {
					enterHurt();
				}
			}
			
			if (justHurt > 0) {
				justHurt -= Time.deltaTime;
				if (justHurt < 0)
					justHurt = 0;
			}
		}
	}

	void enterHurt() {
		playerState = PlayerState.HURT;
		--health;
		if (health <= 0) {
			anim.playSound ("event:/SFX/Mepo/sfx_mepo_death");
			anim.hide();
			Invoke ("reset", 0.5f);
		}
		int hurtDirX, hurtDirY;
		hurtDirX = hurtDirY = 0;
		hurtTimer = hurtTime;
		anim.EnterHurt ();
		if (hurtControl.collisions.above) {
			hurtDirY = -1;
		} else if (hurtControl.collisions.below) {
			hurtDirY = 1;
		}
		if (hurtControl.collisions.left) {
			hurtDirX = 1;
		} else if (hurtControl.collisions.right) {
			hurtDirX = -1;
		}
		
		if (hurtDirY != 0) {
			velocity.x = hurtForce * hurtDirX;
			velocity.y = hurtForce * hurtDirY;
		} else {
			velocity.x = hurtForce * hurtDirX;
		}	
	}

	void checkFallen() {
		if (transform.position.y < level.bounds.min.y - GetComponent<BoxCollider2D> ().bounds.extents.y) {
			anim.hide ();
			fallen = true;
			health = 0;
			anim.playSound ("event:/SFX/Mepo/sfx_mepo_death");
			Invoke ("reset", 0.5f);
		}
	}

	void reset() {
		transform.position = new Vector2 (initX, initY);
		velocity = new Vector2 (0f, 0f);
		health = maxHealth;
		anim.show ();
		fallen = false;
	}

	public Vector2 getVelocity() {
		return velocity;
	}

	public int getHealth() {
		return health;
	}
}
