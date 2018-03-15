using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSFX : MonoBehaviour
{
    public AudioClip[] mAudioClips;

    private GameObject mLocalPlayer;
    private AudioSource mAudioSource;

    private void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!mLocalPlayer)
            FindLocalPlayer();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (mLocalPlayer && collision.transform.tag != "Player")
        {
            PlaySFX();

            if (GetComponent<Rigidbody>().velocity == Vector3.zero)
                mLocalPlayer = null;
        }
    }

    public void PlaySFX()
    {
        mAudioSource.clip = mAudioClips[Random.Range(0, mAudioClips.Length)];
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
