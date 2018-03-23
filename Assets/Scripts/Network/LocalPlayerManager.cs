using UnityEngine;

public class LocalPlayerManager : MonoBehaviour
{
    // this script will house local player variables for easy access
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject worldCamera;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject otherPlayerObject;
    [SerializeField] private bool isRaven;
    [SerializeField] private bool otherIsIncapacitated = false;
    [SerializeField] private float otherSanityLevel = 100.0f;

    private void Start()
    {
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

    public bool IsRaven()
    {
        return isRaven;
    }

    public bool OtherIsIncapacitated()
    {
        return otherIsIncapacitated;
    }
    
    public float GetOtherSanityLevel()
    {
        return otherSanityLevel;
    }

    public void SetPlayerObject(GameObject player)
    {
        playerObject = player;
    }
    
    public void SetOtherPlayerObject(GameObject otherPlayer)
    {
        otherPlayerObject = otherPlayer;
    }

    public void SetIsRaven(bool toggle)
    {
        isRaven = toggle;
    }

    public void SetOtherIsIncapacitated(bool toggle)
    {
        otherIsIncapacitated = toggle;
    }

    public void SetOtherSanityLevel(float level)
    {
        otherSanityLevel = level;
    }

    public void ToggleWorldCamera(bool toggle)
    {
        worldCamera.SetActive(toggle);
    }
}
