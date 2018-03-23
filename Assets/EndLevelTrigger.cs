using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour {

	// public variables
	public GameObject m_enginePanel;

	// private variables

	// ------------------------------------
	// Use this for initialization
	// ------------------------------------
	void Start () {
		
	}

	// ------------------------------------
	// Update is called once per frame
	// ------------------------------------
	void Update () {
		
	}

	// ------------------------------------
	// Methods
	// ------------------------------------
	void OnTriggerEnter (Collider col) {
		// If the trigger detact one of the player
		if (col.gameObject.tag == "Pickable") {
			// Debug.Log ("Collided with a player");
			// Change the light on the engine panel
			engineLightController panelScript = m_enginePanel.GetComponent<engineLightController> ();
			panelScript.changeLight ();

			// After all, destroy itself
			Destroy(gameObject);
		}
	}

}
