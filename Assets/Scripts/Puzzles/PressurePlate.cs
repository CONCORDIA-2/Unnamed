using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{

    public GameObject plate;
    public bool heavyPlate, pressed = false, used = false;
    public Transform raised, lowered;
    public float speed = 0.1f;
    private float startTime, journeyLength;

    // Use this for initialization
    void Start()
    {
        plate = this.gameObject;
        journeyLength = Vector3.Distance(raised.position, lowered.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (pressed)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            plate.transform.position = Vector3.Lerp(raised.position, lowered.position, fracJourney);
        }
        else if (used && !pressed)      //"used" prevents this from triggering on startup, and could force single use if needed
        {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            plate.transform.position = Vector3.Lerp(lowered.position, raised.position, fracJourney);
        }
        /*
        float step = speed * Time.deltaTime;
        plate.transform.position = Vector3.MoveTowards(plate.transform.position, lowered.position, step);
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        startTime = Time.time;
        pressed = true;
        used = true;
       // startTime = Time.time;
    }

    private void OnCollisionExit(Collision collision)
    {
        startTime = Time.time;
        pressed = false;
       // startTime = Time.time;
    }
}
