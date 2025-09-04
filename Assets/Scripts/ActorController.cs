using UnityEngine;

public class ActorController : MonoBehaviour
{
    private static readonly int Forward = Animator.StringToHash("forward");
    public GameObject model;
    public PlayerInput pi;
    public float walkingSpeed;

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
        anim.SetFloat(Forward, pi.dirMagnity);
        //角色朝向
        if (pi.dirMagnity > 0.1f)
        {
            model.transform.forward = pi.dirVector;
        }
        //刚体移动
        movingVec = model.transform.forward * (pi.dirMagnity*walkingSpeed);
    }

    private void FixedUpdate()
    {
        //_rigid.velocity = movingVec;
        //_rigid.position += movingVec * Time.fixedDeltaTime;
        _rigid.velocity = new Vector3(movingVec.x, _rigid.velocity.y, movingVec.z);
    }
}
