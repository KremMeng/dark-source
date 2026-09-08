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
        base.Update();
        HandleSight();
        HandleAttack();
    }
    
    //敌人类相关属性，如血量、速度、组件等
    public EnemyHealth health { get; protected set; }
    public EnemyStatManager stat { get; protected set; }


    //玩家
    public Player player { get;protected set; }


    protected void InitializeStatManager() => stat = GetComponent<EnemyStatManager>();
    
    //碰撞相关参数,const编译器常量更省
    protected const int maxViewColliders = 50;
    protected const int maxAttackColliders = 50;
    
    protected Collider[] viewColliders= new Collider[maxViewColliders];  
    protected Collider[] attackColliders= new Collider[maxAttackColliders];
    
    //用Enemy的数值，封装基类相关函数

    //视野范围内
    protected virtual void HandleSight(){
        float radius = stat.current.viewRange;
        float distance = 0;//敌人和玩家间的距离
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position,radius,viewColliders);
        //遍历碰撞体数组
        for(int i = 0;i < numColliders;i++)
        {
            //检测到碰撞体player就锁定
            if (viewColliders[i].CompareTag("Player") && viewColliders[i].TryGetComponent<Player>(out Player player)) {
                this.player = player;
                distance = (player.transform.position - transform.position).magnitude;
                Debug.Log("角色进入视野范围，距离：" + distance);
                
                //出视野范围解除锁定
                if(distance > stat.current.viewRange) {
                    this.player = null;
                    Debug.Log("角色已离开视野范围，距离：" + distance);
                }
            }
        }
    }

    //player进入攻击范围内就实施攻击
    protected virtual void HandleAttack(){
        
    }

    //普攻
    protected virtual void GeneralAttack(){
        
    }
    
    //敌人自己受击，积累伤害
    protected virtual void GetHit(){
        
    }
    
    //二阶段or回血
    protected virtual void StageTwo(){
        
    }
}
