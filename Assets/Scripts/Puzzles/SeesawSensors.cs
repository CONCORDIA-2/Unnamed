using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeesawSensors : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter (Collider collision)
    {
        if (this.transform.name == "LeftSide" && (collision.gameObject.GetComponent<LocalPlayerSetup>().IsRaven() || collision.gameObject.tag == "HeavyPickable")) // && collision.IsRaven() for all checks
            SeeSawController.leftRaven = true;
        else if (this.transform.name == "RightSide" && (collision.gameObject.GetComponent<LocalPlayerSetup>().IsRaven() || collision.gameObject.tag == "HeavyPickable"))
            SeeSawController.rightRaven = true;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (this.transform.name == "LeftSide" && (collision.gameObject.GetComponent<LocalPlayerSetup>().IsRaven() || collision.gameObject.tag == "HeavyPickable"))
            SeeSawController.leftRaven = false;
        else if (this.transform.name == "RightSide" && (collision.gameObject.GetComponent<LocalPlayerSetup>().IsRaven() || collision.gameObject.tag == "HeavyPickable"))
            SeeSawController.rightRaven = false;
    }
}
