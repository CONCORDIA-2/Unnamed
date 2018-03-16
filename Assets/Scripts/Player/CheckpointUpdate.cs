using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointUpdate : MonoBehaviour {

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Setting checkpoint to " + this.gameObject);
            PlayerTeleportation.spawnLocation = this.transform;
        }
    }
}
