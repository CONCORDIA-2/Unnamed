using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PostProcessing;

public class SanityAndLight : NetworkBehaviour {

	private bool beganRoutine = false;

    public GameObject otherPlayer;
    private GameObject localPlayer;
    [SerializeField] private GameObject localPlayerManager;
    [SerializeField] private LocalPlayerManager localPlayerManagerScript;
    public GameObject light;
    public GameObject aura;
    public PostProcessingProfile postProcessing;

    public bool isRaven = true;
    public bool isIncapacitated = false;
    public float sanityLevel = 100.0f;
    public float distance;

    private float maxAuraSize = 4.0f;
    private float maxLightBrightness = 2.0f;
    private float minMoveSpeed = 1.5f;
    private float maxMoveSpeed = 3.8f;
    private float maxMass = 50.0f;
    private float minMass = 25.0f;

    private float safeRadius = 20.0f;
    private float safeLightingRadius = 4.0f;
    
    private bool specialLighting = false;
    private bool opposedLighting = false;

    private float sanityChange = 0.5f;
    private float auraChange = 0.03f;
    private float lightChange = 0.01f;
    private float massChange = 0.7f;
    private float speedChange = 0.03f;
    private float updateWait = 0.01f;

    private AudioSource[] mAudioSources;
    private PlayerAnimation mPlayerAnimation;

    public override void OnStartLocalPlayer()
    {
        mPlayerAnimation = GetComponent<PlayerAnimation>();
        mAudioSources = GetComponents<AudioSource>();

        isRaven = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<LocalPlayerManager>().IsRaven();

        //grab reference to the local player manager if not assigned
        localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
        if (localPlayerManager)
            localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();

        otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
        localPlayer = localPlayerManagerScript.GetLocalPlayerObject();

        //get post processing
        postProcessing = localPlayerManagerScript.GetPlayerCameraObject().gameObject.GetComponent<PostProcessingBehaviour>().profile;

        //get aura effects
        light = transform.Find("Light").gameObject;
        aura = transform.Find("Aura").gameObject;

        light.SetActive(false);
        CmdToggleLight(playerControllerId, false);
        aura.SetActive(false);
        CmdToggleAura(playerControllerId, false);
    }

    void Start()
    {
        isRaven = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<LocalPlayerManager>().IsRaven();

        //grab reference to the local player manager if not assigned
        localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
        if (localPlayerManager)
            localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();

        otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
        localPlayer = localPlayerManagerScript.GetLocalPlayerObject();

        //get post processing
        postProcessing = localPlayerManagerScript.GetPlayerCameraObject().gameObject.GetComponent<PostProcessingBehaviour>().profile;

        //get aura effects
        light = transform.Find("Light").gameObject;
        aura = transform.Find("Aura").gameObject;

        light.SetActive(false);
        CmdToggleLight(playerControllerId, false);
        aura.SetActive(false);
        CmdToggleAura(playerControllerId, false);
    }

    //begin routine if both players have been found
    void Update() {
		if (otherPlayer && !beganRoutine)
		{
			StartCoroutine("SanityRoutine");
			beganRoutine = true;
		}
		else
			otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
	}

