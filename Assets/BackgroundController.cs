using UnityEngine;
using System.Collections;

public class BackgroundController : MonoBehaviour {
	Vector3 offset;
	public GameObject cam;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + 1f, 0.0f);
	}
}
