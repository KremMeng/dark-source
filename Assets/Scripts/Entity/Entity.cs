using UnityEngine;

public abstract class EntityBase : MonoBehaviour {

    public EntityEvents entityEvents; 
    public Vector3 unsizedPos => transform.position;
    public float m_groundOffset = 0.1f;
    [SerializeField] public bool isGrounded { get; protected set; }
    public bool InputEnabled { get; set; }
    public float timeOfLastGrounded { get; protected set; }
    //cc
    public CharacterController cc { get; protected set; }
    public float originHeight { get; protected set; }
    public float height => cc.height; //碰撞器的高度
    public float radius => cc.radius; //碰撞器的直径
    public Vector3 center => cc.center; //碰撞器相较于transform的local坐标偏移，为了包住角色在编辑器面板上设为（0，1，0）
    public Vector3 position => transform.position + center; //加个center以防万一特殊情况需要改cc的中心，见上一条注释
    //rigidbody
    
    public float groundDip { get; protected set; }  //当前地面倾角
    public Vector3 groundNormal { get; protected set; }   //当前的地面法线
    public Vector3 localSlopeDir { get; protected set; }  //当前地面的局部斜坡朝向
    public RaycastHit groundHit;    //留住当前地面检测点信息，以免后续有用

    public Vector3 feetPos => position - transform.up * (height * 0.5f - cc.stepOffset); //stepOffset是台阶距离，也在面板上设置了

    /// <summary>
    /// 检测是否在斜坡上，斜坡和平地的摩擦力不同
    /// </summary>
    public virtual bool OnSlope(){
        return false;
    }
    
    /// <summary>
    /// 封装一下原球形射线检测方法，普通射线检测只能cast一个点出去，而这个可以roll一个有半径的球出去
    /// </summary>
    /// <param name="hit">地面碰撞点位置、法线等信息</param>
    /// <param name="distance">可投射的距离</param>>
    /// <returns>返回bool值</returns>
    public virtual bool SphereCast(Vector3 castDir,float distance,
        out RaycastHit hit,int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        float castDistance = Mathf.Abs(distance - radius);
        return Physics.SphereCast(position,radius,castDir,
            out hit,castDistance,layer,queryTriggerInteraction);
    }
    /// <summary>
    /// 踩踏检测：判断hit点是不是在脚步下方
    /// </summary>
    /// <returns></returns>
    public virtual bool IsUnderFeet(Vector3 hitPoint){
        return feetPos.y > hitPoint.y;
    }

}
//泛型抽象类，给T增加一个泛型约束
public abstract class Entity<T> : EntityBase where T : Entity<T> {
    public EntityStateManager<T> states { get; protected set; } //类型是Manager，对外意图是“玩家的所有状态”
    
    public Vector3 velocity { get; set; }   //当前速度
    
    //系数运行时可能会变，所以不放在静态配置里
    public float turningDragMulti { get; set; } = 1.0f; //转向时阻力系数,值越大阻力越小
    public float maxSpeedMulti { get; set; } = 1.0f;    //最大速度系数
    public float accelerationMulti { get; set; } = 1.0f;    //加速度系数
    public float decelerMulti { get; set; } = 5.0f;    //加速度系数
    public Vector3 horizontalVelocity {
        get { return new Vector3(velocity.x, 0, velocity.z); }
        set { velocity = new Vector3(value.x, velocity.y, value.z); }   //赋值时只改 X/Z，保留原 Y 
    }
    public Vector3 verticalVelocity {
        get { return new Vector3(0, velocity.y, 0); }
        set { velocity = new Vector3(velocity.x, value.y, velocity.z); }
    }
    public bool freezeVelocity { get; protected set; } //进入状态时冻结速度防止平移
    
    protected virtual void Awake(){
        //初始化状态管理器
        InitializeStateManager();
        InitializeCharactorController();
    }

    protected virtual void Update(){
        //角色控制器开启时，处理状态机步进逻辑
        if (cc.enabled) {
            HandlleStates();
            HandleMovementController();
            HandleGround();
        }
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
    public virtual void Accelerate(Vector3 inputDir,float turningDrag,float acceleration,float maxSpeed){
            
        if (inputDir.sqrMagnitude > 0) {
            //确保输入方向标准化（防止累积） 
            inputDir = inputDir.normalized;
            
            //把水平速度拆成"想去的方向"和"想甩掉的残留方向",让角色自然转向
            var inputDirSpeed = Vector3.Dot(inputDir,horizontalVelocity); //cos投影模长
            var inputDirVelocity = inputDir * inputDirSpeed;
            var turningVelocity = horizontalVelocity - inputDirVelocity;
                
            //转向时会有阻力，需要逐渐减掉"想甩掉的残留方向",直到0
            var turningDelta = horizontalVelocity.magnitude * turningDrag * turningDragMulti * Time.deltaTime;
                
            //计算允许的最大速度,系数可以用于加buff
            var targetMaxSpeed = maxSpeed * maxSpeedMulti;
                
            //修正：确保速度不超过最大值
            if (horizontalVelocity.magnitude >= targetMaxSpeed && inputDirSpeed >= 0) {
                //已经达到最大速度且同向，不再加速
                inputDirSpeed = Mathf.Min(inputDirSpeed, targetMaxSpeed);
            }
            else {
                //关键修复：只在需要加速时才增加速度，避免持续累积
                if (inputDirSpeed < targetMaxSpeed) {
                    inputDirSpeed += acceleration * accelerationMulti * Time.deltaTime;
                    //严格限制速度范围
                    inputDirSpeed = Mathf.Clamp(inputDirSpeed, -targetMaxSpeed, targetMaxSpeed);
                }
            }
                
            //重新计算最终的：目标方向速度和水平速度
            inputDirVelocity = inputDir * inputDirSpeed;
            turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turningDelta);
            horizontalVelocity = inputDirVelocity + turningVelocity;    //加速后的目标方向+逐渐衰减的残留方向
                  
            //额外的安全检查：确保总速度不超过上限
            if (horizontalVelocity.magnitude > targetMaxSpeed) {
                horizontalVelocity = horizontalVelocity.normalized * targetMaxSpeed;
            }
        }
    }

