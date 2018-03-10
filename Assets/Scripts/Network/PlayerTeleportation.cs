using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTeleportation : NetworkBehaviour
{
    public LocalPlayerManager playerManager;
    public static Transform spawnLocation;
    public GameObject instance, otherPlayer;
    private bool isPlayer = false;
    private Vector3 spawnOffset;

    public static bool reset = false;

    // Use this for initialization
    void Awake()
    {
        spawnLocation = GameObject.FindGameObjectWithTag("Checkpoint").transform;   //TEMPORARY SET UP FOR CHECKPOINTS
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<LocalPlayerManager>();
        isPlayer = true;
        spawnOffset = new Vector3(0, 0, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (instance == null || otherPlayer == null)
        {
            instance = playerManager.GetLocalPlayerObject();
            otherPlayer = playerManager.GetOtherPlayerObject();
        }
        if (reset)
        {
            reset = false;
            if (isServer)
            {
                InteractableSpawn.reset = true;
                ResetPlayer();
            }
        }
    }

    void ResetPlayer()
    {
        Vector3 localMoveTo = spawnLocation.position + spawnOffset;
        Vector3 otherMoveTo = spawnLocation.position - spawnOffset;
        CmdTeleportPlayer(instance, localMoveTo);
        CmdTeleportPlayer(otherPlayer, otherMoveTo);


        //instance.transform.position = localMoveTo;
        //otherPlayer.transform.position = otherMoveTo;
    }

    [Server]
    public void CmdTeleportPlayer(GameObject target, Vector3 moveTo)
    {
        target.transform.position = moveTo;
        RpcMoveTo(target, moveTo);
    }

    [ClientRpc]
    void RpcMoveTo(GameObject target, Vector3 moveTo)
    {
        target.transform.position = moveTo;
    }

}

