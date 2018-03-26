using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderMusic : MonoBehaviour
{
    public AudioClip[] mAudioClips;
    private GameObject mLocalPlayer;
    private AudioSource[] mAudioSources;

    private static int mCurrentLevel = -1;
    private bool mLevelMusicPlayed = false;

    private void Update()
    {
        if (!mLocalPlayer)
            FindLocalPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mLocalPlayer && !mLevelMusicPlayed)
        {
            mCurrentLevel++;

            if (mCurrentLevel == 4)
                mCurrentLevel = 0;

            mLevelMusicPlayed = true;
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        mAudioSources[1].clip = mAudioClips[mCurrentLevel];
        mAudioSources[1].Play();
    }

    public void FindLocalPlayer()
    {
        GameObject localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
        LocalPlayerManager localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();
        mLocalPlayer = localPlayerManagerScript.GetLocalPlayerObject();

        if(mLocalPlayer)
        {
            mAudioSources = mLocalPlayer.GetComponents<AudioSource>();
        }
    }
}
