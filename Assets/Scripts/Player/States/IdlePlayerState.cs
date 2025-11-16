using UnityEngine;
public class IdlePlayerState : PlayerState {
    protected override void OnEnter(Player player){
        
    }

    protected override void OnExit(Player player){
        
    }

    protected override void OnStep(Player player){
        
        Debug.Log("IdlePlayerState::Onstep");st
    }

    public override void OnContact(Player player, Collider other){
        
    }
}