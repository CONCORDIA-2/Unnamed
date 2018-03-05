using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoorLogic : MonoBehaviour {

    public GameObject f1, f2, f3, f4;  //The 4 fetching quests of level 1's maze
    public FetchQuest b1, b2, b3, b4;   //Tracks the success of each quest
    private bool complete = false;
    private GameObject door;
    private HingeJoint doorHinge;
    private JointLimits open;

	// Use this for initialization
	void Start () {
        door = this.gameObject;
        doorHinge = door.GetComponent<HingeJoint>();
        open = doorHinge.limits;
        open.max += 120;

        b1 = f1.GetComponent<FetchQuest>();
        b2 = f2.GetComponent<FetchQuest>();
        b3 = f3.GetComponent<FetchQuest>();
        b4 = f4.GetComponent<FetchQuest>();
    }
	
	// Update is called once per frame
	void Update () {

        if (!complete && b1.succeeded && b2.succeeded && b3.succeeded && b4.succeeded)
        {
            complete = true;
            doorHinge.limits = open;
            doorHinge.useSpring = true;
        }
	}
}
