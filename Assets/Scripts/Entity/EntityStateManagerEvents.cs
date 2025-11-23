using System;
using UnityEngine.Events;

[Serializable]
public class EntityStateManagerEvents {
    
    /// <summary>
    /// 有状态切换时触发事件
    /// </summary>
    public UnityEvent onChange;
    /// <summary>
    /// 进入某状态时触发事件，并传递所进入的状态类型，方便外部监听
    /// </summary>
    public UnityEvent<Type> onEnter;
    /// <summary>
    /// 退出某状态时触发事件，并传递所退出入的状态类型，方便外部监听
    /// </summary>
    public UnityEvent<Type> onExit;
}
