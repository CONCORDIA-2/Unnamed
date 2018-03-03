using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeSawController : MonoBehaviour {

    public static bool leftRaven = false, rightRaven = false;
    private HingeJoint seesaw;
    private JointLimits ravenLeft, ravenRight, originalLimits;

    // Use this for initialization
    void Start () {
        seesaw = this.gameObject.GetComponent<HingeJoint>();
        originalLimits = seesaw.limits;
        ravenLeft = seesaw.limits;
        ravenRight = seesaw.limits;
        ravenRight.min = originalLimits.min - 100; ; //Increase dip on right
        ravenLeft.max = originalLimits.max + 100; //Increase dip on left
    }
	
	// Update is called once per frame
	void Update () {
        if (leftRaven)
            seesaw.limits = ravenLeft;
        else if (rightRaven)
            seesaw.limits = ravenRight;
        else       
        {
            //reset the limits
            seesaw.limits = originalLimits;
        }

    }
}
