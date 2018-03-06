using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class SanityAndLight : MonoBehaviour {

	private bool beganRoutine = false;

    public GameObject otherPlayer;
    private GameObject localPlayer;
    [SerializeField] private GameObject localPlayerManager;
    [SerializeField] private LocalPlayerManager localPlayerManagerScript;
    public GameObject light;
    public GameObject aura;
    public PostProcessingProfile postProcessing;

    public static readonly float safeRadius = 12.0f;
    public static readonly float safeLightingRadius = 4.0f;

    public float sanityLevel = 100.0f;
    public float distance;
    
    public float auraSize = 0.0f;
    public float lightBrightness = 0.0f;
    public float maxAuraSize = 3.0f;
    public float maxLightBrightness = 1.0f;
    public float maxMass = 75.0f;
    public float minMass = 25.0f;
    
    public bool specialLighting = false;
    public bool opposedLighting = false;
    public bool isRaven = true;
    public bool isIncapacitated = false;

    public float sanityChange = 0.5f;
    public float auraChange = 0.03f;
    public float lightChange = 0.01f;
    public float massChange = 0.7f;
    private static float updateWait = 0.01f;

    void Start() {
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
		aura.SetActive(false);
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
	            	GetComponent<Rigidbody>().mass = maxMass * 2;
	            	sanityLevel = 0;

	            	if (distance < safeLightingRadius)
	            	{
	            		isIncapacitated = false;
                        CritterController.playerIsDown = false;
	            	}
	            }
	            else
	            {
	            	//if player is in special lighting and are too far from their partner, decrease sanity and increase mass
		            if (specialLighting)
		            {
		            	if (distance > safeLightingRadius)
		            	{
		            		DecreaseSanity(sanityChange * 3);
		               		IncreaseWeight();
		            	}
		            	else
		            	{
		            		//else, put mass and sanity back to normal
		            		IncreaseSanity(sanityChange * 6);
			                DecreaseWeight();
		            	}
		            }
		            else
		            {
		            	//decrease sanity if too far from partner / increase sanity if close to partner
			            if (distance < safeRadius)
			            {
			                IncreaseSanity(sanityChange * 6);
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
	            		if (light.GetComponent<Light>().intensity < maxLightBrightness)
	            			light.GetComponent<Light>().intensity += lightChange;
	            	}
	            	else
	            	{
	            		aura.SetActive(true);
	            		if (aura.transform.localScale.x < maxAuraSize)
	            			aura.transform.localScale += new Vector3(auraChange, auraChange, auraChange);
	            	}
	            }
	            else
	            {
	            	if (isRaven)
	            	{
	            		if (light.GetComponent<Light>().intensity > 0.0f)
	            			light.GetComponent<Light>().intensity -= lightChange;
	            		else
	            			light.SetActive(false);
	            	}
	            	else
	            	{
	            		if (aura.transform.localScale.x > 0.0f)
	            			aura.transform.localScale -= new Vector3(auraChange, auraChange, auraChange);
	            		else
	            			aura.SetActive(false);
	            	}
	            }

	            //set postprocessing to reflect sanity levels
	            ChromaticAberrationModel.Settings chroma = postProcessing.chromaticAberration.settings;
	            chroma.intensity = map(sanityLevel, 100.0f, 0.0f, 0.0f, 1.0f);
	            postProcessing.chromaticAberration.settings = chroma;

	            VignetteModel.Settings vignette = postProcessing.vignette.settings;
	            vignette.intensity = map(sanityLevel, 100.0f, 0.0f, 0.35f, 0.5f);
	            postProcessing.vignette.settings = vignette;

	            DepthOfFieldModel.Settings depth = postProcessing.depthOfField.settings;
	            depth.focusDistance = map(sanityLevel, 100.0f, 0.0f, 7.0f, 3.0f);
	            postProcessing.depthOfField.settings = depth;
	        }
        }
    }

    void DecreaseSanity(float change)
    {
        sanityLevel -= change;
        if (sanityLevel < 0.0f)
            sanityLevel = 0.0f;
    }

    void IncreaseSanity(float change)
    {
        sanityLevel += change * 2.0f;
        if (sanityLevel > 100.0f)
            sanityLevel = 100.0f;
    }

    void DecreaseWeight()
    {
    	if (GetComponent<Rigidbody>().mass > minMass)
		    GetComponent<Rigidbody>().mass -= massChange * 2.0f;
    }

    void IncreaseWeight()
    {
    	if (GetComponent<Rigidbody>().mass < maxMass)
	        GetComponent<Rigidbody>().mass += massChange;
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
				opposedLighting = true;
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
    }

    float map (float val, float from1, float to1, float from2, float to2)
    {
		return (val - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
}
