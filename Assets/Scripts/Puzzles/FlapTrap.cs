using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlapTrap : MonoBehaviour
{

    public bool rabbitIn = false, ravenIn = false;

    public GameObject toDestroy1, toDestroy2, toDestroy3, toEnable;

    public void Update()
    {
        if (ravenIn && rabbitIn)
        {
            Destroy(toDestroy1);
            Destroy(toDestroy2);
            Destroy(toDestroy3);
            toEnable.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<LocalPlayerSetup>().IsRaven())
            ravenIn = true;
        else if (collision.gameObject.tag == "Player" && !collision.gameObject.GetComponent<LocalPlayerSetup>().IsRaven())
            rabbitIn = true;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<LocalPlayerSetup>().IsRaven())
            ravenIn = false;
        else if (collision.gameObject.tag == "Player" && !collision.gameObject.GetComponent<LocalPlayerSetup>().IsRaven())
            rabbitIn = false;
    }
}
