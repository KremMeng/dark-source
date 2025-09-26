using UnityEngine;
using UnityEngine.Serialization;

public class ActorController : MonoBehaviour
{
    private static readonly int Forward = Animator.StringToHash("forward");
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int Roll = Animator.StringToHash("roll");
    public GameObject model;
    public PlayerInput pi;
    public float walkingSpeed = 2.0f;
    public float runningSpeed = 2.0f;
    [FormerlySerializedAs("jumpingHeight")] public float jumpingVelocity = 2f;
    public float landingVelocity = 5.0f;
    public float rollVelocity;

    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private bool lockPlaner;
    [SerializeField] private Vector3 planerVec; 
    [FormerlySerializedAs("jumpThrust")] [SerializeField] private Vector3 thrust;
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
        anim.SetFloat(Forward, pi.dirMagnity * Mathf.Lerp(anim.GetFloat(Forward) , (pi.run?2.0f:1.0f) ,0.5f) );//walk2run
        
        if (pi.jump)
        {
            anim.SetTrigger(Jump);
        }
       
        if(rigid.velocity.magnitude >landingVelocity){
            anim.SetTrigger(Roll);
        }
    }
    private void FixedUpdate()
    {
        //刚体移动
        if (lockPlaner == false)
        {
            planerVec = model.transform.forward * (pi.dirMagnity * walkingSpeed * (pi.run?runningSpeed:1.0f));
        }
        //角色朝向
        if (pi.dirMagnity > 0.1f)
        {
            //model.transform.forward = pi.dirVector;
            model.transform.forward = Vector3.Slerp(model.transform.forward, pi.dirVector,0.5f);
        }
        rigid.velocity = new Vector3(planerVec.x, rigid.velocity.y, planerVec.z) + thrust;
        thrust = Vector3.zero;
    }

    /// <summary>
    /// Message processing block
    /// </summary>
    public void OnJumpEnter()
    {
        pi.inputEnabled = false;
        lockPlaner = true;
        thrust = new Vector3(0, 4.0f, 0);
    }
    
    public void IsOnGround()
    {
        anim.SetBool("isGrounded",true);
    }

    public void IsNotGround()
    {
        anim.SetBool("isGrounded",false);
    }

    public void OnGroundEnter()
    {
        pi.inputEnabled = true;
        lockPlaner = false;
    }

    public void OnFallEnter()
    {
        pi.inputEnabled = false;
        lockPlaner = true;
    }
    
    public void OnRollEnter(){
        
        pi.inputEnabled = false;
        lockPlaner = true;
        thrust = new Vector3(0, rollVelocity,0);
    }
    
    public void OnJabEnter(){
        
        pi.inputEnabled = false;
        lockPlaner = true;
    }

    public void OnJabUpdate()
    {
        thrust = -model.transform.forward;
    }
}
