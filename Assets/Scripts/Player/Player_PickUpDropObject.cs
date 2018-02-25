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
	
	private void Update ()
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

            // Force object to the front of the player
            if (mObjectInHands)
            {
                mObjectInHands.transform.localPosition = mCharacterHands.localPosition;
                mObjectInHands.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                mObjectInHands.GetComponent<Rigidbody>().MovePosition(mCharacterHands.position);
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
        mObjectInHands = mObjectInRange;
        mObjectInHands.transform.parent = transform;

        mExtraWeight = mObjectInHands.GetComponent<Rigidbody>().mass;
        GetComponent<Rigidbody>().mass += mExtraWeight;

        Vector3 objectSize = mObjectInHands.GetComponent<Collider>().bounds.size;
        Vector3 playerSize = GetComponent<Collider>().bounds.size;

        mCharacterHands.localPosition = new Vector3(0.0f, objectSize.z / 2.0f, playerSize.z / 2.0f + objectSize.z / 2.0f);
        mObjectInHands.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        mObjectInHands.GetComponent<Rigidbody>().useGravity = false;
        mObjectInHands.GetComponent<Rigidbody>().velocity = Vector3.zero;
        mObjectInHands.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        SetIsHoldingObject(true);
    }

    // Drop down the object in hands
    private void DropDownObject()
    {
        mObjectInHands.GetComponent<Rigidbody>().useGravity = true;
        mObjectInHands.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        GetComponent<Rigidbody>().mass -= mExtraWeight;

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
