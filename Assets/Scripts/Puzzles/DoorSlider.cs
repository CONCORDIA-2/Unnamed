using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlider : MonoBehaviour {

    public GameObject pressurePlate, door;
    private PressurePlate pressurePlateScript;
    public Transform raised, lowered;
    public float timeToMove = 1.0f;
  
    public bool triggered = false, used = false, moving = false;
    private bool pastState = false;

    // Use this for initialization
    void Start () {
        pressurePlateScript = pressurePlate.GetComponent<PressurePlate>();
    }
	
	// Update is called once per frame
	void Update () {
        triggered = pressurePlateScript.pressed;
        
        if (pastState != triggered)
        {
            pastState = triggered;

            if (triggered && !moving)
            {
                moving = true;
                StartCoroutine(MoveToPosition(door.transform, raised.position, timeToMove));
            }
            else if (!triggered && !moving)
            {
                moving = true;
                StartCoroutine(MoveToPosition(door.transform, lowered.position, timeToMove));
            }
        }
    }

    public IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        GetComponent<AudioSource>().Play();
        var currentPos = transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
        moving = false;
    }

}
