using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkController : NetworkManager
{
    public int mPlayerNumber;
    private int mSpawnNumber = 0;
    public Transform[] spawnPositions;

    //Called on client when connect
    public override void OnClientConnect(NetworkConnection conn)
    {
        // Create message to set the player
        IntegerMessage msg = new IntegerMessage(mPlayerNumber);

        // Call Add player and pass the message
        ClientScene.AddPlayer(conn, 0, msg);
    }

    // Server
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        // Read client message and receive index
        if (extraMessageReader.Length > 0)
        {
            var stream = extraMessageReader.ReadMessage<IntegerMessage>();
            mPlayerNumber = stream.value;
        }

        //Select the prefab from the spawnable objects list
        var playerPrefab = spawnPrefabs[mPlayerNumber];

        // Create player object with prefab
        var player = Instantiate(playerPrefab, spawnPositions[mSpawnNumber].position, Quaternion.identity) as GameObject;

        // Add player object for connection
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public void SetPlayerNumber(int number)
    {
        mPlayerNumber = number;
    }
}