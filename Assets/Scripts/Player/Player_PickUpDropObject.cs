using UnityEngine;
using UnityEngine.Networking;

// Author: Tri-Luong Steven Dien
public class Player_PickUpDropObject : NetworkBehaviour
{
    public Transform mCharacterHands;

    private bool mIsHoldingObject = false;

    private GameObject mObjectInHands;
    private GameObject mObjectInRange;

    private Player_Movement mPlayerMovement;

    private float mExtraWeight;

    public override void OnStartLocalPlayer()
    {
        mPlayerMovement = GetComponent<Player_Movement>();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            // Pick up / drop nearby item
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown("joystick button 1"))
            {
                if (mObjectInHands)
                    DropDownObject();

                else if (mObjectInRange && mPlayerMovement.CheckIfGrounded())
                    PickupObject();
            }

            if (mObjectInHands)
            {
                mObjectInHands.transform.localPosition = mCharacterHands.localPosition;
                mObjectInHands.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            CheckIfNearItem();
        }
    }

    private void CheckIfNearItem()
    {
        RaycastHit rayItemHit;

        if (Physics.Raycast(transform.position - new Vector3(0.0f, 0.4f, 0.0f), transform.TransformDirection(Vector3.forward), out rayItemHit, 1f))
            if (rayItemHit.transform.gameObject.tag == "Pickable")
            {
                mObjectInRange = rayItemHit.transform.gameObject;
                return;
            }

        mObjectInRange = null;
    }

    // Pick up a nearby object
    private void PickupObject()
    {
        // Put the object in range into the player hands
        mObjectInHands = mObjectInRange;
        mObjectInHands.transform.parent = transform;

        // Add the weight of the object to the total weight of the player
        mExtraWeight = mObjectInHands.GetComponent<Rigidbody>().mass;
        GetComponent<Rigidbody>().mass += mExtraWeight;

        // Get the bound size of the object and the player
        Vector3 objectSize = mObjectInHands.GetComponent<Collider>().bounds.size;
        Vector3 playerSize = GetComponent<Collider>().bounds.size;

        // Reposition the player hands (location) and rotate the object
        mCharacterHands.localPosition = new Vector3(0.0f, objectSize.z / 2.0f, playerSize.z / 2.0f + objectSize.z / 2.0f);
        mObjectInHands.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        // Disable the use of gravity of the object, remove the velocity on it and add constraints to it
        mObjectInHands.GetComponent<Rigidbody>().useGravity = false;
        mObjectInHands.GetComponent<Rigidbody>().velocity = Vector3.zero;
        mObjectInHands.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        SetIsHoldingObject(true);
    }

    // Drop down the object in hands
    private void DropDownObject()
    {
        // Re-Enable the use of gravity on the object and remove all constraints
        mObjectInHands.GetComponent<Rigidbody>().useGravity = true;
        mObjectInHands.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        // Remove the object weight from the player total weight
        GetComponent<Rigidbody>().mass -= mExtraWeight;

        // Unparent the object from the player
        mObjectInHands.transform.parent = null;
        mObjectInHands = null;
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
