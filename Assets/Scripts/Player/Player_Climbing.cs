using UnityEngine;
using UnityEngine.Networking;

// Author: Tri-Luong Steven Dien
public class Player_Climbing : NetworkBehaviour
{
    [Header("Climb")]
    public Transform mClimbLandingSpot;

    private bool mIsHanging = false;
    private float mMaxDistanceToWall = 0.5f;
    private float mMaxTimer = 0.5f;
    private float mHangingTimer;

    private Rigidbody mRb;
    private Player_Movement mPlayerMovement;
    private Player_PickUpDropObject mPlayerPickUpDropItem;

    public override void OnStartLocalPlayer()
    {
        mHangingTimer = mMaxTimer;
        mRb = GetComponent<Rigidbody>();
        mPlayerMovement = GetComponent<Player_Movement>();
        mPlayerPickUpDropItem = GetComponent<Player_PickUpDropObject>();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (mIsHanging)
                mHangingTimer -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown("joystick button 1"))
            {
                if (mIsHanging)
                    LetGo();
            }

            // If the player is allow to climb, on stick move up/down/left/right depending on the player orientation, the player will teleport to the top of the wall
            if (mIsHanging && transform.localRotation.eulerAngles.y >= 89.0f && transform.localRotation.eulerAngles.y <= 91.0f) // 90 degrees - Stick -> Right
                if (Input.GetAxis("Horizontal") > 0.9f && mHangingTimer <= 0.0f)
                {
                    ClimbUp();
                }
            if (mIsHanging && transform.localRotation.eulerAngles.y >= 269.0f && transform.localRotation.eulerAngles.y <= 271.0f) // 270 degrees - Stick -> Left
                if (Input.GetAxis("Horizontal") < -0.9f && mHangingTimer <= 0.0f)
                {
                    ClimbUp();
                }
            if (mIsHanging &&
                (transform.localRotation.eulerAngles.y >= -1.0f && transform.localRotation.eulerAngles.y <= 1.0f) ||
                (transform.localRotation.eulerAngles.y >= 359.0f && transform.localRotation.eulerAngles.y <= -361.0f)) // 0 degrees or 360 degrees - Stick -> Up
                if (Input.GetAxis("Vertical") > 0.9f && mHangingTimer <= 0.0f)
                {
                    ClimbUp();
                }
            if (mIsHanging && transform.localRotation.eulerAngles.y >= 179.0f && transform.localRotation.eulerAngles.y <= 181.0f) // 180 degrees - Stick -> Down
                if (Input.GetAxis("Vertical") < -0.9f && mHangingTimer <= 0.0f)
                {
                    ClimbUp();
                }
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
            CheckIfNearLedge();
    }

    // Check with two raycasts if the player is close to a ledge
    private void CheckIfNearLedge()
    {
        RaycastHit rayTopHit;
        RaycastHit rayBotHit;
        RaycastHit rayCenterHit;

        if (Physics.Raycast(transform.position + new Vector3(0.0f, 0.5f, 0.0f), transform.TransformDirection(Vector3.forward), out rayBotHit, mMaxDistanceToWall) &&
            !Physics.Raycast(transform.position + new Vector3(0.0f, 0.75f, 0.0f), transform.TransformDirection(Vector3.forward), out rayTopHit, mMaxDistanceToWall))
        {
            if (rayBotHit.transform.gameObject.tag == "Climbable")
            {
                // If the player is near ledge and not grabbing anything, the player will automatically grab the ledge of the wall
                if (!mPlayerPickUpDropItem.GetIsHoldingObject() && !mIsHanging && mPlayerMovement.GetWasJumping())
                {
                    // Rotate the player "facing direction" to directly face the wall (perpendicular with the wall)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(-rayBotHit.normal), 360.0f);

                    // Cast another ray (after rotating the player) to offset the distance between the player and the wall
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayCenterHit, mMaxDistanceToWall))
                    {
                        // Make sure that the distance between the player and a ledge is always the same.
                        float offset = mMaxDistanceToWall - rayCenterHit.distance;

                        if (transform.localRotation.eulerAngles.y >= 89.0f && transform.localRotation.eulerAngles.y <= 91.0f) // 90 degrees - Stick -> Right
                        {
                            transform.position = new Vector3(transform.position.x - offset, transform.position.y, transform.position.z);
                        }
                        if (transform.localRotation.eulerAngles.y >= 269.0f && transform.localRotation.eulerAngles.y <= 271.0f) // 270 degrees - Stick -> Left
                        {
                            transform.position = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);
                        }
                        if ((transform.localRotation.eulerAngles.y >= -1.0f && transform.localRotation.eulerAngles.y <= 1.0f) ||
                            (transform.localRotation.eulerAngles.y >= 359.0f && transform.localRotation.eulerAngles.y <= -361.0f)) // 0 degrees or 360 degrees - Stick -> Up
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - offset);
                        }
                        if (transform.localRotation.eulerAngles.y >= 179.0f && transform.localRotation.eulerAngles.y <= 181.0f) // 180 degrees - Stick -> Down
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + offset);
                        }

                        FreezePosition();
                        SetIsHanging(true);
                    }
                }
            }
        }
    }

    // Teleport the player to the mClimbindLandingSpot transform position
    private void ClimbUp()
    {
        transform.position = mClimbLandingSpot.position;
        UnfreezePosition();
        mPlayerMovement.SetWasJumping(false);
        SetIsHanging(false);
        mHangingTimer = mMaxTimer;
    }

    // Unfreeze the player's movement and cancel climb
    private void LetGo()
    {
        UnfreezePosition();
        mPlayerMovement.SetWasJumping(false);
        SetIsHanging(false);
        mHangingTimer = mMaxTimer;
    }

    // Freeze the rigidbody position (x, y, z) and rotation (x, z)
    private void FreezePosition()
    {
        mRb.constraints = RigidbodyConstraints.FreezePositionX
            | RigidbodyConstraints.FreezePositionY
            | RigidbodyConstraints.FreezePositionZ
            | RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ;
    }

    // Unfreeze the rigidbody but keep frozen the rotation on the x and z axis
    private void UnfreezePosition()
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