    IEnumerator SanityRoutine() {
        while (true)
        {
        	yield return new WaitForSeconds(updateWait);

        	if (this.gameObject == localPlayer)
        	{
	            //retrieve the distance between players
	            distance = Vector3.Distance(transform.position, otherPlayer.transform.position);

	             //handle incapacitation
	            if (isIncapacitated)
	            {
                    mAudioSources[2].Play();

                    GetComponent<Rigidbody>().mass = maxMass * 2;
	            	GetComponent<Player_Movement>().mMaxSpeed = minMoveSpeed;
	            	sanityLevel = 0;

                    if (GetComponent<Player_Movement>().mMaxSpeed < maxMoveSpeed && GetComponent<Player_Movement>().mMaxSpeed > 0.0f)
                    {
                        mPlayerAnimation.CmdSetBool("isIncapWalking", true);
                        mPlayerAnimation.CmdSetBool("isIdle", false);
                    }
                    else if (GetComponent<Player_Movement>().mMaxSpeed <= 0.0f)
                    {
                        mPlayerAnimation.CmdSetBool("isIncapWalking", false);
                        mPlayerAnimation.CmdSetBool("isIncapacitated", true);
                    }

	            	if (distance < safeLightingRadius)
	            	{
	            		isIncapacitated = false;
                        CmdSetOtherIsIncapacitated(playerControllerId, false);
                        CritterController.playerIsDown = false;
	            	}
	            }
	            else
	            {
                    mAudioSources[2].Stop();

                    if (GetComponent<Animator>().GetBool("isIncapWalking") || GetComponent<Animator>().GetBool("isIncapacitated"))
                    {
                        mPlayerAnimation.CmdSetBool("isIncapWalking", false);
                        mPlayerAnimation.CmdSetBool("isIncapacitated", false);
                        mPlayerAnimation.CmdSetBool("isIdle", true);
                    }

                    //if player is in special lighting and are too far from their partner, decrease sanity and increase mass
                    if (specialLighting)
		            {
		            	if (distance > safeLightingRadius)
		            	{
		            		DecreaseSanity(sanityChange * 1.5f);
		               		IncreaseWeight();
		            	}
		            	else
		            	{
		            		//else, put mass and sanity back to normal
		            		IncreaseSanity(sanityChange * 2);
			                DecreaseWeight();
		            	}
		            }
		            else
		            {
		            	//decrease sanity if too far from partner / increase sanity if close to partner
			            if (distance < safeRadius)
			            {
			                IncreaseSanity(sanityChange * 2);
			                DecreaseWeight();
			            }
			            else DecreaseSanity(sanityChange);
		            }
	            }

	            //set light / shadow aura on player depending on lighting context
	            if (opposedLighting)
	            {
	            	if (isRaven)
	            	{
	            		light.SetActive(true);
                        CmdToggleLight(playerControllerId, true);

	            		if (light.GetComponent<Light>().intensity < maxLightBrightness)
	            			light.GetComponent<Light>().intensity += lightChange;
                        CmdSetLightIntensity(playerControllerId, light.GetComponent<Light>().intensity);
	            	}
	            	else
	            	{
	            		aura.SetActive(true);
                        CmdToggleAura(playerControllerId, true);

                        if (aura.transform.localScale.x < maxAuraSize)
	            			aura.transform.localScale += new Vector3(auraChange, auraChange, auraChange);
                        CmdSetAuraSize(playerControllerId, aura.transform.localScale.x);
	            	}
	            }
	            else
	            {
	            	if (isRaven)
	            	{
                        if (light.GetComponent<Light>().intensity > 0.0f)
                        {
                            light.GetComponent<Light>().intensity -= lightChange;
                            CmdSetLightIntensity(playerControllerId, light.GetComponent<Light>().intensity);
                        }
                        else
                        {
                            light.SetActive(false);
                            CmdToggleLight(playerControllerId, false);
                        }
	            	}
	            	else
	            	{
                        if (aura.transform.localScale.x > 0.0f)
                        {
                            aura.transform.localScale -= new Vector3(auraChange, auraChange, auraChange);
                            CmdSetAuraSize(playerControllerId, aura.transform.localScale.x);
                        }
                        else
                        {
                            aura.SetActive(false);
                            CmdToggleAura(playerControllerId, false);
                        }
	            	}
	            }

	            //set postprocessing to reflect sanity levels
	            ChromaticAberrationModel.Settings chroma = postProcessing.chromaticAberration.settings;
	            chroma.intensity = map(sanityLevel, 100.0f, 0.0f, 0.0f, 2.0f);
	            postProcessing.chromaticAberration.settings = chroma;

	            VignetteModel.Settings vignette = postProcessing.vignette.settings;
	            vignette.intensity = map(sanityLevel, 100.0f, 0.0f, 0.35f, 0.5f);
	            postProcessing.vignette.settings = vignette;

	            DepthOfFieldModel.Settings depth = postProcessing.depthOfField.settings;
	            depth.focusDistance = map(sanityLevel, 100.0f, 0.0f, 7.0f, 2.0f);
	            postProcessing.depthOfField.settings = depth;
	        }
        }
    }

