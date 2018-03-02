using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CritterSpawner : NetworkBehaviour {

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
	            StartCoroutine(SpawnCritter());
	        }
        } else
            getPlayers();
	}

    private void getPlayers()
    {
        player1 = localPlayerManagerScript.GetLocalPlayerObject();
        player2 = localPlayerManagerScript.GetOtherPlayerObject();
    }

    public IEnumerator SpawnCritter()
    { 
        yield return new WaitForSeconds(Random.Range(1,3));  //wait 1-3 second before a spawn
        //find appropriate spawn location
        Debug.Log("Spawning...");
        if (spawnLocation != null)
        {
        	GameObject newCritter = Instantiate(critterPrefab, spawnLocation.position, Quaternion.identity, spawnLocation);
            NetworkServer.Spawn(newCritter);
        } else
        {
        	GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("Spawn");
        	if (spawnLocations.Length > 0)
                spawnLocation = spawnLocations[0].transform;
        }

        spawning = false;

        yield return null;
    }
}


