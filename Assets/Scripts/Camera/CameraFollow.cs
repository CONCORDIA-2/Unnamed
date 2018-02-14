using UnityEngine;

// Author: Tri-Luong Steven Dien
public class CameraFollow : MonoBehaviour
{
    // Changeable in the Unity Inspector
    public Transform mTarget;

    // Private variables
    private Vector3 mDistanceBetweenCameraAndTarget;

    private void Start()
    {
        // Stick the camera to the same x axis as the character
        transform.position = new Vector3(mTarget.position.x, transform.position.y, transform.position.z);

        // Gets the distance between the player character and the position of the main camera
        mDistanceBetweenCameraAndTarget = transform.position - mTarget.position;
    }

    private void Update()
    {
        // Gets the future camera position
        Vector3 targetCamPos = mTarget.position + mDistanceBetweenCameraAndTarget;
        
        // Smoothly follow the player character
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Time.deltaTime * 2f);
    }
}