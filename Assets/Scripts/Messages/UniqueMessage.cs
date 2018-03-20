using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class UniqueMessage : NetworkBehaviour
{
    private bool set = false, isRaven;
    public Text wallMessage;
    public string raven, rabbit;
    public LocalPlayerManager localPlayerManager;
    public GameObject player;

    // Use this for initialization
    public override void OnStartLocalPlayer()
    {
        wallMessage = this.gameObject.GetComponent<Text>();
        localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<LocalPlayerManager>();
        player = localPlayerManager.GetLocalPlayerObject();
    }
	
	// Update is called once per frame
	void Update () {
        if (wallMessage == null || localPlayerManager == null || player == null)
        {
            wallMessage = this.gameObject.GetComponent<Text>();
            localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<LocalPlayerManager>();
            player = localPlayerManager.GetLocalPlayerObject();
            isRaven = player.GetComponent<LocalPlayerSetup>().IsRaven();
        }
        if (!set) //&& isLocalPlayer)
        {
            if (isRaven)
                wallMessage.text = raven;
            else
                wallMessage.text = rabbit;
            set = true;
        }
	}
}
