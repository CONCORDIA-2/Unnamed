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
    private string displayMessage;

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
        }

        if (localPlayerManager.IsRaven()) displayMessage = raven;
        else displayMessage = rabbit;

        //Debug.Log(localPlayerManager.IsRaven);

        //while (!set) //&& isLocalPlayer)
        //{
            isRaven = localPlayerManager.IsRaven();
            if (isRaven)
                wallMessage.text = displayMessage;
            else
                wallMessage.text = displayMessage;
            set = true;
        //}
	}
}
