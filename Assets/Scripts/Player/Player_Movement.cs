using UnityEngine;
using UnityEngine.Networking;

// Author: Tri-Luong Steven Dien
public class Player_Movement : NetworkBehaviour
{
    [Header("Movement")]
    public float mMaxSpeed = 3.0f;

    [Header("Jump")]
    // Rabbit: 290.0f (old) | initial: 8.0f, extra 250.0f
    // Raven: 260.0f (old) | initial: 7.0f, extra 250.0f
    public float mInitialJumpPower;
    public float mExtraJumpPower;
    private float mMaxExtraJumpTime = 0.3f;
    private float mJumpTimer = 0.0f;
    private float mDelayToExtraJumpForce = 0.25f;
    private bool mWasJumping = false;
    private bool mIsJumping = false;

    private float mDistanceToGround;

    // Orientation
    private float mRotationDegreesPerSecond = 400f;

    // Player rigidbody component
    private Rigidbody mRb;

    // Other attached script
    private Player_Climbing mPlayerClimbing;

    public override void OnStartLocalPlayer()
    {
        mRb = GetComponent<Rigidbody>();
        mPlayerClimbing = GetComponent<Player_Climbing>();
        mDistanceToGround = GetComponent<Collider>().bounds.extents.y + 0.1f;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            // If the player is grounded, they can jump
            if ((Input.GetKeyDown("space") || Input.GetKeyDown("joystick button 0")) && CheckIfGrounded())
            {
                mIsJumping = true;
                mWasJumping = true;
                mJumpTimer = Time.time;
            }

            if ((Input.GetKeyUp("space") || Input.GetKeyUp("joystick button 0")) || Time.time - mJumpTimer > mMaxExtraJumpTime)
            {
                mIsJumping = false;
            }
        }
    }

    private void FixedUpdate()
    {
        // If the player is not hanging, they can move
        if (isLocalPlayer)
        {
            if (!mPlayerClimbing.GetIsHanging())
                Move();

            if (mIsJumping || mWasJumping)
                Jump();

            // Add additional downard force to ground player
        	mRb.AddForce(new Vector3(0.0f, -90.0f, 0.0f), ForceMode.Force);
        }
    }

    // Function that takes the joystick movement input to move the character
    private void Move()
    {
        // Gets input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Creates a vector direction from the inputs
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical);

        // If there's an input from the player
        if (horizontal != 0f || vertical != 0f)
        {
            // Normalize input
            if (direction.magnitude > 1 || direction.magnitude < -1)
            	direction = Vector3.Normalize(direction);

            // Map max speed to input intensity
            float localMaxSpeed = Remap(direction.magnitude / 1f, 0f, 1f, 0f, mMaxSpeed);

            // Rotates the character so it faces the same orientation as the vector direction
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), mRotationDegreesPerSecond * Time.deltaTime);

            // Keep Y magnitude so as to not affect jump physics with speed clamp
            float yMag = mRb.velocity.y;

            // Moves the character towards vector direction
            // Speed limited by default as a result of normalized movement vector and localMaxSpeed multiplier
            Vector2 movement = new Vector2(mRb.velocity.x, mRb.velocity.z);
            Vector2 newMove = new Vector2(direction.x * localMaxSpeed, direction.z * localMaxSpeed);
            if (newMove.magnitude > movement.magnitude)
            {
            	if (CheckIfGrounded())
           			movement = newMove;
           		else
           			//aerial control
           			movement = Vector2.Lerp(movement, newMove, 0.07f);
            }

            mRb.velocity = new Vector3(movement.x, yMag, movement.y);
        }
        else
        {
        	// Stop movement if no significant movement is present
        	if (mRb.velocity.magnitude < new Vector3(0.1f, 0.1f, 0.1f).magnitude)
    			mRb.velocity = new Vector3(0, 0, 0);
        }
    }

    // Linear map
    float Remap(float val, float min1, float max1, float min2, float max2)
    {
        return (val - min1) / (max1 - min1) * (max2 - min2) + min2;
    }

    // Function that takes the joystick button input for the character to jump
    private void Jump()
    {
        // Makes the character jump toward the Y axis
        if (mWasJumping)
        {
            mRb.AddForce(new Vector3(0.0f, mInitialJumpPower, 0.0f), ForceMode.VelocityChange);
            mWasJumping = false;
        }

        if (mIsJumping && Time.time - mJumpTimer > mDelayToExtraJumpForce)
        {
            mRb.AddForce(new Vector3(0.0f, mExtraJumpPower, 0.0f), ForceMode.Acceleration);
            mIsJumping = false;
        }
    }

    // Function that checks if the player is grounded
    public bool CheckIfGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, mDistanceToGround);
    }

    // Get the value of mWasJumping
    public bool GetWasJumping()
    {
        return mWasJumping;
    }

    // Set the value of mWasJumping
    public void SetWasJumping(bool wasJumping)
    {
        mWasJumping = wasJumping;
    }
}