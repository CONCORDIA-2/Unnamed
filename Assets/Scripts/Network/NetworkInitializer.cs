using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkInitializer : NetworkDiscovery
{
    [SerializeField] private NetworkManagerHUD hud;

    // used in testing to turn off "Host Game - Join Game" UI
    [SerializeField] private GameObject connectionUI;

    // set this to "false" when you want to get rid of the HUD
    private bool useDefaultHUD = false;

    private bool initialized = true;
    private string ip;
    private const short port = 7777;
    private const int discoverKey = 6674;
    private const int discoveryPort = 57777;

    private void Start()
    {
        hud = GetComponent<NetworkManagerHUD>();
        connectionUI = GameObject.FindGameObjectWithTag("ConnectionUI");

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
    }

    public void StartHost()
    {
        GetHostInfo();
        NetworkManager.singleton.networkAddress = ip;

        if (NetworkIsReady())
        {
            Initialize();
            ResetNetwork();
            StartAsServer();
            NetworkManager.singleton.StartHost();
            ToggleConnectionUI(false);
            initialized = true;
        }
    }

    public void StartClient()
    {
        if (NetworkIsReady())
        {
            Initialize();
            ResetNetwork();
            StartAsClient();
            initialized = true;
        }
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);

        Debug.Log("received broadcast from " + fromAddress + ": " + data);
        StopBroadcast();

        NetworkManager.singleton.networkAddress = fromAddress;
        NetworkManager.singleton.StartClient();
        ToggleConnectionUI(false);
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

    private void ResetNetwork()
    {
        if (initialized)
        {
            StopBroadcast();
            if (isServer)
                NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StopClient();
            Network.Disconnect();
            NetworkServer.Reset();
            ToggleConnectionUI(true);
            initialized = false;
        }
    }

    private void ToggleConnectionUI(bool toggle)
    {
        if (connectionUI)
            connectionUI.SetActive(toggle); 
    }
}
