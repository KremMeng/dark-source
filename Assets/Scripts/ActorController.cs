using UnityEngine;

public class ActorController : MonoBehaviour
{
    private static readonly int Forward = Animator.StringToHash("forward");
    private static readonly int Jump = Animator.StringToHash("jump");
    public GameObject model;
    public PlayerInput pi;
    public float walkingSpeed = 2.0f;
    public float runningSpeed = 2.0f;
    public float jumpingHeight = 2.5f;

    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private bool lockPlaner;
    [SerializeField] private Vector3 planerVec;
    [SerializeField] private Vector3 jumpThrust;
    // Start is called before the first frame update
    void Awake()
    {
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        anim.SetFloat(Forward, pi.dirMagnity * Mathf.Lerp(anim.GetFloat(Forward) , (pi.run?2.0f:1.0f) ,0.5f) );
        if (pi.jump)
        {
            anim.SetTrigger(Jump);
        }
        //角色朝向
        if (pi.dirMagnity > 0.1f)
        {
            //model.transform.forward = pi.dirVector;
            model.transform.forward = Vector3.Slerp(model.transform.forward, pi.dirVector,0.3f);
        }
        //刚体移动
        if (lockPlaner == false)
        {
            planerVec = model.transform.forward * (pi.dirMagnity * walkingSpeed * (pi.run?runningSpeed:1.0f));
        }
    }
    private void FixedUpdate()
    {
        rigid.velocity = new Vector3(planerVec.x, rigid.velocity.y, planerVec.z) + jumpThrust;
        jumpThrust = Vector3.zero;
    }

    public void OnJumpEnter()
    {
        pi.inputEnabled = false;
        lockPlaner = true;
        jumpThrust = new Vector3(0, 4.0f, 0);
        print("on jump enter");
    }

    public void OnJumpExit()
    {
        pi.inputEnabled = true;
        lockPlaner = false;
    }
}
