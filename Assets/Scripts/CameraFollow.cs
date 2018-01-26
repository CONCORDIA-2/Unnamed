using UnityEngine;

// Author: Tri-Luong Steven Dien
public class CameraFollow : MonoBehaviour
{
    // Changeable in the Unity Inspector
    public Transform mTarget;

    // Private variables
    private Vector3 mDistanceBetweenCameraAndTarget;

    void Start()
    {
        // Gets the distance between the player character and the position of the main camera
        mDistanceBetweenCameraAndTarget = transform.position - mTarget.position;
    }

    void FixedUpdate()
    {
        // Gets the future camera position
        Vector3 targetCamPos = mTarget.position + mDistanceBetweenCameraAndTarget;
        
        // Smoothly follow the player character
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Time.deltaTime);
    }
}