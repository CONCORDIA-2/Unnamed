using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSFX : MonoBehaviour
{
    public AudioClip[] mAudioClips;

    private GameObject mParentPlayer;
    private AudioSource mAudioSource;

    private void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (mParentPlayer && collision.transform.tag != "Player" )
        {
            mAudioSource.clip = mAudioClips[Random.Range(0, mAudioClips.Length)];
            mParentPlayer.GetComponent<PlayerAudio>().CmdPlayClipFromSource(gameObject);

            if (!mParentPlayer.GetComponent<Player_PickUpDropObject>().GetIsHoldingObject())
                mParentPlayer = null;
        }
    }

    public void SetParentPlayer(GameObject parent)
    {
        mParentPlayer = parent;
    }
}
