using UnityEngine;

// Author: Tri-Luong Steven Dien
public class Player_Climbing : MonoBehaviour
{
    [Header("Climb")]
    public Transform mClimbLandingSpot;

    private bool mNearLedge = false;
    private bool mIsHanging = false;

    private Rigidbody mRb;
    private Player_Movement mPlayerMovement;
    private Player_PickUpDropObject mPlayerPickUpDropItem;

    private void Start ()
    {
        mRb = GetComponent<Rigidbody>();
        mPlayerMovement = GetComponent<Player_Movement>();
        mPlayerPickUpDropItem = GetComponent<Player_PickUpDropObject>();
    }

    private void Update ()
    {
        // If the player is near ledge, the player can hang onto it by pressing the hang button
        if (mNearLedge && Input.GetKeyDown(KeyCode.Tab))
        {
            FreezePosY();
            SetIsHanging(true);
        }

        // If the player is allow to climb, on a button press, the player will teleport to the top of the wall
        if (mIsHanging && Input.GetKeyDown(KeyCode.Q))
            ClimbUp();
    }

    private void FixedUpdate()
    {
        CheckIfNearLedge();
    }

    // Check with two raycasts if the player is close to a ledge
    private void CheckIfNearLedge()
    {
        RaycastHit rayTopHit;
        RaycastHit rayBotHit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayBotHit, 0.75f) &&
            !Physics.Raycast(transform.position + new Vector3 (0.0f, 0.5f, 0.0f), transform.TransformDirection(Vector3.forward), out rayTopHit, 0.75f))
            if (rayBotHit.transform.gameObject.tag == "Climbable")
            {
                mNearLedge = true;
                return;
            }

        mNearLedge = false;
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
