using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizControlScript : MonoBehaviour
{

    public GameObject pointerL, pointerR, pointerM;
    public int questionCounter = 0;
    GameObject[] questions = new GameObject[3];


    // Use this for initialization
    void Start()
    {
        questions[0] = this.transform.GetChild(0).gameObject;
        questions[1] = this.transform.GetChild(1).gameObject;
        questions[2] = this.transform.GetChild(2).gameObject;
        setPointers();
        pointerM.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal < -0.9)
        {
            pointerM.SetActive(false);
            pointerL.SetActive(true);
            pointerR.SetActive(false);
        }
        if (horizontal > 0.9)
        {
            pointerM.SetActive(false);
            pointerL.SetActive(false);
            pointerR.SetActive(true);
        }
        if (Input.GetKeyDown("joystick button 0") && (pointerL.activeInHierarchy || pointerR.activeInHierarchy))
        {
            if (pointerR.active)
            {
                //psych profile stuff
            }
            else
            {
                //psych profile stuff
            }
            questions[questionCounter].SetActive(false);
            questions[++questionCounter].SetActive(true);       //Need to load in next scene after Q3
            setPointers();
            horizontal = 0;
            pointerM.SetActive(true);
        }

    }

    void setPointers()
    {
        GameObject answers = questions[questionCounter].transform.Find("Answers").gameObject;
        pointerL = answers.transform.Find("Yes").gameObject.transform.Find("Pointer").gameObject;
        pointerR = answers.transform.Find("No").gameObject.transform.Find("Pointer").gameObject;
    }
}
