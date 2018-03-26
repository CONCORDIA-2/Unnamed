using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : NetworkBehaviour
{
    [SerializeField] private Animator animator;

    // eventually, if we want to implement some sort of wait coroutine
    // (similar to PlayerAudio.cs)
    //private Coroutine waitCoroutine = null;

    public override void OnStartLocalPlayer()
    {
        if (!animator)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!animator)
            animator = GetComponent<Animator>();
    }

    [Command]
    public void CmdSetBool(string key, bool val)
    {
        // server-side command, to execute client rpc
        RpcSetBool(key, val);
    }

    [Command]
    public void CmdSetFloat(string key, float val)
    {
        // server-side command, to execute client rpc
        RpcSetFloat(key, val);
    }

    [Command]
    public void CmdSetInteger(string key, int val)
    {
        // server-side command, to execute client rpc
        RpcSetInteger(key, val);
    }

    [Command]
    public void CmdSetTrigger(string key)
    {
        // server-side command, to execute client rpc
        RpcSetTrigger(key);
    }

    [ClientRpc]
    public void RpcSetBool(string key, bool val)
    {
        animator.SetBool(key, val);
    }

    [ClientRpc]
    public void RpcSetFloat(string key, float val)
    {
        animator.SetFloat(key, val);
    }

    [ClientRpc]
    public void RpcSetInteger(string key, int val)
    {
        animator.SetInteger(key, val);
    }

    [ClientRpc]
    public void RpcSetTrigger(string key)
    {
        animator.SetTrigger(Animator.StringToHash(key));
    }
}
