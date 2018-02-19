using UnityEngine;
using UnityEngine.Networking;

// Author: Tri-Luong Steven Dien
public class Player_Movement : NetworkBehaviour
{
    [Header("Movement")]
    public float mMovementSpeed = 300.0f;
    public float mMaxSpeed = 2.2f;

    [Header("Jump")]
    public float mJumpPower = 120.0f;
    private float mDistanceToGround;
    private bool mWasJumping = false;

    // Orientation
    private float mRotationDegreesPerSecond = 220f;

    private Rigidbody mRb;
    private Player_Climbing mPlayerClimbing;

    public override void OnStartLocalPlayer()
    {
        mRb = GetComponent<Rigidbody>();
        mPlayerClimbing = GetComponent<Player_Climbing>();
        mDistanceToGround = GetComponent<Collider>().bounds.extents.y;
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
            if (direction.magnitude > 1 || direction.magnitude < -1) direction = Vector3.Normalize(direction);

            // Map max speed to input intensity
            float calcSpeed = direction.magnitude / 1f;
            float localMaxSpeed = Remap(calcSpeed, 0f, 1f, 0f, mMaxSpeed);

            // Rotates the character so it faces the same orientation as the vector direction
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), mRotationDegreesPerSecond * Time.deltaTime);

            // Moves the character towards vector direction
            Vector3 force = direction * mMovementSpeed;

            // Keep Y magnitude so as to not affect jump physics with speed clamp
            float yMag = mRb.velocity.y;

            // Limit speed
            Vector2 horizontalSpeed = new Vector2(mRb.velocity.x, mRb.velocity.z);
            if (horizontalSpeed.magnitude > localMaxSpeed || horizontalSpeed.magnitude < -localMaxSpeed)
            {
                Vector2 temp = horizontalSpeed.normalized * localMaxSpeed;
                mRb.velocity = new Vector3(temp.x, yMag, temp.y);
            }

            force = new Vector3(force.x, yMag, force.z);
            mRb.AddForce(force, ForceMode.Force);
        }
    }

    // Remap the local max speed
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
