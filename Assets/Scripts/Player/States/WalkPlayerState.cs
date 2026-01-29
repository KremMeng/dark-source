using UnityEngine;

public class WalkPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.IsNotFreeze();
    }

    protected override void OnExit(Player player){
        
    }

    protected override void OnStep(Player player){
        player.Gravity();//用cc的话需要接入手写的重力
        player.Fall();
        player.Run();
        //检测相机空间下的玩家输入
        var inputDirection = player.inputs.GetMovementCameraDirction();
        if (inputDirection.sqrMagnitude > 0) {
            player.Accelerate(inputDirection);
            player.FaceDirectionSmooth(inputDirection);
            player.Roll();
            player.Jump();
        }else{
            //没有输入，根据摩擦力减速
            player.Friction();
            //减速到零，切换到Idle
            if (player.horizontalVelocity.sqrMagnitude <= 0.1f) {
                player.states.Change<IdlePlayerState>();
            }
        }
    }

    public override void OnContact(Player player, Collider other){
        
    }
}
