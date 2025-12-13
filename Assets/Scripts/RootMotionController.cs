using UnityEngine;

public class RootMotionController : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorMove()
    {
        SendMessageUpwards("OnUpdateRM",anim.deltaPosition);//装箱
    }
}
  