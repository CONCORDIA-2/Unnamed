using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockDoorLogic : MonoBehaviour {

    private bool complete = false;
    private GameObject door;
    private HingeJoint doorHinge;
    private JointLimits open;
    public Text instructions, resolution;

	// Use this for initialization
	void Start () {
        door = this.gameObject;
        doorHinge = door.GetComponent<HingeJoint>();
        open = doorHinge.limits;
        open.min -= 120;
    }
	
	// Update is called once per frame
	void Update () {

        if (!complete && FetchQuest.succeeded)
        {
            complete = true;
            doorHinge.limits = open;
            doorHinge.useSpring = true;
            if (instructions.color.a >= 0.99)  
                 StartCoroutine(MessageFades.FadeTextToZeroAlpha(8f, instructions));
            StartCoroutine(MessageFades.FadeTextToFullAlpha(6f, resolution));
        }
	}
}
