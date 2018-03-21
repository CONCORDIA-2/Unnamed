using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkManager))]
public class NetworkInitializer : NetworkDiscovery
{
    [SerializeField] private NetworkManager manager;
    [SerializeField] private NetworkManagerHUD hud;

    // set this to "false" when you want to get rid of the HUD
    [SerializeField] private bool useHUD = false;

    // used in testing to turn off "Host Game - Join Game" UI
    //[SerializeField] private GameObject connectionUI;

    private bool initialized = true;
    private string ip;
    private const short port = 7777;
    private const int discoverKey = 6674;
    private const int discoveryPort = 57777;

    private void Awake()
    {
        hud = GetComponent<NetworkManagerHUD>();

        if (useHUD)
            enabled = false;

        else
        {
            hud.enabled = false;

            broadcastKey = discoverKey;
            broadcastPort = discoveryPort;
            useNetworkManager = true;
            showGUI = false;
            manager = GetComponent<NetworkManager>();
            manager.networkPort = port;
        }

        //connectionUI = GameObject.Find("ConnectionUI");
    }

    public void StartHost()
    {
        GetHostInfo();
        manager.networkAddress = ip;

        if (NetworkIsReady())
        {
            Initialize();
            ResetNetwork();
            StartAsServer();
            manager.StartHost();
            //ToggleConnectionUI(false);
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
        Debug.Log("received broadcast from " + fromAddress + ": " + data);
        manager.networkAddress = fromAddress;
        if (!NetworkClient.active)
        {
            manager.StartClient();
            StopBroadcast();
            //ToggleConnectionUI(false);
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

    private void ResetNetwork()
    {
        if (initialized)
        {
            if (isServer)
                StopBroadcast();
            manager.StopClient();
            manager.StopHost();
            Network.Disconnect();
            NetworkServer.Reset();
            //ToggleConnectionUI(true);
            initialized = false;
        }
    }

    private void ToggleConnectionUI(bool toggle)
    {
        //connectionUI.SetActive(toggle);
    }
}
