using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform mTarget;

    private Vector3 mOffset;

    void Start()
    {
        // Gets the distance between the player character and the position of the main camera
        mOffset = transform.position - mTarget.position;
    }

    void FixedUpdate()
    {
        // Gets the future camera position
        Vector3 targetCamPos = mTarget.position + mOffset;
        
        // Smoothly follow the player character
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Time.deltaTime);
    }
}