    void DecreaseSanity(float change)
    {
        sanityLevel -= change;
        if (sanityLevel < 0.0f)
            sanityLevel = 0.0f;
        CmdSetOtherSanityLevel(playerControllerId, sanityLevel);
    }

    void IncreaseSanity(float change)
    {
        sanityLevel += change * 2.0f;
        if (sanityLevel > 100.0f)
            sanityLevel = 100.0f;
        CmdSetOtherSanityLevel(playerControllerId, sanityLevel);
    }

    void DecreaseWeight()
    {
    	if (GetComponent<Rigidbody>().mass > minMass)
		    GetComponent<Rigidbody>().mass -= massChange * 2.0f;

		if (GetComponent<Player_Movement>().mMaxSpeed < maxMoveSpeed)
			GetComponent<Player_Movement>().mMaxSpeed += speedChange;
    }

    void IncreaseWeight()
    {
    	if (GetComponent<Rigidbody>().mass < maxMass)
	        GetComponent<Rigidbody>().mass += massChange;

	    if (GetComponent<Player_Movement>().mMaxSpeed > minMoveSpeed)
			GetComponent<Player_Movement>().mMaxSpeed -= speedChange;
    }

    //on collision stay with collider specific to client, toggle specialLighting flag and appropriate aura response
    void OnTriggerStay(Collider collision)
    {
        if (isRaven)
        {
            if (collision.gameObject.tag == "LitArea")
                specialLighting = true;
            else if (collision.gameObject.tag == "DarkArea")
                opposedLighting = true;
        }
        else
        {
            if (collision.gameObject.tag == "DarkArea")
                specialLighting = true;
            else if (collision.gameObject.tag == "LitArea")
            {
                opposedLighting = true;
            }
        }
        if (isLocalPlayer)
        {
            CmdSetOtherOpposedLighting(playerControllerId, opposedLighting);
            CmdSetOtherSpecialLighting(playerControllerId, specialLighting);
        }
    }

    //on collision exit with collider specific to client, toggle specialLighting flag and appropriate aura response
    void OnTriggerExit(Collider collision)
    {
        if (isRaven)
        {
            if (collision.gameObject.tag == "LitArea")
                specialLighting = false;
            else if (collision.gameObject.tag == "DarkArea")
                opposedLighting = false;
        }
        else
        {
            if (collision.gameObject.tag == "DarkArea")
                specialLighting = false;
            else if (collision.gameObject.tag == "LitArea")
                opposedLighting = false;
        }
        if (isLocalPlayer)
        {
            CmdSetOtherOpposedLighting(playerControllerId, opposedLighting);
            CmdSetOtherSpecialLighting(playerControllerId, specialLighting);
        }
    }

