using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchQuest : MonoBehaviour {

    public GameObject toFetch;
    private bool succeeded = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!succeeded && !toFetch.activeInHierarchy)   //if the object is somehow lost before completing the puzzle
        {
            toFetch = Instantiate(toFetch);
            toFetch.SetActive(true);
        }

	}

    private void OnCollisionEnter(Collision collision)
    {
        if (GameObject.ReferenceEquals(collision.gameObject, toFetch))
        {
            Debug.Log("You fetched it!");
            Destroy(collision.gameObject);
        }
    }
}
