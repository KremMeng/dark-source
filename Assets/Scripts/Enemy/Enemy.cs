using System;
using UnityEngine;
//继承自Entity基类的敌人类
//主要职责：根据视野距离索敌、攻击-->就相当于player的硬件检测输入
public class Enemy : Entity<Enemy> {

    protected override void Awake(){
        base.Awake();
        //初始化各类组件
    }

    protected override void Update(){
        base.Update();
        HandleSight();
        HandleAttack();
    }
    
    //敌人类相关属性，如血量
    public EnemyHealth health { get; protected set; }
    
    
    //用Enemy的数值，封装基类相关函数

    //视野范围内检测到碰撞体player就锁定，出视野范围解除锁定
    protected virtual void HandleSight(){
        
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
