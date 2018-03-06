using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingText : MonoBehaviour {

    public Transform toFollow;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(toFollow.position.x,
                           toFollow.position.y + 1,
                           toFollow.position.z);
        transform.rotation = Quaternion.identity;
    }
}
