using UnityEngine;
using UnityEngine.UI;

public class GameStartButtons : MonoBehaviour
{
    private void Start()
    {
        NetworkInitializer netInit = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkInitializer>();

        GameObject btnHostObj = transform.GetChild(1).gameObject;
        GameObject btnClientObj = transform.GetChild(2).gameObject;

        Button btnHost = btnHostObj.GetComponent<Button>();
        Button btnClient = btnClientObj.GetComponent<Button>();

        btnHost.onClick.AddListener(() => netInit.StartHost());
        btnClient.onClick.AddListener(() => netInit.StartClient());
    }
}
