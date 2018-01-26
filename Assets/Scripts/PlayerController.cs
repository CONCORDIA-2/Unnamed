using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float mMovementSpeed;
    public float mJumpPower;

    private float mRotationDegreesPerSecond = 180f;
    private bool mGrounded = true;

    void Update()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        // Gets the keyboard inputs (WASD)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Creates a vector direction from the inputs
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical);

        // If there's an input from the player
        if (horizontal != 0f || vertical != 0f)
        {
            // Rotates the character so it faces the same orientation as the vector direction
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), mRotationDegreesPerSecond * Time.deltaTime);

            // Moves the characters towards that vector direction
            transform.position += direction * Time.deltaTime * mMovementSpeed;
        }
    }

    private void Jump()
    {
        // If the player has pressed the spacebar and its character is grounded
        if (Input.GetKeyDown("space") && mGrounded)
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
}
