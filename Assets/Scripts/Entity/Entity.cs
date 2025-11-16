using System;
using UnityEngine;


public abstract class EntityBase : MonoBehaviour {
    
    
}
//泛型抽象类，给T增加一个泛型约束
public abstract class Entity<T> : EntityBase where T : Entity<T> {
    public EntityStateManager<T> states { get; protected set; } //类型是Manager，对外意图是“玩家的所有状态”
    protected virtual void Awake(){
        //初始化状态管理器
        InitializeStateManager();
    }

    protected virtual void Update(){
        //处理状态机步进逻辑
        HandlleStates();
    }
    
    //Entity需要Manager来驱动
    protected virtual void InitializeStateManager() => states = GetComponent<EntityStateManager<T>>();

    //轮询
    protected virtual void HandlleStates() => states.Step();
}
