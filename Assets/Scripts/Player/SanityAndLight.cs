using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityAndLight : MonoBehaviour {

	private bool beganRoutine = false;

    public GameObject otherPlayer;
    [SerializeField] private GameObject localPlayerManager;
    [SerializeField] private LocalPlayerManager localPlayerManagerScript;
    public GameObject light;
    public GameObject aura;

    public static readonly float safeRadius = 12.0f;
    public static readonly float safeLightingRadius = 4.0f;
    public static readonly float maxTimeSeparate = 20.0f;	//seconds

    public float sanityLevel;
    public float sanityRate = 0.1f;
    public float distance;
    
    public float auraSize = 0.0f;
    public float lightBrightness = 0.0f;
    public float maxAuraSize = 3.0f;
    public float maxLightBrightness = 1.0f;
    public float maxMass = 90.0f;
    public float minMass = 25.0f;
    
    public bool specialLighting = false;
    public bool opposedLighting = false;
    public bool isRaven = true;

    public float auraChange = 0.03f;
    public float lightChange = 0.01f;
    public float massChange = 0.5f;
    private static float updateWait = 0.01f;

    void Start() {
		//grab reference to the local player manager if not assigned
	    localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
	    if (localPlayerManager)
	        localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();

		otherPlayer = localPlayerManagerScript.GetOtherPlayerObject();

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

            //retrieve the distance between players
            distance = Vector3.Distance(transform.position, otherPlayer.transform.position);

            //if player is in special lighting and are too af from their partner, decrease sanity and increase mass
            if (specialLighting && distance > safeLightingRadius)
            {
                DecreaseSanity();
                if (GetComponent<Rigidbody>().mass < maxMass)
                    GetComponent<Rigidbody>().mass += massChange;
            }
            else
            {
            	//else, put mass back to normal
                if (GetComponent<Rigidbody>().mass > minMass)
                    GetComponent<Rigidbody>().mass -= massChange * 2;

                //decrease sanity if too far from partner / increase sanity if close to partner
                if (distance < safeRadius)
                	IncreaseSanity();
            	else DecreaseSanity();
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


        }
    }

    void DecreaseSanity()
    {
        sanityLevel -= sanityRate;
        if (sanityLevel < 0)
            sanityLevel = 0;
    }

    void IncreaseSanity()
    {
        sanityLevel += sanityRate * 2;
        if (sanityLevel > 100)
            sanityLevel = 100;
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
}
