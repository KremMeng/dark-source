using UnityEngine;
    
public class WalkState : IState {
    private Transform npc;
    
    /// <summary>
    /// 构造函数初始化
    /// </summary>
    public WalkState(Transform npc){
            this.npc = npc;
    }
    
    public override void OnEnter(){
        //Debug.Log("npc walk OnEnter");
    }
    public override void OnStep(){
        //Debug.Log("npc walk OnStep");
    }
    public override void OnExit(){
       // Debug.Log("npc walk Exit");
    }
}
