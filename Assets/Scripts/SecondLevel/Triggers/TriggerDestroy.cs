using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDestroy : MonoBehaviour {

	// ------------------------------------
	// Methods
	// ------------------------------------
	void OnTriggerEnter (Collider col) {
		// Destroy any pickable objects that pass throught the trigger
		if (col.gameObject.tag == "Pickable") {
			Destroy (col.gameObject);
		}
	}

}
