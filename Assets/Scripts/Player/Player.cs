using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player : Entity<Player> {
    //玩家的inputmanager实例
    public PlayerInputManager inputs { get; protected set; }
    public PlayerStatManager stat { get; protected set; }

    public PlayerStateManagerEvents playerEvents;
    
    public int jumpCounter { get; protected set; }
    
    protected override void Awake(){
        base.Awake();//先让父类初始化
        InitializeInputs();
        InitializeStat();
    }

    protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();
    protected virtual void InitializeStat() => stat = GetComponent<PlayerStatManager>();

    //从Entity类里封装转向、减速函数等
    public virtual void Accelerate(Vector3 inputDir){
        var turningDrag = stat.current.turningDrag;
        var acceleration = stat.current.acceleration;
        var maxSpeed = stat.current.maxSpeed;
        
        Accelerate(inputDir,turningDrag,acceleration,maxSpeed);
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
        isGrounded = false;
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

    /// <summary>
    /// 跳跃逻辑--主要进行跳跃条件判定
    /// </summary>
    public virtual void Jump(){
        isGrounded = true;
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
        verticalVelocity = new Vector3(0, height, 0);
        states.Change<FallPlayerState>();
        playerEvents.OnJump?.Invoke();
    }
}
