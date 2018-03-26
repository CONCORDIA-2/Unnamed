using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSFX : MonoBehaviour
{
    public AudioClip[] mAudioClips;
    private GameObject mLocalPlayer;
    private AudioSource[] mAudioSources;

    private void Start()
    {
        mAudioSources = GetComponents<AudioSource>();
    }

    private void Update()
    {
        if (!mLocalPlayer)
            FindLocalPlayer();
    }

    public void PlaySFX()
    {
        mAudioSources[0].clip = mAudioClips[Random.Range(0, mAudioClips.Length)];
        PlayerAudio playerAudio = mLocalPlayer.GetComponent<PlayerAudio>();
        playerAudio.CmdPlayClipFromSource(gameObject);
    }

    public void FindLocalPlayer()
    {
        GameObject localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
        LocalPlayerManager localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();
        mLocalPlayer = localPlayerManagerScript.GetLocalPlayerObject();
    }
}
