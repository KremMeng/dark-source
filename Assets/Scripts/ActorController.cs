using UnityEngine;

public class ActorController : MonoBehaviour
{
    private static readonly int Forward = Animator.StringToHash("forward");
    public GameObject model;
    public PlayerInput pi;
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 2.0f;
   

    [SerializeField] 
    private Animator anim;
    private Rigidbody _rigid;
    [SerializeField] private Vector3 movingVec;
    
    // Start is called before the first frame update
    void Awake()
    {
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat(Forward, pi.dirMagnity*(pi.run?2.0f:1.0f));
        //角色朝向
        if (pi.dirMagnity > 0.1f)
        {
            model.transform.forward = pi.dirVector;
        }
        //刚体移动
        movingVec = model.transform.forward * (pi.dirMagnity*walkingSpeed * (pi.run?runningSpeed:1.0f));
    }

    private void FixedUpdate()
    {
        _rigid.velocity = new Vector3(movingVec.x, _rigid.velocity.y, movingVec.z);
    }
}
