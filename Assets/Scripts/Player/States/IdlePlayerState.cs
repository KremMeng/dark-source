using UnityEngine;

public class IdlePlayerState : PlayerState {
    protected override void OnEnter(Player player){
        
    }

    protected override void OnExit(Player player){
        
    }

    protected override void OnStep(Player player){
        //player.Friction();
        player.Gravity();//用cc的话需要接入手写的重力
        //Player.SnapToGround();
        player.Jump();
        player.Fall();
        var inputDirection = player.inputs.GetMovementDirction();
        if (inputDirection.sqrMagnitude > 0 || player.horizontalVelocity.sqrMagnitude > 0) {
            player.states.Change<WalkPlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other){
        
    }
}