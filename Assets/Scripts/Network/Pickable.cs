using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pickable : NetworkBehaviour
{
    [SerializeField][SyncVar] private bool isPickable = true;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private NetworkTransform ntwrkT;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ntwrkT = GetComponent<NetworkTransform>();
    }

    public bool IsPickable()
    {
        return isPickable;
    }
    
    public void SetPickable(bool toggle)
    {
        Debug.Log("setting pickable");

        // set velocity to zero to prevent flying out Team Rocket-style
        if (!toggle)
            rb.velocity = Vector3.zero;

        // set rigidbody constraints
        rb.useGravity = toggle ? true : false;
        rb.constraints = toggle ? RigidbodyConstraints.None
            : (RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationY
            | RigidbodyConstraints.FreezeRotationZ);

        // switch between sync modes when picked up (since physics are disabled)
        ntwrkT.transformSyncMode = toggle
            ? NetworkTransform.TransformSyncMode.SyncRigidbody3D
            : NetworkTransform.TransformSyncMode.SyncTransform;

        isPickable = toggle;
    }
}