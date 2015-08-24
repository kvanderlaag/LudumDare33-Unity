using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthController : MonoBehaviour {

	public PlayerControllerNew plc;

	public GameObject doop1;
	public GameObject doop2;
	public GameObject doop3;
	public GameObject doop4;
	public GameObject doop5;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		switch (plc.getHealth ()) {
		case 0:
			doop1.GetComponent<Image> ().enabled = false;
			doop2.GetComponent<Image> ().enabled = false;
			doop3.GetComponent<Image> ().enabled = false;
			doop4.GetComponent<Image> ().enabled = false;
			doop5.GetComponent<Image> ().enabled = false;
			break;
		case 1:
			doop1.GetComponent<Image> ().enabled = true;
			doop2.GetComponent<Image> ().enabled = false;
			doop3.GetComponent<Image> ().enabled = false;
			doop4.GetComponent<Image> ().enabled = false;
			doop5.GetComponent<Image> ().enabled = false;
			break;
		case 2:
			doop1.GetComponent<Image> ().enabled = true;
			doop2.GetComponent<Image> ().enabled = true;
			doop3.GetComponent<Image> ().enabled = false;
			doop4.GetComponent<Image> ().enabled = false;
			doop5.GetComponent<Image> ().enabled = false;
			break;
		case 3:
			doop1.GetComponent<Image> ().enabled = true;
			doop2.GetComponent<Image> ().enabled = true;
			doop3.GetComponent<Image> ().enabled = true;
			doop4.GetComponent<Image> ().enabled = false;
			doop5.GetComponent<Image> ().enabled = false;
			break;
		case 4:
			doop1.GetComponent<Image> ().enabled = true;
			doop2.GetComponent<Image> ().enabled = true;
			doop3.GetComponent<Image> ().enabled = true;
			doop4.GetComponent<Image> ().enabled = true;
			doop5.GetComponent<Image> ().enabled = false;
			break;
		case 5:
			doop1.GetComponent<Image> ().enabled = true;
			doop2.GetComponent<Image> ().enabled = true;
			doop3.GetComponent<Image> ().enabled = true;
			doop4.GetComponent<Image> ().enabled = true;
			doop5.GetComponent<Image> ().enabled = true;
			break;
		}
	}
}
