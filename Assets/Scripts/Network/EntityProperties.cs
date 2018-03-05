using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EntityProperties : NetworkBehaviour
{
    public class PlayerMessage : MessageBase
    {
        public const short id = 77;
        public float sendTime;

        // sanity and light system
        public bool isIncapacitated;
    }

    public class TransformMessage : MessageBase
    {
        public const short id = 777;
        public float sendTime;

        // position
        public float posX;
        public float posY;
        public float posZ;

        // rotation
        public float rotX;
        public float rotY;
        public float rotZ;
    }

    public class RigidbodyMessage : MessageBase
    {
        public const short id = 7777;
        public float sendTime;

        // velocity
        public float velX;
        public float velY;
        public float velZ;

        // angular velocity
        public float angX;
        public float angY;
        public float angZ;
    }

    // what we need to serialize: player properties
    [SerializeField] private bool isIncapacitated;

    // what we need to serialize: transform properties
    [SerializeField] private float posX;
    [SerializeField] private float posY;
    [SerializeField] private float posZ;
    [SerializeField] private float rotX;
    [SerializeField] private float rotY;
    [SerializeField] private float rotZ;

    // what we need to serialize: rigidbody properties
    [SerializeField] private float velX;
    [SerializeField] private float velY;
    [SerializeField] private float velZ;
    [SerializeField] private float angX;
    [SerializeField] private float angY;
    [SerializeField] private float angZ;

    // values warranting attribute updates on the client
    [SerializeField] private const float epsilonPosition = 0.25f;
    [SerializeField] private const float epsilonVelocity = 0.25f;
    [SerializeField] private const float epsilonRotation = 5.00f;
    [SerializeField] private const float epsilonAngularVelocity = 45f;

    // time between reconciliation events
    [SerializeField] private const float syncSendInterval = 1.0f;
    //[SerializeField] private const float syncUpdateInterval = 0.25f;
    [SerializeField] private float syncSendTimer = 0.0f;
    //[SerializeField] private float syncUpdateTimer = 0.0f;

    // reference to local player network connection
    [SerializeField] private NetworkConnection networkConn;

    // reference to entity scripts
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SanityAndLight sal;

    public override void OnStartServer()
    {
        base.OnStartServer();

        // set message handlers
        NetworkServer.RegisterHandler(PlayerMessage.id, HandlePlayerMessage);
        NetworkServer.RegisterHandler(TransformMessage.id, HandleTransformMessage);
        NetworkServer.RegisterHandler(RigidbodyMessage.id, HandleRigidbodyMessage);
    }

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            // grab reference to network connection
            networkConn = connectionToServer;

            // grab references to entity components
            rb = GetComponent<Rigidbody>();
            sal = GetComponent<SanityAndLight>();

            // set message handlers
            NetworkManager.singleton.client.RegisterHandler(PlayerMessage.id, HandlePlayerMessage);
            NetworkManager.singleton.client.RegisterHandler(TransformMessage.id, HandleTransformMessage);
            NetworkManager.singleton.client.RegisterHandler(RigidbodyMessage.id, HandleRigidbodyMessage);
        }

        else if (isServer)
            // grab reference to network connection
            networkConn = connectionToClient;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            // update local properties
            UpdatePlayerProperties();
            UpdateTransformProperties();
            UpdateRigidbodyProperties();

            // send them over to the server at regular intervals
            syncSendTimer += Time.deltaTime;
            if (syncSendTimer >= syncSendInterval)
            {
                SendPlayerProperties();
                SendTransformProperties();
                SendRigidbodyProperties();

                syncSendTimer = 0.0f;
            }

            //syncUpdateTimer += Time.deltaTime;
        }
    }

    void UpdatePlayerProperties()
    {
        isIncapacitated = sal.isIncapacitated;
    }

    void UpdateTransformProperties()
    {
        posX = transform.position.x;
        posY = transform.position.y;
        posZ = transform.position.z;
    }

    void UpdateRigidbodyProperties()
    {
        velX = rb.velocity.x;
        velY = rb.velocity.y;
        velZ = rb.velocity.z;

        angX = rb.angularVelocity.x;
        angY = rb.angularVelocity.y;
        angZ = rb.angularVelocity.z;
    }

    void SendPlayerProperties()
    {
        PlayerMessage msg = new PlayerMessage();
        msg.sendTime = Time.time;

        msg.isIncapacitated = isIncapacitated;

        NetworkManager.singleton.client.Send(PlayerMessage.id, msg);
    }

    void SendTransformProperties()
    {
        TransformMessage msg = new TransformMessage();
        msg.sendTime = Time.time;

        msg.posX = posX;
        msg.posY = posY;
        msg.posZ = posZ;

        NetworkManager.singleton.client.Send(TransformMessage.id, msg);
    }

    void SendRigidbodyProperties()
    {
        RigidbodyMessage msg = new RigidbodyMessage();
        msg.sendTime = Time.time;

        msg.velX = velX;
        msg.velY = velY;
        msg.velZ = velZ;

        msg.angX = angX;
        msg.angY = angY;
        msg.angZ = angZ;

        NetworkManager.singleton.client.Send(RigidbodyMessage.id, msg);
    }

    void HandlePlayerMessage(NetworkMessage msg)
    {
        PlayerMessage playerMsg = msg.ReadMessage<PlayerMessage>();
        if (isServer)
        {
            Debug.Log("playermsg: hi from server");
            NetworkServer.SendToClient(networkConn.connectionId, PlayerMessage.id, playerMsg);
        }
        else if (isLocalPlayer)
            Debug.Log("playermsg: hi from client");
    }

    void HandleTransformMessage(NetworkMessage msg)
    {
        TransformMessage transformMsg = msg.ReadMessage<TransformMessage>();
        if (isServer)
        {
            Debug.Log("transformmsg: hi from server");
            NetworkServer.SendToClient(networkConn.connectionId, PlayerMessage.id, transformMsg);
        }
        else if (isLocalPlayer)
            Debug.Log("transformmsg: hi from client");
    }

    void HandleRigidbodyMessage(NetworkMessage msg)
    {
        RigidbodyMessage rigidbodyMsg = msg.ReadMessage<RigidbodyMessage>();
        if (isServer)
        {
            Debug.Log("rigidbodymsg: hi from server");
            NetworkServer.SendToClient(networkConn.connectionId, PlayerMessage.id, rigidbodyMsg);
        }
        else if (isLocalPlayer)
            Debug.Log("rigidbodymsg: hi from client");
    }
}