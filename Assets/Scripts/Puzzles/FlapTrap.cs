using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlapTrap : MonoBehaviour
{

    public GameObject toDestroy1, toDestroy2, toDestroy3, toEnable;

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Flapped!");
        Destroy(toDestroy1);
        Destroy(toDestroy2);
        Destroy(toDestroy3);
        toEnable.SetActive(true);

    }
}
