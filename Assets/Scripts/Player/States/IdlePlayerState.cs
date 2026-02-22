using UnityEngine;

public class IdlePlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.IsIdleFreeze();
    }

    protected override void OnExit(Player player){
        player.IsNotFreeze();
    }

    protected override void OnStep(Player player){
        Debug.Log(player.horizontalVelocity);
        player.Gravity();//用cc的话需要接入手写的重力
        player.SnapToGround();
        player.Fall();
        player.Run();
        player.Jump();
        player.Roll();
        player.Friction();
        
        var inputDirection = player.inputs.GetMovementDirction();
        if (inputDirection.sqrMagnitude > 0) {
            player.states.Change<WalkPlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other){
        
    }
}