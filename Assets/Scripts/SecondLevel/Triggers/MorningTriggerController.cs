using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorningTriggerController : MonoBehaviour {

	// public variables
	public GameObject m_enginePanel;
	public GameObject m_conveyorBelt;
	public bool m_activating;

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
		if (col.gameObject.tag == "Player") {
			// Debug.Log ("Collided with a player");
			// Change the light on the engine panel
			engineLightController panelScript = m_enginePanel.GetComponent<engineLightController> ();
			panelScript.changeLight ();

			// Start the conveyor belt
			conveyorBeltMaster conveyorScript = m_conveyorBelt.GetComponent<conveyorBeltMaster> ();
			conveyorScript.statusConveyor (m_activating);

			// After all, destroy itself
			Destroy(gameObject);
		}
	}

}
