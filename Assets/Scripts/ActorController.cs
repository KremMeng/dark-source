using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ActorController : MonoBehaviour
{
    private static readonly int Forward = Animator.StringToHash("forward");
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int Roll = Animator.StringToHash("roll");
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int Attack1HAVelocity = Animator.StringToHash("attack1hAVelocity");
    private static readonly int Defense = Animator.StringToHash("defense");

    public GameObject model;
    public IUserInput playerInput;
    public float walkingSpeed = 2.0f;
    public float runningSpeed = 2.0f;
    [FormerlySerializedAs("jumpingHeight")] public float jumpingVelocity = 2f;
    public float landingVelocity = 5.0f;
    public float rollVelocity;

    [Space(10)] [Header("===== Friction Settings =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;
    
    
    private Animator anim;
    private Rigidbody rigid;
    private CapsuleCollider col;
    private Vector3 movingVec; //planerVec
    private Vector3 thrust;
    private bool _canAttack;
    private bool freezeVelocity;//lockPlaner
    private Vector3 deltaPos;
    public float alpha = 0.7f;
    
    [SerializeField]
    private CameraController camCon;

    private bool trackDirection;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach (var input in inputs)
        {
            if (input.enabled)
            {
                playerInput = input;
                break;
            }
        }
        //playerInput = GetComponent<IUserInput>();
        
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        //注入混合树参数
        //区分是否是索敌状态
        if (camCon.lockState == false) {
            //anim.SetFloat(Forward, playerInput.dirMagnity*(playerInput.run?2.0f:1.0f));
            anim.SetFloat(Forward, playerInput.dirMagnity * Mathf.Lerp(anim.GetFloat(Forward) , (playerInput.run?2.0f:1.0f) ,0.5f) );//walk2run
            anim.SetFloat("right",0);
        }
        else {
            Vector3 localVec = transform.InverseTransformVector(playerInput.dirVector);
            anim.SetFloat("forward",localVec.z * (playerInput.run?2.0f:1.0f));
            anim.SetFloat("right",localVec.x * (playerInput.run?2.0f:1.0f));
        }
        
        anim.SetBool(Defense,playerInput.defense);
        
        if (playerInput.jump)
        {
            anim.SetTrigger(Jump);
            _canAttack = false;
        }

        if (playerInput.attack && (CheckState("ground") || CheckStateTag("attack"))  && _canAttack) //状态名，不要填成混合树参数
        {
            anim.SetTrigger(Attack);
        }
        
        if(playerInput.roll || rigid.velocity.magnitude >10.0f){
            anim.SetTrigger("roll");
            _canAttack = false;
        }

        if (playerInput.lockon) {
            camCon.LockOrLockOn();
        }
        
        //角色朝向、向量、速度等物理信息
        if (camCon.lockState == false) {
            //角色朝向
            if (playerInput.dirMagnity > 0.05f)
            {
                //model.transform.forward = pi.dirVector;
                model.transform.forward = Vector3.Slerp(model.transform.forward, playerInput.dirVector,0.5f);
            }
            //刚体移动
            if (freezeVelocity == false)
            {
                movingVec = model.transform.forward * (playerInput.dirMagnity * walkingSpeed * (playerInput.run?runningSpeed:1.0f));
            }
        }
        else {
            //索敌时，角色移动时朝向面对目标敌人
            if (trackDirection == false) {
                model.transform.forward = transform.forward;
            }
            //但是roll和jump时，朝向需要能够往两侧转
            else {
                
                model.transform.forward = movingVec.normalized;
            }
            
            if (freezeVelocity == false) {
                movingVec = playerInput.dirVector * (walkingSpeed * (playerInput.run ? runningSpeed : 1.0f));
            }
        }
        
        rigid.position += deltaPos;
        rigid.velocity = new Vector3(movingVec.x, rigid.velocity.y, movingVec.z) + thrust;
        thrust = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    /// <summary>
    /// 信号处理块
    /// </summary>
    public void OnJumpEnter()
    {
        playerInput.inputEnabled = false;
        freezeVelocity = true;
        trackDirection = true;
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
        playerInput.inputEnabled = true;
        freezeVelocity = false;
        trackDirection = false;
        _canAttack = true;
        col.material = frictionOne;
    }

    public void OnGroundExit()
    {
        col.material = frictionZero;
    }

    public void OnFallEnter()
    {
        playerInput.inputEnabled = false;
        freezeVelocity = true;
    }
    
    public void OnRollEnter(){
        
        playerInput.inputEnabled = false;
        freezeVelocity = true;
        trackDirection = true;
        thrust = new Vector3(0, rollVelocity,0); //向上的冲量
    }
    
    public void OnJabEnter(){
        
        playerInput.inputEnabled = false;
        freezeVelocity = true;
    }

    public void OnJabUpdate()
    {
        //thrust = -model.transform.forward;
        thrust = model.transform.forward * anim.GetFloat("onJabVelocity"); //乘以曲线，曲线值为负，不需要加负号反向
    }

    //攻击部分状态机
    //1.加Lerp平滑动画
    public void OnAttack1hAEnter()
    {
        playerInput.inputEnabled = false;
        //freezeVelocity = true;
        //动态调整layer权重
        //layerWeightTarget = 1.0f;
    }

    public void OnAttack1hAUpdate()
    {
        thrust = model.transform.forward * anim.GetFloat(Attack1HAVelocity); //model朝向的位移
        //Idle到攻击动画做插值
       // anim.SetLayerWeight(anim.GetLayerIndex("attack"),Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")),layerWeightTarget,0.4f));
    }
    
    //2.限制攻击条件
        //1.CheckState判断isGrounded状态才能 2.canAttack描述
    private bool CheckState(string stateName,string layerName = "Base Layer")
    {
        int layerIndex = anim.GetLayerIndex(layerName);
        bool result = anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        return result;
    }
    private bool CheckStateTag(string tagName,string layerName = "Base Layer")
    {
        int layerIndex = anim.GetLayerIndex(layerName);
        bool result = anim.GetCurrentAnimatorStateInfo(layerIndex).IsTag(tagName);
        return result;
    }

    public void OnUpdateRM(object _deltaPos)
    {
        if (CheckState("attack1hC"))
        {
              //deltaPos += (Vector3)_deltaPos;
              deltaPos = alpha * deltaPos + (1 - alpha) * (Vector3)_deltaPos;
        }
    }
}
