
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

    public virtual void Accelerate(Vector3 inputDir){
        var turningDrag = stat.current.turningDrag;
        var acceleration = stat.current.acceleration;
        var maxSpeed = stat.current.maxSpeed;
        
        Accelerate(inputDir,turningDrag,acceleration,maxSpeed);
    }
}
