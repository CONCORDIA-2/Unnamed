using UnityEngine;
using UnityEngine.Networking;

// Author: Tri-Luong Steven Dien
public class Player_PickUpDropObject : NetworkBehaviour
{
    [Header("Player Hands")]
    public Transform mCharacterHands;
    public float mThrowForce;

    // Bool variable to know if the player is holding an object
    private bool mIsHoldingObject = false;

    // The actual pickable object
    private GameObject mObjectInHands;
    private GameObject mObjectInRange;

    // The object mass
    private float mExtraWeight;

    // Other attached script
    private Player_Movement mPlayerMovement;
    private LocalPlayerSetup mLocalPlayerSetup;
    private PlayerAnimation mPlayerAnimation;

    public override void OnStartLocalPlayer()
    {
        mPlayerMovement = GetComponent<Player_Movement>();
        mLocalPlayerSetup = GetComponent<LocalPlayerSetup>();
        mPlayerAnimation = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            // Pick up / drop nearby item
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown("joystick button 2"))
            {
                if (mObjectInHands)
                    CmdDropDownObject();
                else if (mObjectInRange && mPlayerMovement.CheckIfGrounded())
                    CmdPickupObject(mObjectInRange);
            }
        }

        if (mObjectInHands)
        {
            // Keeps the object in hands at the same position and orientation
            mObjectInHands.transform.localPosition = mCharacterHands.localPosition;
            mObjectInHands.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            // Check if an pickable item is near the player
            CheckIfNearItem();
        }
    }

    private void CheckIfNearItem()
    {
        RaycastHit rayItemHit;

        // Check if an pickable object is in range
        Debug.DrawRay(transform.position - new Vector3(0.0f, 0.0f, 0.0f), transform.TransformDirection(Vector3.forward), Color.blue);
        if (Physics.Raycast(transform.position - new Vector3(0.0f, 0.0f, 0.0f), transform.TransformDirection(Vector3.forward), out rayItemHit, 1f))
            if (rayItemHit.transform.gameObject.tag == "Pickable" || (rayItemHit.transform.gameObject.tag == "HeavyPickable" && mLocalPlayerSetup.IsRaven()))
            {
                mObjectInRange = rayItemHit.transform.gameObject;
                return;
            }
            else if (rayItemHit.transform.gameObject.tag == "Heavy Pickable" && !mLocalPlayerSetup.IsRaven())
            {
                // Display feedback > Rabbit cannot pick up heavy objects
            }

        mObjectInRange = null;
    }

    // Pick up a nearby object
    [Command]
    private void CmdPickupObject(GameObject obj)
    {
        RpcPickupObject(obj);
    }

    [ClientRpc]
    public void RpcPickupObject(GameObject obj)
    {
        Pickable pickable = obj.GetComponent<Pickable>();

        // [Ben]: only pick up object if not already picked up
        if (pickable && pickable.IsPickable())
        {
            // Put the object in range into the player hands
            mObjectInHands = obj;
            mObjectInHands.transform.parent = transform;

            // [Ben]: toggle object pickable
            CmdSetPickable(pickable.gameObject, false);

            // Add the weight of the object to the total weight of the player
            mExtraWeight = mObjectInHands.GetComponent<Rigidbody>().mass;
            GetComponent<Rigidbody>().mass += mExtraWeight;

            // Get the bound size of the object and the player
            Vector3 objectSize = mObjectInHands.GetComponent<Collider>().bounds.size;
            Vector3 playerSize = GetComponent<Collider>().bounds.size;

            // Reposition the player hands (location) and rotate the object
            mCharacterHands.localPosition = new Vector3(0.0f, playerSize.y + objectSize.y / 2.0f, 0.0f);
            mObjectInHands.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            // Disable the use of gravity of the object, remove the velocity on it and add constraints to it
            mObjectInHands.GetComponent<Rigidbody>().useGravity = false;
            mObjectInHands.GetComponent<Rigidbody>().velocity = Vector3.zero;
            mObjectInHands.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            if (mObjectInHands.tag == "Pickable" || mObjectInHands.tag == "HeavyPickable")
            {
                mObjectInHands.GetComponent<ItemSFX>().PlaySFX();
            }

            mPlayerAnimation.CmdSetBool("isLifting", true);
            mPlayerAnimation.CmdSetBool("isIdle", false);

            SetIsHoldingObject(true);
        }
    }

    // Drop down the object in hands
    [Command]
    private void CmdDropDownObject()
    {
        RpcDropDownObject();
    }

    [ClientRpc]
    public void RpcDropDownObject()
    {
        // Re-Enable the use of gravity on the object and remove all constraints
        mObjectInHands.GetComponent<Rigidbody>().useGravity = true;
        mObjectInHands.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        if (mExtraWeight >= 10.0f)
            mObjectInHands.GetComponent<Rigidbody>().AddForce(transform.forward * mThrowForce * 3f, ForceMode.Impulse);
        else
            mObjectInHands.GetComponent<Rigidbody>().AddForce(transform.forward * mThrowForce, ForceMode.Impulse);

        // Remove the object weight from the player total weight
        GetComponent<Rigidbody>().mass -= mExtraWeight;

        // [Ben]: set object as pickable again
        CmdSetPickable(mObjectInHands, true);

        // Unparent the object from the player
        mObjectInHands.transform.parent = null;
        mObjectInHands = null;
        SetIsHoldingObject(false);

        mPlayerAnimation.CmdSetBool("isLifting", false);
        mPlayerAnimation.CmdSetBool("isIdle", true);
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

    [Command]
    void CmdSetPickable(GameObject obj, bool toggle)
    {
        RpcSetPickable(obj, toggle);
    }

    [ClientRpc]
    void RpcSetPickable(GameObject obj, bool toggle)
    {
        Pickable pickable = obj.GetComponent<Pickable>();

        if (pickable)
            pickable.SetPickable(toggle);
    }
}
