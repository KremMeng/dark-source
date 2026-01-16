using UnityEngine;

public class IdlePlayerState : PlayerState {
    protected override void OnEnter(Player player){
        Vector3 velocity = player.horizontalVelocity;
        velocity = Vector3.zero;
    }

    protected override void OnExit(Player player){
        
    }

    protected override void OnStep(Player player){
        Debug.Log(player.horizontalVelocity);
        //player.Gravity();//用cc的话需要接入手写的重力
        player.Fall();
        player.Run();
        player.Jump();
        var inputDirection = player.inputs.GetMovementDirction();
        if (inputDirection.sqrMagnitude > 0) {
            player.states.Change<WalkPlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other){
        
    }
}