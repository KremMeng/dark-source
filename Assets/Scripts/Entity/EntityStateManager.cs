using System;
using System.Collections.Generic;
using UnityEngine;

//管理所有状态机的抽象基类
public abstract class EntityStateManager : MonoBehaviour {
   
    public EntityStateManagerEvents events;
}
//泛型子类，管理特定类型T的状态机
public abstract class EntityStateManager<T> : EntityStateManager where T : Entity<T> {
    public T entity { get; protected set; }
    protected List<EntityState<T>> m_list = new List<EntityState<T>>(); //所有状态列表
    protected Dictionary<Type,EntityState<T>> m_state = new Dictionary<Type, EntityState<T>>();//键:类型，值:实例
    /// <summary>
    /// 当前状态实例，外界只读
    /// </summary>
    public EntityState<T> current { get; protected set; }
    /// <summary>
    /// 上一个状态实例，外界只读
    /// </summary>
    public EntityState<T> last { get; protected set; }
    /// <summary>
    /// 状态列表中，当前状态的索引位置
    /// </summary>
    public int curIndex => m_list.IndexOf(current);
    /// <summary>
    /// 状态列表中，上一个状态的索引值
    /// </summary>
    public int lastIndex => m_list.IndexOf(last);
    
    

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

    public virtual void Change<TState>() where TState : EntityState<T>{
        var type = typeof(TState);
        if (m_state.ContainsKey(type)) {
            Change(m_state[type]);  //转换到目标type
        }
    }

    public virtual void Change(EntityState<T> targetState){
        // 目标状态存在、游戏未暂停时，调用退出状态的函数
       
        if (targetState != null && Time.timeScale > 0) {
            current.Exit(entity);
            events.onExit.Invoke(current.GetType());
            last = current;
        }
        //切换到目标状态
        current = targetState;
        current.Enter(entity);
        events.onEnter.Invoke(current.GetType());
        events.onChange?.Invoke();//只要成功切了状态（不管从哪到哪），只要切完就喊一声

    }
    
}
