using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsanityCalculator : MonoBehaviour {

    public GameObject otherPlayer;
    [SerializeField] private GameObject localPlayerManager;
    [SerializeField] private LocalPlayerManager localPlayerManagerScript;

    public static readonly float safeRadius = 12;
    public static readonly float safeLightingRadius = 4;
    public static readonly float maxTimeSeparate = 20;	//seconds

    public float insanityLevel;
    public float insanityRate = 0.001f;
    public float distance;
    private static float updateFrequency = 10;

    public bool specialLighting = false;
    public bool isRaven = true;

    void Start() {
		// grab reference to the local player manager if not assigned
	    localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
	    if (localPlayerManager)
	        localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();

		otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
    }

	void Update() {
		if (otherPlayer)
			StartCoroutine("InsanityRoutine");
		else
			otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
	}

    // Update is called once per frame
    IEnumerator InsanityRoutine() {
        while (true)
        {
        	yield return new WaitForSeconds(1f);

            //retrieve the distance between players
            distance = Vector3.Distance(transform.position, otherPlayer.transform.position);

            if (specialLighting && distance > safeLightingRadius)
            {
                increaseInsanity();
                if (GetComponent<Rigidbody>().mass < 120.0f)
                    GetComponent<Rigidbody>().mass += 0.001f;
            } else
            {
                if (GetComponent<Rigidbody>().mass > 25.0f)
                    GetComponent<Rigidbody>().mass -= 0.002f;
            }

            //if within the safe distance, rapidly reduce insanity towards 0
            if (distance < safeRadius)
                decreaseInsanity();
            //if outside the safe radius, increase insanity towards 100
            else increaseInsanity();
        }
    }

    void increaseInsanity()
    {
        insanityLevel += insanityRate;
        if (insanityLevel > 100)
            insanityLevel = 100;
    }

    void decreaseInsanity()
    {
        insanityLevel -= insanityRate * 5f;
        if (insanityLevel < 0)
            insanityLevel = 0;
    }

    //on collision enter with collider specific to client, toggle specialLighting flag
    void OnTriggerEnter(Collider collision)
    {
        if (isRaven) {
            if (collision.gameObject.tag == "LitArea") specialLighting = true;
        } else {
            if (collision.gameObject.tag == "DarkArea") specialLighting = true;
        }
    }

    //on collision exit with collider specific to client, toggle specialLighting flag
    void OnTriggerExit(Collider collision)
    {
        if (isRaven) {
            if (collision.gameObject.tag == "LitArea") specialLighting = false;
        } else {
            if (collision.gameObject.tag == "DarkArea") specialLighting = false;
        }
    }
}
