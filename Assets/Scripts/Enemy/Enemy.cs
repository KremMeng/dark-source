using Entity;
using UnityEngine;
public class Enemy : Entity<Enemy> {
    
    protected override void Awake(){
        base.Awake();
        InitializeStatManager();
    }
    public Player player;
    public EnemyHealth health { get;protected set; }
    public EnemyStatManager stat { get;protected set; }
    
    public EnemyStateManagerEvents enemyEvents;


    public Vector3 horizonVelocity {
        get { return new Vector3(horizonVelocity.x, 0, horizonVelocity.z); }
        set { velocity = new Vector3(horizonVelocity.x, velocity.y, horizonVelocity.z); }
    }


    // 储存视野碰撞体的数组
    protected GameObject[] viewOverlaps = new GameObject[50];
    // 储存攻击碰撞体的数组
    protected GameObject[] attackOverlaps = new GameObject[50];
    
    
    
    // 初始化
    protected virtual void InitializeStatManager() => GetComponent<EnemyStatManager>();

    // 包一层基类移动
    protected virtual void Accelerate(Vector3 dir){
        var turningDrag = stat.current.turningDragMulti;
        var accelerateMulti = stat.current.accelerationMulti;
        var maxSpeedMulti = stat.current.maxSpeedMulti;

        Accelerate(dir,turningDrag ,accelerateMulti ,maxSpeedMulti);
    }
    
    protected virtual void Decelerate(){
        var deceleration = stat.current.decelerMulti;
        Decelerate(deceleration);
    }

    protected virtual void FaceDirection(Vector3 dir){
        
        float degreesPerSpeed = stat.current.rotationSpeed;
        FaceDirection(dir, degreesPerSpeed);

    }
    
    // 勘测并绑定Player
    protected void HandleSight(){
        
    }
    
    // 攻击Player
    protected void HandleAttack(){
        
    }
    
    
}
