using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    public GameObject pressurePlate;
    private PressurePlate pressurePlateScript;
    public bool triggered = false, used = false;  // #triggered
    public HingeJoint hinge1, hinge2;
    private JointLimits hingesActive, hingesInactive;
    private GameObject invisibleWalls;


    // Use this for initialization
    void Start()
    {
        pressurePlateScript = pressurePlate.GetComponent<PressurePlate>();
        hinge1 = transform.Find("Flap1").GetComponent<HingeJoint>();
        hinge2 = transform.Find("Flap2").GetComponent<HingeJoint>();
        hingesInactive = hinge1.limits;   //arbitrarily using hinge1
        hingesActive = hingesInactive;
        hingesActive.min = hingesInactive.min - 90;

        invisibleWalls = transform.Find("Invisible Walls").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        triggered = pressurePlateScript.pressed;
        if (triggered && !used)
        {
            GetComponent<AudioSource>().Play();
            StartCoroutine(activateDoor());
            used = true;
        }
    }

    public IEnumerator activateDoor()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "GuardCritter")
            {
                Destroy(child.GetComponent<GuardCritterController>());
                Destroy(child.GetComponent<NavMeshAgent>());
            }
        }
        Destroy(this.gameObject.GetComponent<NavMeshSurface>());

        transform.Find("Flap1").GetComponent<Rigidbody>().isKinematic = false;
        transform.Find("Flap2").GetComponent<Rigidbody>().isKinematic = false;

        hinge1.limits = hingesActive;
        hinge2.limits = hingesActive;

        yield return new WaitForSeconds(1);

        hinge1.limits = hingesInactive;
        hinge2.limits = hingesInactive;
        invisibleWalls.SetActive(false);

        yield return null;
    }


}
