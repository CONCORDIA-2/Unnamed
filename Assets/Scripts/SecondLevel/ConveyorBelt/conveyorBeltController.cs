using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conveyorBeltController : MonoBehaviour {

	// public variables
	public float m_conveyorSpeed = 0.5f;
	public bool m_activate = true;

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
	void OnCollisionStay (Collision col) {
		if (col.gameObject != null && m_activate) {
			
			// With translate -------------------
			float step = m_conveyorSpeed * Time.deltaTime;
			col.gameObject.transform.Translate (Vector3.up * step, gameObject.transform);

			// With Velocity -------------------
			// Rigidbody objectRG = col.gameObject.GetComponent<Rigidbody> ();

			// float step = m_conveyorSpeed * Time.deltaTime;
			// objectRG.velocity = new Vector3 (step, 0, 0);
		}
	}
}
