using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour {

	public float rotSpeed;

	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.right * rotSpeed);
	}
}
