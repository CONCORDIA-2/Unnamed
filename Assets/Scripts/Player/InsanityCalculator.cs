using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsanityCalculator : MonoBehaviour {

    public GameObject otherPlayer;
    [SerializeField] private GameObject localPlayerManager;
    [SerializeField] private LocalPlayerManager localPlayerManagerScript;

    public static readonly float safeRadius = 8;
    public static readonly float maxTimeSeparate = 20;	//seconds

    public float insanityLevel;
    public float insanityRate = 0.03f;
    public float distance;
    private static float updateFrequency = 10;

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

            //if within the safe distance, rapidly reduce insanity towards 0
            if (distance < safeRadius)
            {
                insanityLevel -= insanityRate * 10f;
                if (insanityLevel < 0)
					insanityLevel = 0;
            }
            //if outside the safe radius, increase insanity towards 100
            else
            {
                insanityLevel += insanityRate;
                if (insanityLevel > 100)
                	insanityLevel = 100;
            }
        }
    }
}
