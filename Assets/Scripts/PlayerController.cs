using UnityEngine;

// Author: Tri-Luong Steven Dien
public class PlayerController : MonoBehaviour
{
    // Changeable in the Unity Inspector
    public float mMovementSpeed;
    public float mJumpPower;
    public float mMaxSpeed;

    // Private variables
    private float mRotationDegreesPerSecond = 220f;
    private bool mGrounded = true;
    private Rigidbody mRb;

    void Start()
    {
    	mRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
    	Jump();
    }

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
            if (horizontalSpeed.magnitude > localMaxSpeed || horizontalSpeed.magnitude < -localMaxSpeed) {
                Vector2 temp = horizontalSpeed.normalized * localMaxSpeed;
                mRb.velocity = new Vector3(temp.x, yMag, temp.y);
            }

            force = new Vector3(force.x, yMag, force.z);
            mRb.AddForce(force, ForceMode.Force);
        }
    }

    private void Jump()
    {
        // If the player has pressed the spacebar and its character is grounded
        if ((Input.GetKeyDown("space") || Input.GetKeyDown("joystick button 0")) && mGrounded)
        {
            // Makes the character jump toward the Y axis
            GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, mJumpPower, 0.0f), ForceMode.Impulse);
            mGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Checks if the character is touching a "jumpable" object (box, terrain, etc)
        if (collision.collider.gameObject.tag == "Jumpable")
        {
            mGrounded = true;
        }
    }

    float Remap (float val, float min1, float max1, float min2, float max2) {
        return (val - min1) / (max1 - min1) * (max2 - min2) + min2;
    }
}
