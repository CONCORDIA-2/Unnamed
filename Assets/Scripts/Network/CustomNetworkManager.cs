using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager
{
    private Dictionary<NetworkConnection, int> scores = new Dictionary<NetworkConnection, int>();
    private Dictionary<NetworkConnection, int> characters = new Dictionary<NetworkConnection, int>();
    private Dictionary<NetworkConnection, GameObject> players = new Dictionary<NetworkConnection, GameObject>();
    private int clientScore;
    private int readyPlayers = 0;

    private void Start()
    {
        Debug.Log("Start()");
        //client.RegisterHandler(ReadyToAddPlayerMsg.id, OnReadyToAddPlayerMsg);
        NetworkServer.RegisterHandler(QuestionnaireDataStorage.id, OnScoreMsg);
        clientScore = GameObject.FindGameObjectWithTag("ScoreCarrier").GetComponent<ScoreCarrier>().score;
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect()");

        //CharacterMsg msg = new CharacterMsg();
        //msg.characterId = Random.Range(0, 2);
        //msg.character = msg.characterId == 1 ? "Raven" : "Rabbit";
        //Debug.Log("Picked: " + msg.character);
        //singleton.client.Send(CharacterMsg.id, msg);

        client.RegisterHandler(ReadyToAddPlayerMsg.id, OnReadyToAddPlayerMsg);
        ClientScene.Ready(conn);

        QuestionnaireDataStorage scoreMsg = new QuestionnaireDataStorage();
        scoreMsg.score = clientScore;
        client.Send(QuestionnaireDataStorage.id, scoreMsg);

        //ClientScene.AddPlayer(0);
    }

    public override void OnStartHost()
    {
        Debug.Log("OnStartHost()");
    }

    public void OnScoreMsg(NetworkMessage msg)
    {
        Debug.Log("OnScoreMsg()");
        QuestionnaireDataStorage scoreMsg = msg.ReadMessage<QuestionnaireDataStorage>();
        if (!scores.ContainsKey(msg.conn))
            scores.Add(msg.conn, scoreMsg.score);

        if (scores.Count == 2)
            PickCharacters();
    }

    public void OnReadyToAddPlayerMsg(NetworkMessage msg)
    {
        Debug.Log("OnReadyToAddPlayerMsg");
        ReadyToAddPlayerMsg readyMsg = msg.ReadMessage<ReadyToAddPlayerMsg>();
        ClientScene.AddPlayer(0);
    }

    public void PickCharacters()
    {
        Debug.Log("PickCharacters()");
        List<NetworkConnection> conns = new List<NetworkConnection>(scores.Keys);
        NetworkConnection maxConnection = conns[0];
        NetworkConnection minConnection = conns[0];
        int maxScore = int.MinValue;
        int minScore = int.MaxValue;
        bool tied = false;

        foreach (KeyValuePair<NetworkConnection, int> entry in scores)
        {
            if (entry.Value > maxScore)
            {
                maxConnection = entry.Key;
                maxScore = entry.Value;
            }
            else if (entry.Value < minScore)
            {
                minConnection = entry.Key;
                minScore = entry.Value;
            }
            else if (entry.Value == maxScore || entry.Value == minScore)
                tied = true;
        }
        
        if (tied)
        {
            Debug.Log("Tied");
            int random0 = Random.Range(0, 2);
            int random1 = (random0 == 0) ? 1 : 0;

            characters[conns[0]] = random0;
            characters[conns[1]] = random1;
        }
        else
        {
            Debug.Log("Not tied");
            characters[maxConnection] = 0;
            characters[minConnection] = 1;
        }
        //conns[0].

        PreparePrefabs();
    }

    public void PreparePrefabs()
    {
        Debug.Log("SpawnPlayers()");
        //ServerChangeScene("TheGame");
        foreach (KeyValuePair<NetworkConnection, int> entry in characters)
        {
            GameObject prefab = spawnPrefabs[entry.Value];
            players.Add(entry.Key, prefab);

            ReadyToAddPlayerMsg msg = new ReadyToAddPlayerMsg();
            NetworkServer.SendToClient(entry.Key.connectionId, ReadyToAddPlayerMsg.id, msg);
            Debug.Log("Player ready for client");

            //NetworkServer.AddPlayerForConnection(entry.Key, player, 0);
            //Debug.Log("Added player for connection");
        }
        //ServerChangeScene("TheGame");
    }

    //public override void OnServerReady(NetworkConnection conn)
    //{
    //    base.OnServerReady(conn);
    //    ++readyPlayers;

    //    if (readyPlayers == 2)
    //        PickCharacters();
    //}

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("OnServerAddPlayer()");
        if (players.ContainsKey(conn))
        {
            //GameObject prefab = spawnPrefabs[characters[conn]];
            GameObject player = Instantiate(players[conn], startPositions[conn.connectionId].position, startPositions[conn.connectionId].rotation);
            DontDestroyOnLoad(player);
            NetworkServer.AddPlayerForConnection(conn, player, 0);
            ++readyPlayers;
            Debug.Log("Added player for connection: " + conn);
        }

        if (readyPlayers == 2)
            ServerChangeScene("TheGame");
    }
}