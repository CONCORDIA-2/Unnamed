using UnityEngine;
using UnityEngine.Networking;

// Author: Tri-Luong Steven Dien
public class Player_Movement : NetworkBehaviour
{
    [Header("Movement")]
    public float mMaxSpeed = 3.0f;

    [Header("Jump")]
    //rabbit: 290
    //raven: 260
    public float mJumpPower = 260.0f;

    private float mDistanceToGround;
    private bool mWasJumping = false;

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
        // If the player is grounded, they can jump
        if (isLocalPlayer)
        {
            if (CheckIfGrounded())
                Jump();
        }
    }

    private void FixedUpdate()
    {
        // If the player is not hanging, they can move
        if (isLocalPlayer)
        {
            if (!mPlayerClimbing.GetIsHanging())
                Move();

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
        // If the player has pressed the spacebar and its character is grounded
        if ((Input.GetKeyDown("space") || Input.GetKeyDown("joystick button 0")) && CheckIfGrounded())
        {
            // Makes the character jump toward the Y axis
            mRb.AddForce(new Vector3(0.0f, mJumpPower, 0.0f), ForceMode.Impulse);
            SetWasJumping(true);
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