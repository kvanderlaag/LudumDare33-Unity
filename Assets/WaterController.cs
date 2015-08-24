using UnityEngine;
using System.Collections;

public class WaterController : MonoBehaviour {

	public GameObject background;
	Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - background.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = background.transform.position + offset;
	}
}
