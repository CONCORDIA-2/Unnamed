using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLevel : MonoBehaviour
{
    public GameObject[] mAllLevels;
    public GameObject mConcreteWall;
    private GameObject mLocalPlayer;

    private static int mCurrentLevel = -1;
    private bool mTriggeredOnce = false;

    private void Update()
    {
        if (!mLocalPlayer)
            FindLocalPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mLocalPlayer && !mTriggeredOnce)
        {
            mCurrentLevel++;

            if (mCurrentLevel >= 1 && mCurrentLevel < 4)
            {
                mAllLevels[mCurrentLevel - 1].SetActive(false);
            }

            if (mCurrentLevel == 3)
            {
                mConcreteWall.SetActive(true);
            }

            mTriggeredOnce = true;
            mAllLevels[mCurrentLevel + 1].SetActive(true);
        }
    }

    public void FindLocalPlayer()
    {
        GameObject localPlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
        LocalPlayerManager localPlayerManagerScript = localPlayerManager.GetComponent<LocalPlayerManager>();
        mLocalPlayer = localPlayerManagerScript.GetLocalPlayerObject();
    }
}