    float map (float val, float from1, float to1, float from2, float to2)
    {
		return (val - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

    [Command]
    public void CmdSetOtherIsIncapacitated(int controllerId, bool toggle)
    {
        RpcSetOtherIsIncapacitated(controllerId, toggle);
    }

    [Command]
    public void CmdSetOtherSanityLevel(int controllerId, float level)
    {
        RpcSetOtherSanityLevel(controllerId, level);
    }

    [ClientRpc]
    public void RpcSetOtherIsIncapacitated(int controllerId, bool toggle)
    {
        if (controllerId != playerControllerId && localPlayerManagerScript != null)
        {
            localPlayerManagerScript.SetOtherIsIncapacitated(toggle);

            GameObject otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
            if (otherPlayer)
            {
                otherPlayer.GetComponent<SanityAndLight>().isIncapacitated = toggle;
            }
        }
    }

    [ClientRpc]
    public void RpcSetOtherSanityLevel(int controllerId, float level)
    {
        if (controllerId != playerControllerId && localPlayerManagerScript!= null)
        {
            localPlayerManagerScript.SetOtherSanityLevel(level);

            GameObject otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
            if (otherPlayer)
            {
                otherPlayer.GetComponent<SanityAndLight>().sanityLevel = level;
            }
        }
    }

    [Command]
    public void CmdToggleLight(int controllerId, bool toggle)
    {
        RpcToggleLight(controllerId, toggle);
    }

    [ClientRpc]
    public void RpcToggleLight(int controllerId, bool toggle)
    {
        Debug.Log("toggling light");
        if (controllerId != playerControllerId && localPlayerManagerScript != null)
        {
            GameObject otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
            if (otherPlayer)
            {
                SanityAndLight sal = otherPlayer.GetComponent<SanityAndLight>();
                sal.light.SetActive(toggle);
            }
        }
    }

    [Command]
    public void CmdToggleAura(int controllerId, bool toggle)
    {
        RpcToggleAura(controllerId, toggle);
    }

    [ClientRpc]
    public void RpcToggleAura(int controllerId, bool toggle)
    {
        Debug.Log("toggling aura");
        if (controllerId != playerControllerId && localPlayerManagerScript != null)
        {
            GameObject otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
            if (otherPlayer)
            {
                SanityAndLight sal = otherPlayer.GetComponent<SanityAndLight>();
                sal.light.SetActive(toggle);
            }
        }
    }

    [Command]
    public void CmdSetOtherOpposedLighting(int controllerId, bool val)
    {
        RpcSetOtherOpposedLighting(controllerId, val);
    }

    [ClientRpc]
    public void RpcSetOtherOpposedLighting(int controllerId, bool val)
    {
        if (controllerId != playerControllerId && localPlayerManagerScript != null)
        {
            GameObject otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
            if (otherPlayer)
            {
                SanityAndLight sal = otherPlayer.GetComponent<SanityAndLight>();
                sal.opposedLighting = val;
            }
        }
    }

    [Command]
    public void CmdSetOtherSpecialLighting(int controllerId, bool val)
    {
        RpcSetOtherSpecialLighting(controllerId, val);
    }

    [ClientRpc]
    public void RpcSetOtherSpecialLighting(int controllerId, bool val)
    {
        if (controllerId != playerControllerId && localPlayerManagerScript != null)
        {
            GameObject otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
            if (otherPlayer)
            {
                SanityAndLight sal = otherPlayer.GetComponent<SanityAndLight>();
                sal.specialLighting = val;
            }
        }
    }

    [Command]
    public void CmdSetLightIntensity(int controllerId, float val)
    {
        RpcSetLightIntensity(controllerId, val);
    }

    [ClientRpc]
    public void RpcSetLightIntensity(int controllerId, float val)
    {
        Debug.Log("setting light intensity");
        if (controllerId != playerControllerId && localPlayerManagerScript != null)
        {
            GameObject otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
            if (otherPlayer)
            {
                SanityAndLight sal = otherPlayer.GetComponent<SanityAndLight>();
                sal.light.GetComponent<Light>().intensity = val;
            }
        }
    }

    [Command]
    public void CmdSetAuraSize(int controllerId, float val)
    {
        RpcSetAuraSize(controllerId, val);
    }

    [ClientRpc]
    public void RpcSetAuraSize(int controllerId, float val)
    {
        Debug.Log("setting aura size");
        if (controllerId != playerControllerId && localPlayerManagerScript != null)
        {
            GameObject otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();
            if (otherPlayer)
            {
                SanityAndLight sal = otherPlayer.GetComponent<SanityAndLight>();
                sal.aura.transform.localScale = new Vector3(val, val, val);
            }
        }
    }
}
