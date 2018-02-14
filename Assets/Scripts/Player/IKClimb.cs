using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKClimb : MonoBehaviour
{
    public bool mUseIK;

    public bool mLeftHandIK;
    public bool mRightHandIK;

    public bool mLeftFootIK;
    public bool mRightFootIK;

    public Vector3 mLeftHandPosition;
    public Vector3 mRightHandPosition;

    public Vector3 mLeftHandOriginalPosition;
    public Vector3 mRightHandOriginalPosition;

    public Vector3 mLeftFootPosition;
    public Vector3 mRightFootPosition;

    public Vector3 mLeftHandOffset;
    public Vector3 mRightHandOffset;

    public Vector3 mLeftFootOffset;
    public Vector3 mRightFootOffset;

    private Quaternion mLeftHandRot;
    private Quaternion mRightHandRot;

    private Quaternion mLeftFootRot;
    private Quaternion mRightFootRot;

    public Quaternion mLeftFootRotOffset;
    public Quaternion mRightFootRotOffset;

    private Animator mAnimator;

	void Start ()
    {
        mAnimator = GetComponent<Animator>();
	}
	
	void FixedUpdate ()
    {
        RaycastHit LeftHandHit;
        RaycastHit RightHandHit;
        RaycastHit LeftFootHit;
        RaycastHit RightFootHit;

        // Left Hand IK Check
        if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0.0f, 2.0f, 0.5f)), transform.TransformDirection(new Vector3(-0.5f, -1.0f, 0.0f)), out LeftHandHit, 1f))
        {
            Vector3 lookAt = Vector3.Cross(-LeftHandHit.normal, transform.right);
            lookAt = lookAt.y < 0 ? -lookAt : lookAt;

            mLeftHandIK = true;
            mLeftHandPosition = LeftHandHit.point - transform.TransformDirection(mLeftHandOffset);
            mLeftHandRot = Quaternion.LookRotation(LeftHandHit.point + lookAt, LeftHandHit.normal);
        }
        else
            mLeftHandIK = false;

        // Right Hand IK Check
        if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0.0f, 2.0f, 0.5f)), transform.TransformDirection(new Vector3(0.5f, -1.0f, 0.0f)), out RightHandHit, 1f))
        {
            Vector3 lookAt = Vector3.Cross(-RightHandHit.normal, transform.right);
            lookAt = lookAt.y < 0 ? -lookAt : lookAt;

            mRightHandIK = true;
            mRightHandPosition = RightHandHit.point - transform.TransformDirection(mRightHandOffset);
            mRightHandRot = Quaternion.LookRotation(RightHandHit.point + lookAt, RightHandHit.normal);
        }
        else
            mRightHandIK = false;

        // Left FoodT IK Check
        if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(-0.2f, 0.3f, 0.0f)), transform.forward, out LeftFootHit, 0.5f))
        {
            mLeftFootIK = true;
            mLeftFootPosition = LeftFootHit.point - mLeftFootOffset;
            mLeftFootRot = (Quaternion.FromToRotation(Vector3.up, LeftFootHit.normal)) * mLeftFootRotOffset;
        }
        else
            mLeftFootIK = false;

        // Right Foot IK Check
        if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0.2f, 0.3f, 0.0f)), transform.forward, out RightFootHit, 0.5f))
        {
            mRightFootIK = true;
            mRightFootPosition = RightFootHit.point - mRightFootOffset;
            mRightFootRot = (Quaternion.FromToRotation(Vector3.up, RightFootHit.normal)) * mRightFootRotOffset;
        }
        else
            mRightFootIK = false;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0.0f, 2f, 0.50f)), transform.TransformDirection(new Vector3(-0.5f, -1.0f, 0.0f)), Color.green);
        Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0.0f, 2f, 0.50f)), transform.TransformDirection(new Vector3(0.5f, -1.0f, 0.0f)), Color.green);

        Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(-0.2f, 0.3f, 0.0f)), transform.forward, Color.red);
        Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0.2f, 0.3f, 0.0f)), transform.forward, Color.red);
    }

    private void OnAnimatorIK()
    {
        if (mUseIK)
        {
            mLeftHandOriginalPosition = mAnimator.GetIKPosition(AvatarIKGoal.LeftHand);
            mRightHandOriginalPosition = mAnimator.GetIKPosition(AvatarIKGoal.RightHand);

            if (mLeftHandIK)
            {
                mAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                mAnimator.SetIKPosition(AvatarIKGoal.LeftHand, mLeftHandPosition);

                mAnimator.SetIKRotation(AvatarIKGoal.LeftHand, mLeftHandRot);
                mAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            }
            if (mRightHandIK)
            {
                mAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                mAnimator.SetIKPosition(AvatarIKGoal.RightHand, mRightHandPosition);

                mAnimator.SetIKRotation(AvatarIKGoal.RightHand, mRightHandRot);
                mAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            }
            if (mLeftFootIK)
            {
                mAnimator.SetIKPosition(AvatarIKGoal.LeftFoot, mLeftFootPosition);
                mAnimator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);

                mAnimator.SetIKRotation(AvatarIKGoal.LeftFoot, mLeftFootRot);
                mAnimator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
            }
            if (mRightFootIK)
            {
                mAnimator.SetIKPosition(AvatarIKGoal.RightFoot, mRightFootPosition);
                mAnimator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);

                mAnimator.SetIKRotation(AvatarIKGoal.RightFoot, mRightFootRot);
                mAnimator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
            }
        }
    }
}
