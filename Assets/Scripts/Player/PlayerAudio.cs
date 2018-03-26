using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : NetworkBehaviour
{
    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

    private Coroutine waitCoroutine = null;

    public override void OnStartLocalPlayer()
    {
        if (audioSources.Length == 0)
            audioSources = GetComponents<AudioSource>();
    }

    private void Update()
    {
        if (audioSources.Length == 0)
            audioSources = GetComponents<AudioSource>();
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
        if (audioSources[0].isPlaying)
        {
            if (interrupt)
            {
                audioSources[0].Stop();
                if (waitCoroutine != null)
                    StopCoroutine(waitCoroutine);
            }
            else
                waitCoroutine = StartCoroutine(WaitUntilEndOfClip(audioSources[0].clip));
        }

        // switch clips and play audio
        if (oneShot)
        {
            // swap clips
            AudioClip temp = audioSources[0].clip;
            audioSources[0].clip = audioClips[id];

            // play and wait until the clip is done playing
            audioSources[0].PlayOneShot(audioClips[id]);
            waitCoroutine = StartCoroutine(WaitUntilEndOfClip(audioSources[0].clip));

            // restore old clip
            audioSources[0].clip = temp;
        }
        else
        {
            audioSources[0].clip = audioClips[id];
            audioSources[0].Play();
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
            if (audioSources[0].isPlaying)
            {
                if (interrupt)
                {
                    audioSources[0].Stop();
                    if (waitCoroutine != null)
                        StopCoroutine(waitCoroutine);
                }
                else
                    waitCoroutine = StartCoroutine(WaitUntilEndOfClip(audioSources[0].clip));
            }

            // switch clips and play audio
            if (oneShot)
            {
                // swap clips
                AudioClip temp = audioSources[0].clip;
                audioSources[0].clip = clip;

                // play and wait until the clip is done playing
                audioSources[0].PlayOneShot(clip);
                waitCoroutine = StartCoroutine(WaitUntilEndOfClip(audioSources[0].clip));

                // restore old clip
                audioSources[0].clip = temp;
            }
            else
            {
                audioSources[0].clip = clip;
                audioSources[0].Play();
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
        yield return new WaitUntil(() => audioSources[0].isPlaying == false);
    }
}
