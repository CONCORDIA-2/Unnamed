using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoorLogic : MonoBehaviour {

    public bool f1, f2, f3, f4;  //The status of the 4 fetching quests of level 1's maze
    private bool complete = false;
    public GameObject door;
    public HingeJoint doorHinge;
    public JointLimits closed, open;

	// Use this for initialization
	void Start () {
        door = this.gameObject;
        doorHinge = door.GetComponent<HingeJoint>();
        closed = doorHinge.limits;
        open = doorHinge.limits;
        open.max += 120;
    }
	
	// Update is called once per frame
	void Update () {
		if (!complete && f1 && f2 && f3 && f4)
        {
            complete = true;
            doorHinge.limits = open;
            doorHinge.useSpring = true;
        }
	}
}
