using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//一个抽象的状态基类
public abstract class EntityState<T> where T : Entity<T> {

    public UnityEvent onEnter; //进入状态触发事件

    public UnityEvent onExit;   //退出状态触发事件
    
    public float timeSinecEntered { get; protected set; }   //计时/s

    protected abstract void OnEnter(T entity);
    protected abstract void OnExit(T entity);
    protected abstract void OnStep(T entity);
    public abstract void OnContact(T entity, Collider other);   //处理碰撞

    //进入状态时调用，触发onEnter事件
    public void Enter(T entity){
        
        timeSinecEntered = 0;   //进入事件重置计时
        onEnter?.Invoke();  //触发回调
        OnEnter(entity);    //调用子类定义的进入逻辑
    }
    /// <summary>
    /// 退出状态时调用，触发onExit事件
    /// </summary>
    /// <returns></returns>
    public void Exit(T entity){
        onExit?.Invoke();
        OnExit(entity);
    }

    //每帧调用
    public void Step(T entity){
        OnStep(entity);
        timeSinecEntered += Time.deltaTime;
    }

    //反射机制实例化状态，不具体区分实例，返回笼统的EntityState<T>类型
    public static EntityState<T> CreateFromStatesNameString(string typeName){
        var type = System.Type.GetType(typeName);//typeName来自于扫描的各个状态 ？
        return (EntityState<T>)Activator.CreateInstance(type);//object强制转换
    }

    //把获取的实例加入list
    public static List<EntityState<T>> CreateListFromStatesArray(string[] arr){
        List<EntityState<T>> list = new List<EntityState<T>>();
        
        foreach (var typeName in arr) {
            list.Add(CreateFromStatesNameString(typeName));
        }
        return list;
    }
}
