using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nutsGenerator : MonoBehaviour {

	// public variables
	public GameObject m_nuts;

	// private variables

	// ------------------------------------
	// Use this for initialization
	// ------------------------------------
	void Start () {
		StartCoroutine (spawnNut ());
		
	}

	// ------------------------------------
	// Update is called once per frame
	// ------------------------------------
	void Update () {
		
	}

	// ------------------------------------
	// Methods
	// ------------------------------------
	IEnumerator spawnNut () {
		while (true) {
			GameObject newBolt = Instantiate (m_nuts, transform.position, transform.rotation)as GameObject;
			newBolt.transform.parent = gameObject.transform.parent;
			yield return new WaitForSeconds(1);
		}
		
	}

}
