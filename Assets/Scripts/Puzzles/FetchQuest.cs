using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FetchQuest : MonoBehaviour {

    public string toFetch;
    public bool succeeded = false;
    public Text successText;

	// Use this for initialization
	void Start () {	
	}
	
    
	// Update is called once per frame
	void Update () {
        if (succeeded)
            StartCoroutine(MessageFades.FadeTextToFullAlpha(6f, successText));

        /*
        if (!succeeded && !toFetch.activeInHierarchy)   //if the object is somehow lost before completing the puzzle
        {
            toFetch = Instantiate(toFetch);
            toFetch.SetActive(true);
        }
        */
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == toFetch)
        {
            succeeded = true;
            collision.gameObject.SetActive(false);
            StartCoroutine(MessageFades.FadeTextToFullAlpha(6f, successText));
        }
    }
}
