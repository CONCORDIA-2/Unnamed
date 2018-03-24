using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conveyorBeltMaster : MonoBehaviour {

	// public variables
	public GameObject[] m_conveyorElements;
	public GameObject m_conveyorTop;

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
	public void statusConveyor (bool newStatus) {
		// For all textures on the conveyor belt
		for (int i = 0; i < m_conveyorElements.Length; i++) {
			// Change the status of the scroll effect on the textures
			scrollTexture scrollScript = m_conveyorElements [i].GetComponent<scrollTexture> ();
			scrollScript.m_activate = newStatus;
		}

		// For the physic of the top
		conveyorBeltController conveyorScript = m_conveyorTop.GetComponent<conveyorBeltController>();
		conveyorScript.m_activate = newStatus;
	}

}
