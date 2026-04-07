using UnityEngine;
public class AttackState : IState {
    public override void OnEnter(){
        //Debug.Log("npc enter attack");
    }

    public override void OnStep(){
        //Debug.Log("npc attack on step");
    }

    public override void OnExit(){
        //Debug.Log("npc exit attack");
    }
}
