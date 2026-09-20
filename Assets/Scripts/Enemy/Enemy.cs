using System;
using Unity.VisualScripting;
using UnityEngine;
//继承自Entity基类的敌人类
//主要职责：根据视野距离索敌、攻击-->就相当于player的硬件检测输入
public class Enemy : Entity<Enemy> {

    protected override void Awake(){
        base.Awake();
        InitializeStatManager();
    }

    protected override void OnUpdate(){
        HandleSight();
        ContactDamage();
        HandleAttack();
    }
    
    //敌人类相关属性，如血量、速度、组件等
    public EnemyHealth health { get; protected set; }
    public EnemyStatManager stat { get; protected set; }
    public Player player { get;protected set; }
    
    
    //攻击相关参数
    [SerializeField] protected bool canContactDamage;
    public bool isInAttackRange { get; set; }


    protected void InitializeStatManager() => stat = GetComponent<EnemyStatManager>();
    
    
    //碰撞相关参数,const编译器常量更省
    protected const int maxViewColliders = 128;
    protected const int maxAttackColliders = 128;
    
    protected Collider[] viewColliders= new Collider[maxViewColliders];  //视野范围内
    protected Collider[] attackColliders= new Collider[maxAttackColliders];  //追击范围内
    protected Collider[] contactColliders = new Collider[10]; //近身（被动伤害）范围内
    
    //用Enemy的数值，封装基类相关函数(并非重写)
    protected virtual void FaceDirection(Vector3 dir){
        float turningSpeed = stat.current.turningSpeed;
        FaceDirection(dir, turningSpeed);
    }

    protected virtual void Accelerate(Vector3 dir){
        var turningDrag = stat.current.turningDrag;
        var acceleration = stat.current.acceleration;
        var maxSpeed = stat.current.maxSpeed;
        Accelerate(dir,turningDrag,acceleration,maxSpeed);
    }

    protected virtual void Decelerate() => Decelerate(stat.current.deceleration);

    //根据视野绑定/解绑玩家
    protected virtual void HandleSight(){
        float radius = stat.current.viewRange;
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position,radius,viewColliders);
        
        //没锁定玩家时，遍历碰撞体数组，检测到player就锁定
        if (!player) {
            for(int i = 0;i < numColliders;i++)
            {
                if (viewColliders[i].CompareTag("Player") && viewColliders[i].TryGetComponent<Player>(out Player player)) {
                    this.player = player;
                    float distance = (player.transform.position - transform.position).magnitude;;//敌人和玩家间的距离
                    HandleChase();
                    //事件
                }
            }
        }
        //出视野范围或角色死亡解除锁定
        else{
            float distance = (player.transform.position - transform.position).magnitude;;//敌人和玩家间的距离
            if(distance > stat.current.viewRange) {
                this.player = null;
                //事件
            }
        }
    }
    /// <summary>
    /// player能被敌人看到、却又没到可攻击范围时，追逐player
    /// </summary>
    protected virtual void HandleChase(){
        float distance = (player.transform.position - transform.position).magnitude;;//敌人和玩家间的距离
        if (distance > stat.current.attackRange && distance < stat.current.viewRange) {
            var chaseDir = player.transform.position - transform.position;
            FaceDirection(chaseDir);
            Accelerate(chaseDir);
        }
        else if(distance < stat.current.attackRange){//不停？
            Decelerate();
            GeneralAttack();
        }
    }
    //player的距离进入到攻击范围内，敌人实施攻击
    protected virtual void HandleAttack(){
    }

    //普攻：进入角色近身攻击范围（如0.5m）就转攻击
    protected virtual void GeneralAttack(){
        float distance = (player.transform.position - transform.position).magnitude;;//敌人和玩家间的距离
        Debug.Log("目前距离=="+distance);
        if (distance <= stat.current.attackRange) {
            isInAttackRange = true;
            //敌人转attack状态
            Debug.Log("敌人实施普攻");
        }
    }

    /// <summary>
    /// 被动伤害：地刺、史莱姆等碰撞即减血
    /// </summary>
    protected virtual void ContactDamage(){
        //如果能够造成被动伤害
        if (canContactDamage) {
            var colliders = EntityContactOverlap(contactColliders, stat.current.contactAllowance);
            //Debug.Log("敌人目前近身检测碰撞体数："+colliders);
            for(int i = 0;i < colliders;i++)
            {
                if (contactColliders[i].CompareTag("Player") && contactColliders[i].TryGetComponent<Player>(out Player player)) {
                    this.player = player;
                    //角色减血函数
                    player.ApplyDamage();
                    //事件
                }
            }
        }
    }

    //敌人自己受击，积累伤害
    protected virtual void GetHit(){
        
    }
    
    //二阶段or回血
    protected virtual void StageTwo(){
        
    }
}
