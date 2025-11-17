using UnityEngine;
public class IdlePlayerState : PlayerState {
    protected override void OnEnter(Player player){
        
    }

    protected override void OnExit(Player player){
        
    }

    protected override void OnStep(Player player){
        
        Debug.Log("IdlePlayerState::Onstep");
        var inputDirection = player.inputs.GetMovementDirction();
        Debug.Log("InputDir = " + inputDirection);
    }

    public override void OnContact(Player player, Collider other){
        
    }
}