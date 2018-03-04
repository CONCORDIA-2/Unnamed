using UnityEngine;
using UnityEngine.Networking;

public class LocalPlayerSetup : NetworkBehaviour
{
    [SerializeField] private GameObject localPlayerManager;
    [SerializeField] private GameObject camObject;
    [SerializeField] private GameObject otherPlayerObject;
    [SerializeField] private CameraControl camControl;
    [SerializeField] private LocalPlayerManager localPlayerManagerScript;
    [SerializeField] private float checkInterval = 0.5f;
    [SerializeField] private bool isRaven;

    private float checkTimer = 0.0f;

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            // grab reference to the local player manager if not assigned
            localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
            if (localPlayerManager)
                localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();

            // set the local player object in the local player manager
            if (!camObject)
                camObject = localPlayerManagerScript.GetPlayerCameraObject();
            localPlayerManagerScript.SetPlayerObject(gameObject);

            // set camera target to be the local player game object
            if (!camControl)
                camControl = camObject.GetComponent<CameraControl>();
            camControl.SetCameraTarget(transform);
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            // periodically check if other player is still connected/has connected
            checkTimer += Time.deltaTime;
            if (checkTimer >= checkInterval)
            {
                FindOtherPlayerObject();
                checkTimer = 0.0f;
            }
        }
    }

    void FindOtherPlayerObject()
    {
        // searches for players' transform and adds it to the list if present
        GameObject[] playerObjs = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObj in playerObjs)
        {
            if (playerObj != gameObject)
            {
                otherPlayerObject = playerObj;

                // set the other player object in the local player manager
                localPlayerManagerScript.SetOtherPlayerObject(otherPlayerObject);
            }
        }
    }
}