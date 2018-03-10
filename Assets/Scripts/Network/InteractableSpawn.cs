using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InteractableSpawn : NetworkBehaviour
{

    public GameObject toBeSpawned;
    private Transform spawnLocation;
    public GameObject instance;

    public static bool reset = false;

    public void Awake()
    {
        spawnLocation = this.transform;
    }

    void Update()
    {
        if ((reset || instance == null) && isServer)
        {
            SpawnObject();
            reset = false;
            //StartCoroutine(WaitAndReset());
        }
    }

    void SpawnObject()
    {
        if (instance != null)
            Destroy(instance);
        instance = Instantiate(toBeSpawned, spawnLocation.position, Quaternion.identity, spawnLocation);
        NetworkServer.Spawn(instance);
    }

    /*
    void GlobalReset(bool newReset)
    {
        reset = newReset;
    }
    */
    /*
    public IEnumerator WaitAndReset()
    {
        yield return new WaitForSeconds(0.2f);
        reset = false;
    }
    */

}
