using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3LockDoor : MonoBehaviour {

	public GameObject door;
    public Transform raised, lowered;
    public float timeToMove = 1.0f;
  
    public bool triggered = false, used = false, moving = false;

	void Update () {
        if (triggered && !moving)
        {
            moving = true;
            StartCoroutine(MoveToPosition(door.transform, lowered.position, timeToMove));
        }
        else if (!triggered && !moving)
        {
            moving = true;
            StartCoroutine(MoveToPosition(door.transform, raised.position, timeToMove));
        }
    }

    void OnTriggerEnter(Collider collider) {
    	if (collider.gameObject.tag == "Player") triggered = true;
    }

    void OnTriggerExit(Collider collider) {
    	if (collider.gameObject.tag == "Player") triggered = false;
    }

    public IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = door.transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            door.transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
        moving = false;
    }
}
