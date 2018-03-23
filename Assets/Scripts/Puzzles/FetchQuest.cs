using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FetchQuest : MonoBehaviour {

    public const string fetch1 = "Fetch1(Clone)", fetch2 = "Fetch2(Clone)", fetch3 = "Fetch3(Clone)", fetch4 = "Fetch4(Clone)", fetch5 = "Fetch5(Clone)";
    public bool f1, f2, f3, f4, f5;
    public static bool succeeded = false, whatElseVisible = false;
    public Text quote1, quote2, quote3, quote4, quote5;
    public GameObject whatElse;
    private int numComplete = 0;

	// Use this for initialization
	void Start () {	
	}
	
    
	// Update is called once per frame
	void Update () {
        if (numComplete > 0 && !whatElseVisible)
        {
            whatElseVisible = true;
            StartCoroutine(WaitAndSetActive(whatElse, true));
        }

        if (f1 && f2 && f3 && f4 && f5)
            succeeded = true;
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        string inputName = collision.gameObject.name;
        switch (inputName)
        {
            case fetch1:
                f1 = true;
                numComplete++;
                collision.gameObject.SetActive(false);
                StartCoroutine(MessageFades.FadeTextToFullAlpha(6f, quote1));
                break;
            case fetch2:
                f2 = true;
                numComplete++;
                collision.gameObject.SetActive(false);
                StartCoroutine(MessageFades.FadeTextToFullAlpha(6f, quote2));
                break;
            case fetch3:
                f3 = true;
                numComplete++;
                collision.gameObject.SetActive(false);
                StartCoroutine(MessageFades.FadeTextToFullAlpha(6f, quote3));
                break;
            case fetch4:
                f4 = true;
                numComplete++;
                collision.gameObject.SetActive(false);
                StartCoroutine(MessageFades.FadeTextToFullAlpha(6f, quote4));
                break;
            case fetch5:
                f5 = true;
                numComplete++;
                collision.gameObject.SetActive(false);
                StartCoroutine(MessageFades.FadeTextToFullAlpha(6f, quote5));
                break;
        }
    }

    public IEnumerator WaitAndSetActive(GameObject toSet, bool active)
    {
        yield return new WaitForSeconds(3);
        toSet.SetActive(active);
    }
}
