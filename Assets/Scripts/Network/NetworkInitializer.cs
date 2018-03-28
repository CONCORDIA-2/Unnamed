using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkInitializer : NetworkDiscovery
{
    [SerializeField] private NetworkManagerHUD hud;

    // used in testing to turn off "Host Game - Join Game" UI
    //[SerializeField] private GameObject connectionUI;

    // set this to "false" when you want to get rid of the HUD
    private bool useDefaultHUD = false;

    private bool connected = false;
    private bool initialized = false;
    private string ip;
    private const short port = 7777;
    private const int discoverKey = 6674;
    private const int discoveryPort = 57777;

    private bool isHost;

    private void Start()
    {
        PlayerPrefs.SetInt("GameFinished", 0);
        DontDestroyOnLoad(gameObject);
        hud = GetComponent<NetworkManagerHUD>();
        //connectionUI = GameObject.FindGameObjectWithTag("ConnectionUI");

        if (useDefaultHUD)
            enabled = false;

        else
        {
        
            hud.enabled = false;

            broadcastKey = discoverKey;
            broadcastPort = discoveryPort;
            useNetworkManager = true;
            showGUI = false;
            NetworkManager.singleton.networkPort = port;
        }

        isHost = GameObject.FindGameObjectWithTag("ScoreCarrier").GetComponent<ScoreCarrier>().isHost;
        Debug.Log("isHost: " + isHost);

        if (isHost)
            StartHost();
        else
            StartClient();
    }

    public void StartHost()
    {
        GetHostInfo();
        NetworkManager.singleton.networkAddress = ip;

        if (!initialized && NetworkIsReady())
        {
            initialized = Initialize();
            StartAsServer();
            NetworkManager.singleton.StartHost();
            //ToggleConnectionUI(false);

            Debug.Log("StartHost initialized = " + initialized + "; hostId = " + hostId);

            //QuestionnaireDataStorage msg = new QuestionnaireDataStorage();
            //msg.score = clientScore;
            //NetworkManager.singleton.client.Send(QuestionnaireDataStorage.id, msg);
        }
    }

    public void StartClient()
    {
        if (!initialized && NetworkIsReady())
        {
            initialized = Initialize();
            StartAsClient();
            Debug.Log("StartClient initialized = " + initialized + "; hostId = " + hostId);
        }
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);

        if (!connected)
        {
            Debug.Log("OnReceivedBroadcast from " + fromAddress + ": " + data);

            NetworkManager.singleton.networkAddress = fromAddress;
            NetworkManager.singleton.StartClient();
            //ToggleConnectionUI(false);

            //QuestionnaireDataStorage msg = new QuestionnaireDataStorage();
            //msg.score = clientScore;
            //NetworkManager.singleton.client.Send(QuestionnaireDataStorage.id, msg);

            connected = true;
        }
    }

    private void GetHostInfo()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress addr in host.AddressList)
        {
            if (addr.AddressFamily == AddressFamily.InterNetwork)
            {
                ip = addr.ToString();
                break;
            }
        }
    }

    private bool NetworkIsReady()
    {
        return !NetworkServer.active
            && !NetworkClient.active;
    }

    //private void ToggleConnectionUI(bool toggle)
    //{
    //    if (connectionUI)
    //        connectionUI.SetActive(toggle); 
    //}
}
