using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class engineLightController : MonoBehaviour {

	// public variables
	public GameObject m_startLight;
	public GameObject m_nextLight;


	// ------------------------------------
	// Use this for initialization
	// ------------------------------------
	void Start () {
		m_startLight.SetActive (true);
		m_nextLight.SetActive (false);
	}

	// ------------------------------------
	// Methods
	// ------------------------------------
	public void changeLight () {
		GetComponent<AudioSource>().Play();
		m_startLight.SetActive (false);
		m_nextLight.SetActive (true);
	}

}
