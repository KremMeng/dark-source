using System;
using Unity.VisualScripting;
using UnityEngine;
//继承自Entity基类的敌人类
//主要职责：根据视野距离索敌、攻击-->就相当于player的硬件检测输入
public class Enemy : Entity<Enemy> {

    protected override void Awake(){
        base.Awake();
        //初始化各类组件
        InitializeStatManager();
    }

    protected override void Update(){
        //base.Update();
        HandleSight();
        ContactDamage();
    }
    
    //敌人类相关属性，如血量、速度、组件等
    public EnemyHealth health { get; protected set; }
    public EnemyStatManager stat { get; protected set; }


    //玩家
    public Player player { get;protected set; }


    protected void InitializeStatManager() => stat = GetComponent<EnemyStatManager>();
    
    //攻击相关参数
    [SerializeField] protected bool canContactDamage;
    
    //碰撞相关参数,const编译器常量更省
    protected const int maxViewColliders = 128;
    protected const int maxAttackColliders = 128;
    
    protected Collider[] viewColliders= new Collider[maxViewColliders];  //视野范围内
    protected Collider[] attackColliders= new Collider[maxAttackColliders];  //追击范围内
    protected Collider[] contactColliders = new Collider[10]; //近身（被动伤害）范围内
    
    //用Enemy的数值，封装基类相关函数

    //根据视野绑定/解绑玩家
    protected virtual void HandleSight(){
        float radius = stat.current.viewRange;
        
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position,radius,viewColliders);
        //Debug.Log("敌人目前视野范围内GO数："+numColliders);
        //没锁定玩家时，遍历碰撞体数组，检测到player就锁定
        if (!player) {
            for(int i = 0;i < numColliders;i++)
            {
                //Debug.Log("视野范围内碰撞体："+viewColliders[i]+"  tag为" + viewColliders[i].tag);
                if (viewColliders[i].CompareTag("Player") && viewColliders[i].TryGetComponent<Player>(out Player player)) {
                    this.player = player;
                    float distance = (player.transform.position - transform.position).magnitude;;//敌人和玩家间的距离
                    //Debug.Log("角色进入视野范围，距离：" + distance);
                    //事件
                }
            }
        }
        //出视野范围或角色死亡解除锁定
        else{
            float distance = (player.transform.position - transform.position).magnitude;;//敌人和玩家间的距离
            if(distance > stat.current.viewRange) {
                this.player = null;
                //Debug.Log("角色已离开视野范围，距离：" + distance);
                //事件
            }
        }
    }

    //player进入攻击范围内就实施攻击
    protected virtual void HandleAttack(){
        
    }

    //普攻：进入角色近身攻击范围（如0.5m）就转攻击
    protected virtual void GeneralAttack(){
        
    }

    /// <summary>
    /// 被动伤害：地刺、史莱姆等碰撞即减血
    /// </summary>
    protected virtual void ContactDamage(){
        //如果能够造成被动伤害
        if (canContactDamage) {
            var colliders = EntityContactOverlap(contactColliders, stat.current.contactAllowance);
            Debug.Log("敌人目前近身检测碰撞体数："+colliders);
            for(int i = 0;i < colliders;i++)
            {
                Debug.Log("近身碰撞体："+contactColliders[i]+"  tag为" + viewColliders[i].tag);
                if (contactColliders[i].CompareTag("Player") && contactColliders[i].TryGetComponent<Player>(out Player player)) {
                    this.player = player;
                    Debug.Log("角色接触到敌人，产生被动伤害：");
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
