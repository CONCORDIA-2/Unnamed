using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager
{
    private Dictionary<NetworkConnection, int> characters = new Dictionary<NetworkConnection, int>();

    private void Start()
    {
        Debug.Log("Start()");
        NetworkServer.RegisterHandler(CharacterMsg.id, OnCharacterMsg);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect()");
        //GameObject playerObject = (GameObject)Instantiate(Resources.Load("Player/Raven"));
        //DontDestroyOnLoad(playerObject);

        CharacterMsg msg = new CharacterMsg();
        msg.characterId = Random.Range(0, 2);
        msg.character = msg.characterId == 1 ? "Raven" : "Rabbit";
        Debug.Log("Picked: " + msg.character);
        singleton.client.Send(CharacterMsg.id, msg);

        ClientScene.Ready(conn);
        ClientScene.AddPlayer(0);
        //base.OnClientConnect(conn);
    }

    public override void OnStartHost()
    {
        Debug.Log("OnStartHost()");
        //GameObject playerObject = (GameObject)Instantiate(Resources.Load("Player/Rabbit"));
        //DontDestroyOnLoad(playerObject);
    }

    public void OnCharacterMsg(NetworkMessage msg)
    {
        Debug.Log("OnCharacterMsg()");
        CharacterMsg charMsg = msg.ReadMessage<CharacterMsg>();
        if (!characters.ContainsKey(msg.conn))
            characters.Add(msg.conn, charMsg.characterId);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("OnServerAddPlayer()");
        GameObject player = null;
        if (characters.ContainsKey(conn))
        {
            //playerPrefab = Resources.Load("Player/" + characters[conn]) as GameObject;
            //player = (GameObject)Instantiate(Resources.Load("Player/" + characters[conn]));
            GameObject prefab = spawnPrefabs[characters[conn]];
            player = Instantiate(prefab);
            DontDestroyOnLoad(player);
            NetworkServer.AddPlayerForConnection(conn, player, 0);
            Debug.Log("Added player for connection");
        }
        else
            Debug.Log("We have problems");
        //base.OnServerAddPlayer(conn, playerControllerId);
    }
}