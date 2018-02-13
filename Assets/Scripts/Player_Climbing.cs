using UnityEngine;

// Author: Tri-Luong Steven Dien
public class Player_Climbing : MonoBehaviour
{
    [Header("Climb")]
    public Transform mClimbLandingSpot;

    private bool mAllowToClimb = false;
    private bool mIsHanging = false;

    private Rigidbody mRb;
    private Player_Movement mPlayerMovement;
    private Player_PickUpDropItem mPlayerPickUpDropItem;

    private void Start ()
    {
        mRb = GetComponent<Rigidbody>();
        mPlayerMovement = GetComponent<Player_Movement>();
        mPlayerPickUpDropItem = GetComponent<Player_PickUpDropItem>();
    }

    private void Update ()
    {
        // If the player is allow to climb, on a button press, the player will teleport to the top of the wall
        if (mAllowToClimb && Input.GetKeyDown(KeyCode.Q))
            ClimbUp();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Climbable")
        {
            mAllowToClimb = true;

            // If the player was jumping and is not holding an item, the player will then hang onto the ledge of the wall
            if (mPlayerMovement.GetWasJumping() && !mPlayerPickUpDropItem.GetIsHoldingObject())
            {
                FreezePosY();
                SetIsHanging(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Climbable")
        {
            mAllowToClimb = false;
        }
    }

    // Teleport the player to the mClimbindLandingSpot transform position
    private void ClimbUp()
    {
        transform.position = mClimbLandingSpot.position;
        UnfreezePosY();
        mPlayerMovement.SetWasJumping(false);
        SetIsHanging(false);
    }

    // Freeze the rigidbody position (x, y, z) and rotation (x, z)
    private void FreezePosY()
    {
        mRb.constraints = RigidbodyConstraints.FreezePositionX
            | RigidbodyConstraints.FreezePositionY
            | RigidbodyConstraints.FreezePositionZ
            | RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ;
    }

    // Unfreeze the rigidbody but keep frozen the rotation on the x and z axis
    private void UnfreezePosY()
    {
        mRb.constraints = RigidbodyConstraints.None;
        mRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Get the value of mIsHanging
    public bool GetIsHanging()
    {
        return mIsHanging;
    }

    // Set the value of mIsHanging
    public void SetIsHanging(bool isHanging)
    {
        mIsHanging = isHanging;
    }
}
