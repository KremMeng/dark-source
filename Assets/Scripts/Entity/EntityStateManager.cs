using System;
using System.Collections.Generic;
using UnityEngine;

//管理所有状态机的抽象基类
public abstract class EntityStateManager : MonoBehaviour {
    
}
//泛型子类，管理特定类型T的状态机
public abstract class EntityStateManager<T> : EntityStateManager where T : Entity<T> {
    
    protected List<EntityState<T>> m_list = new List<EntityState<T>>(); //所有状态列表
    protected Dictionary<Type,EntityState<T>> m_state = new Dictionary<Type, EntityState<T>>();//键:类型，值:实例
    public EntityState<T> current { get; protected set; }//外界只读
    public T entity { get; protected set; }

    protected abstract List<EntityState<T>> GetStateList();  //子类实现

    protected virtual void Start(){
        InitialzeEntity();
        InitializeStates();
    }
    //Manager需要从Entity获得数据,从当前go获取实体组件T如animtor
    protected virtual void InitialzeEntity() => entity = GetComponent<T>();
    
    //初始化状态列表和状态字典
    protected virtual void InitializeStates(){
        //拿到状态列表
        m_list = GetStateList();
        //给列表里的状态元素配上type，加入状态字典
        foreach (var state in m_list) {
            
            var type = state.GetType();
            
            if (!m_state.ContainsKey(type)) {
                m_state.Add(type,state);
            }
        }
        //初始:把列表的第一个状态赋为current
        current = m_list[0];
    }
    //每帧调用更新状态逻辑
    public virtual void Step(){
        //current状态存在，游戏运行中
        if (current != null && Time.timeScale > 0) {
            current.Step(entity);
        }
    }
        
}
