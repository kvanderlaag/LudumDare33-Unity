using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (Animator))]
public class PlayerAnimController : MonoBehaviour {

	PlayerController controller;
	Animator anim;
	SpriteRenderer[] sprite;
	public float flashInterval;
	float flashTime;
	public Camera cam;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		controller = GetComponent<PlayerController> ();
		sprite = GetComponentsInChildren<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (controller.playerState == PlayerController.PlayerState.HURT) {
			flashTime += Time.deltaTime;
			if (flashTime >= flashInterval) {
				flashTime = 0;
				for (int i = 0; i < sprite.Length; ++i) {
					if (sprite[i].color == Color.red)
						sprite[i].color = Color.white;
					else if (sprite[i].color == Color.white)
						sprite[i].color = Color.red;
				}
			}
		}
	}

	public void EnterStand() {
		anim.Play (Animator.StringToHash ("Stand"));
	}

	public void EnterWalk() {
		anim.Play (Animator.StringToHash ("Walk"));
	}

	public void EnterJump() {
		playSound ("event:/SFX/Mepo/sfx_mepo_jump");
		anim.Play (Animator.StringToHash ("Jump"));
	}

	public void EnterFall() {
		anim.Play (Animator.StringToHash ("Fall"));
	}

	public void EnterHurt() {
		anim.Play (Animator.StringToHash ("Hurt"));
		playSound ("event:/SFX/Mepo/sfx_mepo_damage");
		for (int i = 0; i < sprite.Length; ++i) {
			sprite [i].color = Color.red;
		}
		flashTime = 0;
	}

	public void Flip() {
		Vector3 scale;
			scale = gameObject.transform.localScale;
			scale.x *= -1;
			gameObject.transform.localScale = scale;
	}

	public void ExitHurt() {
		for (int i = 0; i < sprite.Length; ++i) {
			sprite[i].color = Color.white;	
		}
	}

	public void playSound(string path) {
		FMOD_StudioSystem.instance.PlayOneShot (
			path, cam.transform.position);
	}

	public void hide() {
		for (int i = 0; i < sprite.Length; ++i) {
			sprite[i].enabled = false;	
		}
	}

	public void show() {
		for (int i = 0; i < sprite.Length; ++i) {
			sprite[i].enabled = true;	
		}
	}

}

