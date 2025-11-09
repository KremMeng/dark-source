using UnityEngine;

public class LeftArmAnimFix : MonoBehaviour
{
    private Animator anim;
    private ActorController ac;
    public Vector3 currentAngle;
    public float rotationMultipier = 0.75f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        ac = GetComponentInParent<ActorController>();
    }

    private void OnAnimatorIK()
    {
        //只有在左手武器是遁的时候才fix anim
        if (ac.leftIsShield == true) {
            if (anim.GetBool("defense") == false)
            {
                Transform leftWrist = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                leftWrist.localEulerAngles += currentAngle * rotationMultipier;
                anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm,Quaternion.Euler(leftWrist.localEulerAngles));
            }
        }
    }
}
