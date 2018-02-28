using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class WaterTrapControl : MonoBehaviour {

    public GameObject waterContact;
    private bool waterOn = false;

    private GameObject invisibleWalls;

    // Use this for initialization
    void Start () {
        waterContact = transform.Find("Critter Water").gameObject;
        invisibleWalls = transform.Find("Invisible Walls").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        if (waterContact.activeInHierarchy && !waterOn)
        {
            waterOn = true;

            foreach (Transform child in transform)
            {
                if (child.tag == "GuardCritter")
                    StartCoroutine(waterDeath(child.gameObject));
            }

            StartCoroutine(removeInvisibleWalls());
        }
    }

    public IEnumerator waterDeath(GameObject agent)
    {
        yield return new WaitForSeconds(Random.Range(0, 4));
        UnityEngine.Object.Destroy(agent.gameObject);
        yield return null;
    }

    public IEnumerator removeInvisibleWalls()
    {
        yield return new WaitForSeconds(4.5f);     //all critters will have blown up at this point
        invisibleWalls.SetActive(false);
        yield return null;
    }
}
