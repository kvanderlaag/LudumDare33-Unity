using UnityEngine;
using System.Collections;

using FMOD.Studio;

public class CameraController : MonoBehaviour {
	
	public float interpVelocity;
	public float minDistance;
	public float followDistance;
	public GameObject target;
	public Vector3 offset;
	Vector3 targetPos;
	public float minX, maxX;

	public Transform level;


	FMOD_StudioEventEmitter music;
	FMOD.Studio.ParameterInstance levelProgress;

	public float targetY = -3;
	// Use this for initialization
	void Start () {

		targetPos = transform.position;
		music = GetComponent<FMOD_StudioEventEmitter> ();
		music.StartEvent ();
		//music = FMOD_StudioSystem.instance.GetEvent ("event:/Music/mx_forest");

		//music.start ();
		//music.getParameter ("level_progress", out levelProgress);
		levelProgress = music.getParameter ("level_progress");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (target)
		{
			float currVal;
			levelProgress.setValue (((transform.position.x - minX) / (maxX - minX)) * 100);
			FMOD_StudioSystem.instance.GetEvent ("event:/Music/mx_forest").setVolume (0.5f);
			levelProgress.getValue(out currVal);
			//Debug.Log (currVal);
			Vector3 posNoZ = transform.position;
			posNoZ.z = target.transform.position.z;
			
			Vector3 targetDirection = (target.transform.position - posNoZ);
			targetDirection.y = targetY;


			interpVelocity = targetDirection.magnitude * 12f;
			
			targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

			if (targetPos.x < minX) {
				targetPos.x = minX;
			} else if (targetPos.x > maxX) {
				targetPos.x = maxX;
			}

			targetPos.y = targetY;
			
			transform.position = Vector3.Lerp( transform.position, targetPos + offset, 0.25f);
			
		}
	}

}