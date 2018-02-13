using UnityEngine;

// Author: Tri-Luong Steven Dien
public class Player_PickUpDropItem : MonoBehaviour
{
    [Header("Pick Up / Drop")]
    public Transform mCharacterHands;

    private bool mIsHoldingObject = false;

    private GameObject mObjectInHands;
    private GameObject mObjectInRange;
	
	private void Update ()
    {
        // Pick up the nearby item
        if (mObjectInRange && Input.GetKeyDown(KeyCode.E))
            PickupObject();

        // Drop down the item in hands
        if (mObjectInHands && Input.GetKeyDown(KeyCode.R))
            DropDownObject();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickable")
            mObjectInRange = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickable")
            mObjectInRange = null;
    }

    // Pick up a nearby object
    private void PickupObject()
    {
        mObjectInHands = mObjectInRange;
        mObjectInHands.GetComponent<Rigidbody>().isKinematic = true;
        mObjectInHands.transform.parent = mCharacterHands;
        mObjectInHands.transform.localPosition = Vector3.zero;
        SetIsHoldingObject(true);
    }

    // Drop down the object in hands
    private void DropDownObject()
    {
        mObjectInHands.transform.parent = null;
        mObjectInHands.GetComponent<Rigidbody>().isKinematic = false;
        SetIsHoldingObject(false);
    }

    // Get the value of mIsHoldingObject
    public bool GetIsHoldingObject()
    {
        return mIsHoldingObject;
    }

    // Set the value of mIsHoldingObject
    public void SetIsHoldingObject(bool isHoldingObject)
    {
        mIsHoldingObject = isHoldingObject;
    }
}
