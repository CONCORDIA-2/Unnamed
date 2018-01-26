using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform mTarget;

    private Vector3 mOffset;

    void Start()
    {
        mOffset = transform.position - mTarget.position;
    }

    void FixedUpdate()
    {
        Vector3 targetCamPos = mTarget.position + mOffset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Time.deltaTime);
    }
}