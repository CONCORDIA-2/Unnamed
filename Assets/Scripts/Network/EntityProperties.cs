using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EntityProperties : NetworkBehaviour
{
    //[SyncVar(hook = "OnReceivePlayerProperties")] public PlayerProperties playerProperties;
    //[SyncVar(hook = "OnReceiveTransformProperties")] public TransformProperties transformProperties;
    [SyncVar] public PlayerProperties playerProperties;
    [SyncVar] public TransformProperties transformProperties;

    private PlayerProperties lastReceveivedPlayerProperties;
    private TransformProperties lastReceveivedTransformProperties;

    // values warranting attribute updates on the client
    [SerializeField] private float epsilonPosition = 0.2f;
    [SerializeField] private float epsilonVelocity = 0.1f;
    [SerializeField] private float epsilonRotation = 5.0f;
    [SerializeField] private float epsilonAngularVelocity = 15f;

    // time between reconciliation events
    [SerializeField] private float syncInterval = 0.25f;
    private float syncTimer = 0.0f;

    // reference to entity scripts
    private Rigidbody rb;
    private SanityAndLight sal;
    
    public struct PlayerProperties
    {
        public float sendTime;

        public bool isIncapacitated;
        // anything else we need here?
    }

    public struct TransformProperties
    {
        public float sendTime;

        public Vector3 position;
        public Vector3 velocity;
        public Quaternion rotation;
        public Vector3 angularVelocity;
    }

    public override void OnStartLocalPlayer()
    {
        // grab references to entity components
        rb = GetComponent<Rigidbody>();
        sal = GetComponent<SanityAndLight>();

        // set initial states
        playerProperties.sendTime = 0.0f;
        playerProperties.isIncapacitated = sal.isIncapacitated;

        transformProperties.sendTime = 0.0f;
        transformProperties.position = transform.position;
        transformProperties.velocity = rb.velocity;
        transformProperties.rotation = transform.rotation;
        transformProperties.angularVelocity = rb.angularVelocity;

        lastReceveivedPlayerProperties = playerProperties;
        lastReceveivedTransformProperties = transformProperties;

        // send initial states to server
        CmdSendPlayerProperties(playerProperties);
        CmdSendTransformProperties(transformProperties);
    }

    private void Update()
    {
        // if local player, send local properties to server
        if (isLocalPlayer)
        {
            CmdSendPlayerProperties(playerProperties);
            CmdSendTransformProperties(transformProperties);
        }

        // if server, send received properties to clients regularly
        if (isServer)
        {
            syncTimer += Time.deltaTime;
            if (syncTimer >= syncInterval)
            {
                RpcSyncPlayerProperties(playerProperties);
                RpcSyncTransformProperties(transformProperties);
            }
        }
    }

    // send properties on channel 1 (unreliable) to avoid congestion
    [Command(channel = 1)]
    void CmdSendPlayerProperties(PlayerProperties properties)
    {
        playerProperties.sendTime = Time.time;

        playerProperties.isIncapacitated = properties.isIncapacitated;
        // anything else we need here?
    }

    // send properties on channel 1 (unreliable) to avoid congestion
    [Command(channel = 1)]
    void CmdSendTransformProperties(TransformProperties properties)
    {
        transformProperties.sendTime = Time.time;

        transformProperties.position = properties.position;
        transformProperties.velocity = properties.velocity;
        transformProperties.rotation = properties.rotation;
        transformProperties.angularVelocity = properties.angularVelocity;
    }

    [ClientRpc]
    void RpcSyncPlayerProperties(PlayerProperties properties)
    {
        // check the timestamp of the properties to make sure they're the latest
        if (properties.sendTime >= lastReceveivedPlayerProperties.sendTime)
        {
            playerProperties.isIncapacitated = properties.isIncapacitated;
            // antyhing else we need here?
        }
    }

    [ClientRpc]
    void RpcSyncTransformProperties(TransformProperties properties)
    {
        // check the timestamp of the properties to make sure they're the latest
        if (properties.sendTime >= lastReceveivedTransformProperties.sendTime)
        {            
            transform.position = properties.position;
            rb.velocity = properties.velocity;
            transform.rotation = properties.rotation;
            rb.angularVelocity = properties.angularVelocity;
        }
    }

    void UpdatePlayerProperties()
    {
        // only update properties if they are different
        if (sal.isIncapacitated != playerProperties.isIncapacitated)
            sal.isIncapacitated = playerProperties.isIncapacitated;
    }

    void UpdateTransformProperties()
    {
        // only update properties if they differ by epsilon presets
        bool updateX;
        bool updateY;
        bool updateZ;

        // position
        updateX = Mathf.Abs(transform.position.x - transformProperties.position.x) >= epsilonPosition;
        updateY = Mathf.Abs(transform.position.x - transformProperties.position.x) >= epsilonPosition;
        updateZ = Mathf.Abs(transform.position.x - transformProperties.position.x) >= epsilonPosition;

        transform.position = new Vector3(updateX ? transformProperties.position.x : transform.position.x,
            updateY ? transformProperties.position.x : transform.position.x,
            updateZ ? transformProperties.position.x : transform.position.x);

        // velocity
        updateX = Mathf.Abs(rb.velocity.x - transformProperties.velocity.x) >= epsilonVelocity;
        updateY = Mathf.Abs(rb.velocity.y - transformProperties.velocity.y) >= epsilonVelocity;
        updateZ = Mathf.Abs(rb.velocity.z - transformProperties.velocity.z) >= epsilonVelocity;

        rb.velocity = new Vector3(updateX ? transformProperties.velocity.x : rb.velocity.x,
            updateY ? transformProperties.velocity.y : rb.velocity.y,
            updateZ ? transformProperties.velocity.z : rb.velocity.z);

        // rotation
        Vector3 rotationEuler = transform.rotation.eulerAngles;
        Vector3 rotationEulerReceived = transformProperties.rotation.eulerAngles;
        updateX = Mathf.Abs(rotationEuler.x - rotationEulerReceived.x) >= epsilonRotation;
        updateY = Mathf.Abs(rotationEuler.y - rotationEulerReceived.y) >= epsilonRotation;
        updateZ = Mathf.Abs(rotationEuler.z - rotationEulerReceived.z) >= epsilonRotation;

        transform.rotation = Quaternion.Euler(updateX ? rotationEulerReceived.x : rotationEuler.x,
            updateY ? rotationEulerReceived.y : rotationEuler.y,
            updateZ ? rotationEulerReceived.z : rotationEuler.z);

        // angular velocity - set epsilon to a high value to prevent erratic behaviour
        updateX = Mathf.Abs(rb.angularVelocity.x - transformProperties.angularVelocity.x) >= epsilonAngularVelocity;
        updateY = Mathf.Abs(rb.angularVelocity.y - transformProperties.angularVelocity.y) >= epsilonAngularVelocity;
        updateZ = Mathf.Abs(rb.angularVelocity.z - transformProperties.angularVelocity.z) >= epsilonAngularVelocity;

        rb.angularVelocity = new Vector3(updateX ? transformProperties.angularVelocity.x : rb.angularVelocity.x,
            updateY ? transformProperties.angularVelocity.y : rb.angularVelocity.y,
            updateZ ? transformProperties.angularVelocity.z : rb.angularVelocity.z);
    }
}