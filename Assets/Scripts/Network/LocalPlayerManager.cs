using UnityEngine;

public class LocalPlayerManager : MonoBehaviour
{
    // this script will house local player variables for easy access
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject worldCamera;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject otherPlayerObject;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        worldCamera = GameObject.FindGameObjectWithTag("WorldCamera");
        ToggleWorldCamera(false);
    }

    public GameObject GetPlayerCameraObject()
    {
        return playerCamera;
    }

    public GameObject GetWorldCameraObject()
    {
        return worldCamera;
    }

    public GameObject GetLocalPlayerObject()
    {
        return playerObject;
    }

    public GameObject GetOtherPlayerObject()
    {
        return otherPlayerObject;
    }

    public void SetPlayerObject(GameObject player)
    {
        playerObject = player;
    }
    
    public void SetOtherPlayerObject(GameObject otherPlayer)
    {
        otherPlayerObject = otherPlayer;
    }

    public void ToggleWorldCamera(bool toggle)
    {
        worldCamera.SetActive(toggle);
    }
}
