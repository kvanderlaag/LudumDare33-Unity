using UnityEngine;
using System.Collections;

public class BackgroundController : MonoBehaviour {
	public Camera cam;
	public Transform player;
	// Use this for initialization
	public void SetupCam () {
		float sizeX, sizeY;
		sizeX = GetComponent<SpriteRenderer> ().bounds.extents.x * 2f;
		sizeY = GetComponent<SpriteRenderer> ().bounds.extents.y * 2f;
		float camY = cam.orthographicSize * 2f;
		float camX = cam.orthographicSize * cam.aspect * 2f;

		float scaleX = camX / sizeX;
		float scaleY = camY / sizeY;
		transform.localScale = new Vector2 (scaleX, scaleY);
	}
	
	// Update is called once per frame
	public void UpdateBG () {
		transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);// new Vector3(cam.transform.position.x, cam.transform.position.y, 0.0f);
	}
}
