using System;
using UnityEngine;

//管理所有状态机的抽象基类
public abstract class EntityStateManager : MonoBehaviour {
    
}
//泛型子类，管理特定类型T的状态机
public abstract class EntityStateManager<T> : EntityStateManager where T : Entity<T> {
    
}
