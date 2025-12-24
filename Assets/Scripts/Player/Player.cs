using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player : Entity<Player> {
    //玩家的inputmanager实例
    public PlayerInputManager inputs { get; protected set; }
    public PlayerStatManager stat { get; protected set; }

    public PlayerStateManagerEvents playerEvents;
    
    public int jumpCounter { get; protected set; }
    public bool run { get; protected set; }
    
    protected override void Awake(){
        base.Awake();//先让父类初始化
        InitializeInputs();
        InitializeStat();
        
        //运行的时候监听落地事件，重置跳跃/空中技能次数
        entityEvents.OnGroundEnter.AddListener(()=>ResetJump());
    }

    protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();
    protected virtual void InitializeStat() => stat = GetComponent<PlayerStatManager>();
    
    internal void ChangeOnAnimFinish(System.Action callback) =>
        GetComponent<PlayerAnimator>().QueueFinishCallBack(callback);

    //从Entity类里封装转向、减速函数等
    public virtual void Accelerate(Vector3 inputDir){
        var turningDrag = stat.current.turningDrag;
        var acceleration = stat.current.acceleration;
        var maxSpeed = stat.current.maxSpeed;
        
        Accelerate(inputDir,turningDrag,acceleration,maxSpeed);
    }
    public virtual void ConstantSpeedMove(Vector3 inputDir){
        var maxSpeed = stat.current.maxSpeed;
        var turningDrag = stat.current.turningDrag;
        ConstantSpeedMove( inputDir, maxSpeed, turningDrag);
    }
    public virtual void Decelerate() => Decelerate(stat.current.deceleration);
    
    /// <summary>
    /// 减速--摩擦力区分
    /// </summary>
    public virtual void Friction(){
        if (OnSlope()) {
            Decelerate(stat.current.slopeFriction);
        }
        else {
            Decelerate(stat.current.groundFriction);
        }
    }

    public virtual void Gravity(){
        var gravityMaxSpeed = stat.current.gravityMaxSpeed;
        var gravityMulti = 1.0f;
        var gravity = stat.current.gravity;
        var fallGravity = stat.current.fallGravity;
        
        Gravity(isGrounded,gravityMulti,gravityMaxSpeed,gravity,fallGravity);
    }
    /// <summary>
    /// 平滑地转向dir方向，角速度匀速
    /// </summary>
    /// <param name="dir"></param>
    public virtual void FaceDirectionSmooth(Vector3 dir) => FaceDirection(dir, stat.current.rotationSpeed);

    public virtual void AccelerateWithInputDir(){
        var inputDir = inputs.GetMovementDirction();//基于相机的局部朝向
        Accelerate(inputDir);
    }
    /// <summary>
    /// 跳跃逻辑--主要进行跳跃条件判定
    /// </summary>
    public virtual void Jump(){
        //是否触发多段跳
        bool canMultiJump = (jumpCounter > 0) && (jumpCounter < stat.current.multiJumps);
        //是否符合土狼跳跃宽限
        bool canCoyotoJump = (jumpCounter == 0) && (Time.time < timeOfLastGrounded + stat.current.coyotoJumpThreshold);
        //只要满足任意一个能跳的条件：地面起跳、二段跳、刚离开平台但还在土狼时间内也能起跳，那就给一个最小初速度
        if ( isGrounded || canMultiJump || canCoyotoJump) {
            if (inputs.JumpOnPressed()) {
              Jump(stat.current.maxJumpHeight);
            }
        }
        if (inputs.JumpOnReleased() && jumpCounter > 0 && verticalVelocity.y > stat.current.minJumpHeight) {
            verticalVelocity = Vector3.up * stat.current.minJumpHeight;
        }
    }
    //需要改成有水平wei'yi'd
    public virtual void Jump(float height){
        jumpCounter++;
        states.Change<FallPlayerState>();
        playerEvents.OnJump?.Invoke();
    }
    /// <summary>
    /// 重置跳跃计数，避免累加
    /// </summary>
    public virtual void ResetJump() => jumpCounter = 0;
    
    public virtual void Fall(){
        if(!isGrounded) states.Change<FallPlayerState>();
    }
    /// <summary>
    /// 跑步判定
    /// </summary>
    public virtual void Run(){
        print("run pressing:  " + inputs.RunOnPressed());
        print("isGrounded0 "+isGrounded);
        if (isGrounded  && inputs.RunIsPressing()) {
            run = true;
            states.Change<RunPlayerState>();
        }
    }
    //把玩家强制贴到地面上.防止悬空
    public virtual void SnapToGround() => SnapToGround(stat.current.snapForce);

    public virtual void SnapToGround(float snapForce){
        //接地且垂直速度朝下
        if (isGrounded && verticalVelocity.y <= 0) {
            verticalVelocity = Vector3.down * snapForce;
        }
    }

}
