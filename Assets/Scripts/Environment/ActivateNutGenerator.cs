using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateNutGenerator : MonoBehaviour
{
    public GameObject[] m_NutGenerators;
    private GameObject mLocalPlayer;

    private void Update()
    {
        if (!mLocalPlayer)
            FindLocalPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mLocalPlayer)
        {
            foreach (GameObject go in m_NutGenerators)
            {
                go.SetActive(true);
            }
        }
    }

    public void FindLocalPlayer()
    {
        GameObject localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
        LocalPlayerManager localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();
        mLocalPlayer = localPlayerManagerScript.GetLocalPlayerObject();
    }
}
