﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterSpawner : MonoBehaviour {

    public static int critterCount = 0;
    public int maxNumCritters = 3;
    public GameObject critterPrefab;
    public Transform spawnLocation;
    public GameObject guardLocation;
    public GameObject player1, player2;

    [SerializeField] private LocalPlayerManager localPlayerManagerScript;
    [SerializeField] private GameObject localPlayerManager;

    private bool spawning = false;

    // Use this for initialization
    void Start () {
		// grab reference to the local player manager if not assigned
        localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
        if (localPlayerManager)
            localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();

        getPlayers();
	}
	
	// Update is called once per frame
	void Update () {
		if (player1 && player2)
		{
	        if (critterCount <= maxNumCritters && !spawning)
	        {
	            spawning = true;
	            critterCount++;
	            StartCoroutine(spawnCritter());
	        }
        } else
            getPlayers();
	}

    private void getPlayers()
    {
        player1 = localPlayerManagerScript.GetLocalPlayerObject();
        player2 = localPlayerManagerScript.GetOtherPlayerObject();
    }

    public IEnumerator spawnCritter()
    { 
        yield return new WaitForSeconds(Random.Range(1,3));  //wait 1-3 second before a spawn
        //find appropriate spawn location
        Debug.Log("Spawning...");
        if (spawnLocation != null)
        {
        	GameObject newCritter = Instantiate(critterPrefab, spawnLocation.position, Quaternion.identity, spawnLocation);
        } else
        {
        	GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("Spawn");
        	Debug.Log(spawnLocations[0]);
        	if (spawnLocations.Length > 0)
                spawnLocation = spawnLocations[0].transform;
        }

        spawning = false;

        yield return null;
    }
}


