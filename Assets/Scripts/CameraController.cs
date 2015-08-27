using UnityEngine;
using System.Collections;
using FMOD.Studio;

public class CameraController : MonoBehaviour {

	public Transform player;
	public Transform level;

	float minX, maxX;
	float targetY;

	FMOD.Studio.ParameterInstance levelProgress;

	public Vector3 targetNoYZ;
	
	public float lerpSpeed = 2f;

	// Use this for initialization
	void Start () {

		GetComponent<FMOD_StudioEventEmitter> ().StartEvent ();
		levelProgress = GetComponent<FMOD_StudioEventEmitter> ().getParameter ("level_progress");

		GetComponent<Camera> ().orthographicSize = level.GetComponent<MeshRenderer> ().bounds.extents.y;
		float halfWidth = GetComponent<Camera> ().aspect * GetComponent<Camera> ().orthographicSize;
		minX = level.GetComponent<MeshRenderer>().bounds.min.x + halfWidth;
		maxX = level.GetComponent<MeshRenderer>().bounds.max.x - halfWidth;
		targetY = level.position.y - level.GetComponent<MeshRenderer>().bounds.extents.y;
		transform.position = new Vector3 (Mathf.Clamp (player.position.x, minX, maxX), targetY, transform.position.z);
		GameObject.Find ("Background").GetComponent<BackgroundController>().SetupCam ();
	}
	
	// Update is called once per frame
	void Update () {

		levelProgress.setValue ((transform.position.x - minX) / (maxX - minX) * 100);
		targetNoYZ = new Vector3 (player.transform.position.x, targetY, transform.position.z);

		targetNoYZ.x = Mathf.Clamp (targetNoYZ.x, minX, maxX);


		transform.position = Vector3.Lerp (transform.position, targetNoYZ, Time.deltaTime * lerpSpeed);
		GameObject.Find ("Background").GetComponent<BackgroundController>().UpdateBG();
	}
}
