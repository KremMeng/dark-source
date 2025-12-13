using UnityEngine;

public abstract class EntityBase : MonoBehaviour {

    public Vector3 unsizedPos => transform.position;
    public bool isGrounded { get; protected set; } = true;
    
    public CharacterController cc { get; protected set; }
    public float originHeight { get; protected set; }

    /// <summary>
    /// 检测是否在斜坡上，斜坡和平地的摩擦力不同
    /// </summary>
    public virtual bool OnSlope(){
        return false;
    }

}
//泛型抽象类，给T增加一个泛型约束
public abstract class Entity<T> : EntityBase where T : Entity<T> {
    public EntityStateManager<T> states { get; protected set; } //类型是Manager，对外意图是“玩家的所有状态”
    
    public Vector3 velocity { get; set; }   //当前速度
    
    //系数运行时可能会变，所以不放在静态配置里
    public float turningDragMulti { get; set; } = 1.0f; //转向时阻力系数
    public float maxSpeedMulti { get; set; } = 1.0f;    //最大速度系数
    public float accelerationMulti { get; set; } = 1.0f;    //加速度系数
    public float decelerMulti { get; set; } = 1.0f;    //加速度系数
    public Vector3 horizontalVelocity {
        get { return new Vector3(velocity.x, 0, velocity.z); }
        set { velocity = new Vector3(value.x, velocity.y, value.z); }   //赋值时只改 X/Z，保留原 Y 
    }
    public Vector3 verticalVelocity {
        get { return new Vector3(0, velocity.y, 0); }
        set { velocity = new Vector3(velocity.x, value.y, velocity.z); }
    }
    
    protected virtual void Awake(){
        //初始化状态管理器
        InitializeStateManager();
        InitializeCharactorController();
    }

    protected virtual void Update(){
        //处理状态机步进逻辑
        HandlleStates();
        HandleActorController();
    }

    protected virtual void InitializeCharactorController(){
        //获取当前物体身上的cc
        cc = GetComponent<CharacterController>();
        //没有的话就添加一个cc组件
        if (!cc) {
            cc = gameObject.AddComponent<CharacterController>();
        }
        //设置一些基本参数
        cc.skinWidth = 0.005f;
        cc.minMoveDistance = 0;
        originHeight = cc.height;
    }
    /// <summary>
    /// 加速度--阻尼感
    /// </summary>
    /// <param name="inputDir">相机y轴下的输入方向</param>
    public void Accelerate(Vector3 inputDir,float turningDrag,float acceleration,float maxSpeed){
        
        if (inputDir.sqrMagnitude > 0) {
            //把水平速度拆成“想去的方向”和“想甩掉的残留方向”,让角色自然转向
            var inputDirSpeed = Vector3.Dot(inputDir,horizontalVelocity);
            var inputDirVelocity = inputDir * inputDirSpeed;
            var turningVelocity = horizontalVelocity - inputDirVelocity;
            
            //转向时会有阻力，需要逐渐减掉“想甩掉的残留方向”,直到0
            var turningDelta = turningDrag * turningDragMulti * Time.deltaTime;
            turningVelocity = Vector3.MoveTowards(turningVelocity,Vector3.zero,turningDelta);
            
            //计算允许的最大速度,系数可以用于加buff
            var targetMaxSpeed = maxSpeed * maxSpeedMulti;
            //速度没到顶可以继续加，或要转向了也要先反向加速到0（否则转向会太慢）
            if (horizontalVelocity.magnitude < targetMaxSpeed || inputDirSpeed < 0) {
                //计算速度，同时需要限制速度在±maxSpeed
                inputDirSpeed += acceleration * accelerationMulti * Time.deltaTime;  //两个因素影响速度：加速度和dt
                inputDirSpeed += Mathf.Clamp(inputDirSpeed,-targetMaxSpeed, targetMaxSpeed);
            }
            //重新计算最终的：目标方向速度和水平速度
            inputDirVelocity = inputDir * inputDirSpeed;
            horizontalVelocity = inputDirVelocity + turningVelocity;    //加速后的目标方向+逐渐衰减的残留方向
        }
    }
    /// <summary>
    /// 减速-
    /// </summary>
    public void Decelerate(float deceleration){
        var decelerateDelta = deceleration * Time.deltaTime * decelerMulti;
        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, decelerateDelta);
    }
    
    //转向函数,按秒/度转
    public virtual void FaceDirection(Vector3 dir, float degreesPerSpeed){
        //确保输入方向的有效性
        if (dir != Vector3.zero) {
            //当前的旋转角度
            var currentRotation = transform.rotation;
            //目标旋转角--LookAt方向
            var targetRotation = Quaternion.LookRotation(dir, Vector3.up);  //如果传入速度向量，会归一化，丢掉长度
            //最大旋转幅度
            var rotationDelta = degreesPerSpeed * Time.deltaTime;
            //计算最终旋转,RotateTowards角速度固定
            transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationDelta);
        }
    }

    /// <summary>
    /// 给cc添加重力
    /// </summary>
    public virtual void Gravity(bool isGrounded,float gravityMulti,float gravityMaxSpeed,float gravity,float fallGravity){
        var speed = verticalVelocity.y;
        //如果向下的竖直速率还没有到上限，例如向下速度最大为-50，那么-30(下降)、+10(上升)都是可以的
        if (!isGrounded && speed > -gravityMaxSpeed) {
            //重力加速度，区分在空中上升和下降
            var gForce = verticalVelocity.y > 0 ? gravity : fallGravity;
            speed -= gForce * gravityMulti * Time.deltaTime;
            //速度封底:如速度＝=-60那就取更大的-50
            speed = Mathf.Max(speed, -gravityMaxSpeed);
            verticalVelocity = new Vector3(0, speed, 0);
        }
    }
    //角色动作控制--velocity位移
    protected virtual void HandleActorController(){
        //位移==速度*时间
        if (cc.enabled) {
            cc.Move(velocity * Time.deltaTime * 0.1f);
            return;
        }
        //如果没开启cc就用position计算
        transform.position += velocity * Time.deltaTime * 0.1f;
    }
    
    //Entity需要Manager来驱动
    protected virtual void InitializeStateManager() => states = GetComponent<EntityStateManager<T>>();

    //轮询
    protected virtual void HandlleStates() => states.Step();
}
