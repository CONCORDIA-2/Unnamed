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
         float horizontal = Input.GetAxis("Horizontal");
         float vertical = Input.GetAxis("Vertical");
        
         Vector3 direction = new Vector3(horizontal, 0.0f, vertical);
 
         if (horizontal != 0f || vertical != 0f)
         {
             transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), mRotationDegreesPerSecond * Time.deltaTime);
             transform.position += direction* Time.deltaTime * mMovementSpeed;
         }
     }

    private void Jump()
    {
        if (Input.GetKeyDown("space") && mGrounded)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, mJumpPower, 0.0f), ForceMode.Impulse);
            mGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Jumpable")
        {
            mGrounded = true;
        }
    }
}
