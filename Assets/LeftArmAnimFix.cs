using UnityEngine;

public class LeftArmAnimFix : MonoBehaviour
{
    private Animator anim;
    public Vector3 currentAngle;
    public float rotationMultipier = 0.75f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK()
    {
        Transform leftWrist = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        leftWrist.localEulerAngles += currentAngle * rotationMultipier;
        anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm,Quaternion.Euler(leftWrist.localEulerAngles));

    }
}
