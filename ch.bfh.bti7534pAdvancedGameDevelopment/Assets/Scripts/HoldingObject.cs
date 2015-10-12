using UnityEngine;
using System.Collections;

public class HoldingObject : MonoBehaviour
{
    private Animator _animator;

    public bool ikActive = false;
    public Transform rightHandObj = null;
    public Transform lookObj = null;



    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (_animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {

                // Set the look target position, if one has been assigned
                if (lookObj != null)
                {
                    _animator.SetLookAtWeight(1);
                    _animator.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target pos. and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }

            }

            // if the IK is not active, set the position and rotation of 
            // the hand and head back to the original position
            else
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetLookAtWeight(0);
            }
        }
    }
}