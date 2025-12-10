
using UnityEngine;

public class Player : Entity<Player> {
    //玩家的inputmanager实例
    public PlayerInputManager inputs { get; protected set; }
    public PlayerStatManager stat { get; protected set; }

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
    
    /// <summary>
    /// 平滑地转向dir方向，角速度匀速
    /// </summary>
    /// <param name="dir"></param>
    public virtual void FaceDirectionSmooth(Vector3 dir) => FaceDirection(dir, stat.current.rotationSpeed);
    
}
