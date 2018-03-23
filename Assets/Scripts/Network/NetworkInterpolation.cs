using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct Snapshot
{
    public GameObject obj;
    public Vector3 position;
    // public Vector3 velocity;
    public Quaternion rotation;
}

public class NetworkInterpolation : NetworkBehaviour
{
    [SerializeField] private List<Snapshot> snaps = new List<Snapshot>();
    [SerializeField] private double bufferTime = 0.3f;
    [SerializeField] private double bufferTimer = 0.0f;
    [SerializeField] private double currentTime = 0.0f;
    [SerializeField] private double previousTime = 0.0f;

    private void Update()
    {
        currentTime = Network.time;
        double deltaTime = currentTime - previousTime;
        previousTime = currentTime;

        bufferTimer += deltaTime;
        if (bufferTimer >= bufferTime)
        {

        }
        InterpolateSnapshot();
    }

    public void InterpolateSnapshot()
    {

    }

    public void UpdateSnapshots()
    {
        foreach (Snapshot snap in snaps)
        {

        }
    }

    [Command]
    public void CmdBufferSnapshot(Snapshot snap)
    {
        RpcBufferSnapshot(snap);
    }

    [ClientRpc]
    public void RpcBufferSnapshot(Snapshot snap)
    {
        snaps.Add(snap);
    }
}