    /// <summary>
    /// 匀速移动
    /// </summary>
    public void ConstantSpeedMove(Vector3 inputDir,float maxSpeed,float rotateLerpMulti){
        if (inputDir.sqrMagnitude > 0.01f) {
            cc.Move(inputDir * maxSpeed * Time.deltaTime);
            Quaternion target = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotateLerpMulti * Time.deltaTime);
        }
    }
    /// <summary>
    /// 减速-
    /// </summary>
    public virtual void Decelerate(float deceleration){
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
        else if (isGrounded) {
             verticalVelocity = new Vector3(0, -1.0f, 0);
         }
    }
    //角色动作控制--velocity位移
    protected virtual void HandleMovementController(){
        //位移==速度*时间 
        if (cc.enabled && !freezeVelocity) {
            cc.Move(velocity * Time.deltaTime);
        }
        else {
            //如果没开启cc就用position计算
            transform.position += velocity * Time.deltaTime;
        }
        
    }
    
    //Entity需要Manager来驱动
    protected virtual void InitializeStateManager() => states = GetComponent<EntityStateManager<T>>();

    //轮询
    protected virtual void HandlleStates() => states.Step();

    //处理地面相关的逻辑
    protected virtual void HandleGround(){
        
        float castRid = height * 0.5f + m_groundOffset;//胶囊中心点稍微抬高一点点，防止卡地形
        //如果碰撞检测上了，而且正在下落或者已经落地
        if (SphereCast(Vector3.down,castRid,out var hit) && verticalVelocity.y <= 0) {
            //如果不在地面
            if (!isGrounded) {
                //如果满足落地条件，那就进入地面状态
                if (CanLanding(hit)) {
                    EnterGround(hit);
                }
            }
            //如果在地，那就更新地面信息
            else if (IsUnderFeet(hit.point)) {
                UpdateGround(hit);
            }
        }
        else {
            ExitGround();
        }
    }
   
    /// <summary>
    /// 判断是否满足落地条件
    /// </summary>
    /// <returns></returns>
    public virtual bool CanLanding(RaycastHit hit){
        //hit点在脚下 并且 落地点的法线和up夹角 ＜ 角色控制器的“最大限制角度”
        return IsUnderFeet(hit.point) && Vector3.Angle(hit.normal, Vector3.up) < cc.slopeLimit;
    }

    protected virtual void EnterGround(RaycastHit hit){
        //不在地的时候，转入地面状态，防止重复触发
        if (!isGrounded) {
            groundHit = hit;
            isGrounded = true;
            entityEvents.OnGroundEnter?.Invoke();
        }
    }
    //角色刚离开地面调用
    protected virtual void ExitGround(){
        if (isGrounded) {
            isGrounded = false;
            //解绑与地面的父子关系，eg站在移动平台上
            transform.parent = null;
            //记录离开地面的时间（跳跃缓冲判断）
            timeOfLastGrounded = Time.time;
            //限制垂直速度：如果在向下运动不干涉，如果向上运动就要
        }
    }
    //每帧更新地面相关数据
    protected virtual void UpdateGround(RaycastHit hit){
        if (isGrounded) {
            groundHit = hit;
            groundNormal = groundHit.normal;
            groundDip = Vector3.Angle(Vector3.up, groundHit.normal);
            //局部坡度法线方向
            localSlopeDir = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
            //如果地面tag为平台类型，让角色成为平台的子物体;tag是其它类型就不管
            //transform.parent = hit.collider.CompareTag(GameTags.Platform) ? hit.transform : null;
        }
    }
    /// <summary>
    /// 设置一个布尔值flag，进入某个状态时冻结速度，防止平移(尤其是trigger一次的状态)
    /// </summary>
    /// <param name="yes">为true时冻结</param>
    public virtual void IsFrozeVelocity(bool yes){
        freezeVelocity = yes;
    }
}
