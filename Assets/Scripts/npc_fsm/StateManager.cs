using System;
using System.Collections.Generic;
using UnityEngine;
public class StateManager {
    /// <summary>
    /// 状态转换规则类
    /// </summary>
    public class Transition {
        public IState from;
        public IState to;
        public Func<bool> condition;    //切换条件的委托
    
        public Transition(IState from, IState to, Func<bool> condition){
            this.from = from;
            this.to = to;
            this.condition = condition;
        }
    }
    
    protected IState currentState;//当前状态
    protected List<Transition> m_transitions;//存转换列表
    
    //构造函数
    public StateManager(IState initialState){
        m_transitions = new List<Transition>();
        currentState = initialState;    //确保初始状态不为空
        currentState.OnEnter();         //让第一个状态执行进入逻辑
    }
    
    //每帧更新
    public virtual void Tick(){
        var transition = GetTransition();
        if (transition != null) {
            ChangeState(transition.to);
        }
        currentState.OnStep();
    }
    
    //判断当前是否该转换
    public virtual Transition GetTransition(){
        foreach (var ts in m_transitions) {
            if (ts.from == currentState && ts.condition()) {
                return ts;
            }
        }
        return null;
    }

    //添加转换列表
    public virtual void AddTransition(IState from, IState to, Func<bool> condition){
        m_transitions.Add(new Transition(from, to, condition));
    }
    
    //状态转换
    public virtual void ChangeState(IState to){
        if (currentState == to) return; //判重
        currentState.OnExit();
        currentState = to;
        currentState.OnEnter();
    }
}
