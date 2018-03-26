using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

    private Coroutine waitCoroutine = null;

    public override void OnStartLocalPlayer()
    {
        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
    }

    [Command]
    public void CmdPlayClipId(int id, bool interrupt , bool oneShot )
    {
        // server-side command, to execute client rpc
        RpcPlayClipId(id, interrupt, oneShot);
    }

    [Command]
    public void CmdPlayClip(string pathToClip, bool interrupt , bool oneShot )
    {
        // server-side command, to execute client rpc
        RpcPlayClip(pathToClip, interrupt, oneShot);
    }

    [Command]
    public void CmdPlayClipFromSource(GameObject source)
    {
        // server-side command, to execute client rpc
        RpcPlayClipFromSource(source);
    }

    [ClientRpc]
    public void RpcPlayClipId(int id, bool interrupt , bool oneShot )
    {
        // safety checks
        if (id < 0 || id >= audioClips.Count || audioClips[id] == null)
            return;

        //Debug.Log("received rpc, source is playing: " + audioSource.isPlaying);

        // interrupt current audio if specified, otherwise queue it
        if (audioSource.isPlaying)
        {
            if (interrupt)
            {
                audioSource.Stop();
                if (waitCoroutine != null)
                    StopCoroutine(waitCoroutine);
            }
            else
                waitCoroutine = StartCoroutine(WaitUntilEndOfClip(audioSource.clip));
        }

        // switch clips and play audio
        if (oneShot)
        {
            // swap clips
            AudioClip temp = audioSource.clip;
            audioSource.clip = audioClips[id];

            // play and wait until the clip is done playing
            audioSource.PlayOneShot(audioClips[id]);
            waitCoroutine = StartCoroutine(WaitUntilEndOfClip(audioSource.clip));

            // restore old clip
            audioSource.clip = temp;
        }
        else
        {
            audioSource.clip = audioClips[id];
            audioSource.Play();
        }
    }

    [ClientRpc]
    public void RpcPlayClip(string pathToClip, bool interrupt , bool oneShot )
    {
        // search for clip in resources folder
        AudioClip clip = Resources.Load(pathToClip) as AudioClip;
        Debug.Log("loading clip");
        if (clip != null)
        {
            Debug.Log("clip loaded");
            // add clip to existing list
            if (!audioClips.Contains(clip))
                audioClips.Add(clip);

            // interrupt current audio if specified, otherwise queue it
            if (audioSource.isPlaying)
            {
                if (interrupt)
                {
                    audioSource.Stop();
                    if (waitCoroutine != null)
                        StopCoroutine(waitCoroutine);
                }
                else
                    waitCoroutine = StartCoroutine(WaitUntilEndOfClip(audioSource.clip));
            }

            // switch clips and play audio
            if (oneShot)
            {
                // swap clips
                AudioClip temp = audioSource.clip;
                audioSource.clip = clip;

                // play and wait until the clip is done playing
                audioSource.PlayOneShot(clip);
                waitCoroutine = StartCoroutine(WaitUntilEndOfClip(audioSource.clip));

                // restore old clip
                audioSource.clip = temp;
            }
            else
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
        else
            Debug.Log("could not load clip");
    }

    [ClientRpc]
    public void RpcPlayClipFromSource(GameObject source)
    {
        source.GetComponent<AudioSource>().Play();
    }

    private IEnumerator WaitUntilEndOfClip(AudioClip clip)
    {
        yield return new WaitUntil(() => audioSource.isPlaying == false);
    }
}
