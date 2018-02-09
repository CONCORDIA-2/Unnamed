using UnityEngine;

// Author: Tri-Luong Steven Dien
public class PlayerController : MonoBehaviour
{
    // Changeable in the Unity Inspector
    public float mMovementSpeed;
    public float mJumpPower;
    public float mMaxSpeed;
    public Transform[] mClimbWaypoints;
    public Transform mCharacterHands;

    // Private variables
    private float mRotationDegreesPerSecond = 220f;
    private bool mGrounded = true;
    private Rigidbody mRb;

    // Climbing system variables
    private Vector3[] mWaypoints;
    private Vector3 mStartPos;
    private Vector3 mEndPos;
    private int mCurrentStartPoint;
    private float mStartTime;
    private float mClimbSpeed = 1.0f;
    private float mJourneyLength;
    private bool mIsClimbing = false;
    private bool mAllowToClimb = false;

    // Pickup | Dropdown Objects
    private bool mHoldingObject = false;
    private GameObject mObjectInHands;
    private GameObject mObjectInRange;

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

        // If the player is allowed to climb up (Entered a wall detection collider), pressing 'U' will make the player "climb up"
        if (Input.GetKeyDown(KeyCode.U) & mAllowToClimb)
        {
            RegisterWaypoints();
            mRb.isKinematic = true;
            mIsClimbing = true;
        }

        // Linearly interpolate using the waypoints
        if (mIsClimbing && mAllowToClimb)
            ClimbUp();

        if (mObjectInRange && Input.GetKeyDown(KeyCode.E))
            PickupObject();

        if (mObjectInHands && Input.GetKeyDown(KeyCode.R))
            DropDownObject();
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
            mRb.AddForce(new Vector3(0.0f, mJumpPower, 0.0f), ForceMode.Impulse);
            mGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Checks if the character is touching a "jumpable" object (box, terrain, etc)
        if (collision.collider.gameObject.tag == "Jumpable")
            mGrounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the character is touching a "jumpable" object (box, terrain, etc)
        if (other.gameObject.tag == "Climbable")
            mAllowToClimb = true;

        if (other.gameObject.tag == "Pickable")
            mObjectInRange = other.transform.parent.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        // Checks if the character is touching a "jumpable" object (box, terrain, etc)
        if (other.gameObject.tag == "Climbable" && !mIsClimbing)
            mAllowToClimb = false;

        if (other.gameObject.tag == "Pickable")
            mObjectInRange = null;
    }

    float Remap (float val, float min1, float max1, float min2, float max2) {
        return (val - min1) / (max1 - min1) * (max2 - min2) + min2;
    }

    // Updates the next set of waypoints (start and finish)
    private void SetPoints()
    {
        // If check to make sure, it is not out of bounds
        if (mCurrentStartPoint + 1 < mWaypoints.Length)
        {
            mStartPos = mWaypoints[mCurrentStartPoint];
            mEndPos = mWaypoints[mCurrentStartPoint + 1];
            mStartTime = Time.time;
            mJourneyLength = Vector3.Distance(mStartPos, mEndPos);
        }
        else if (mCurrentStartPoint + 1 == mWaypoints.Length) 
        {
            // The climbing is done, put to false all the climbing bool variables and disable the 'Is Kinematic'
            mIsClimbing = false;
            mAllowToClimb = false;
            mRb.isKinematic = false;
        }
    }

    // Linearly intepolate between two vectors (Climbing up)
    private void ClimbUp()
    {
        float distCovered = (Time.time - mStartTime) * mClimbSpeed;
        float fracJourney = distCovered / mJourneyLength;

        transform.position = Vector3.Lerp(mStartPos, mEndPos, fracJourney);

        if (fracJourney >= 1f && mCurrentStartPoint + 1 < mWaypoints.Length)
        {
            mCurrentStartPoint++;
            SetPoints();
        }
    }

    // Register the climb waypoints
    private void RegisterWaypoints()
    {
        mWaypoints = new Vector3[mClimbWaypoints.Length];

        for (int i = 0; i < mWaypoints.Length; ++i)
            if (i == 0 || i == mWaypoints.Length - 1)
                mWaypoints[i] = mClimbWaypoints[i].position;
            else
                mWaypoints[i] = mClimbWaypoints[i].position - new Vector3(0.0f, mWaypoints[i - 1].y, 0.0f);

        mCurrentStartPoint = 0;
        SetPoints();
    }

    // Pickup a nearby object
    private void PickupObject()
    {
        mObjectInHands = mObjectInRange;
        mObjectInHands.GetComponentInParent<Rigidbody>().isKinematic = true;
        mObjectInHands.transform.parent = mCharacterHands;
        mObjectInHands.transform.localPosition = Vector3.zero;
        mHoldingObject = true;
    }

    // Dropdown the object in hands
    private void DropDownObject()
    {
        mObjectInHands.transform.parent = null;
        mObjectInHands.GetComponentInParent<Rigidbody>().isKinematic = false;
        mHoldingObject = false;
    }
}
