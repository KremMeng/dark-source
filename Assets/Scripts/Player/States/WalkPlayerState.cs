using UnityEngine;

public class WalkPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.IsFrozeVelocity(false);
        player.InputEnabled = true;
        player.maxSpeedMulti = 1.0f;
    }

    protected override void OnExit(Player player){
        
    }

    protected override void OnStep(Player player){
        var anim = player.GetComponentInChildren<Animator>();
        Debug.Log("walk: "+anim.GetNextAnimatorStateInfo(0).normalizedTime);
        // Debug.Log("roll pressed?"+ player.inputs.RollOnPressed());
        // Debug.Log("isgrounded? "+ player.isGrounded);
        // Debug.Log("inputs.RollOnPressed()? "+ player.inputs.RollOnPressed());
         //if(player.horizontalVelocity.magnitude <=0.6) Debug.Log("speed " + (player.horizontalVelocity.magnitude ));
         //Debug.Log("speed " + (player.horizontalVelocity.magnitude ));
        player.Gravity();//用cc的话需要接入手写的重力
        player.SnapToGround();
        //player.Fall();
        player.Run();
        player.Roll();
        //检测相机空间下的玩家输入
        var inputDirection = player.inputs.GetMovementCameraDirction();
        player.Accelerate(inputDirection);
        if (inputDirection.sqrMagnitude > 0) {
            player.FaceDirectionSmooth(inputDirection);
        }
        //走路状态下如果没有输入，根据摩擦力减速
        else if(player.states.curIndex == 1 && inputDirection.sqrMagnitude <= 0){
            player.Decelerate();
            //减速到零，切换到Idle
            if (player.horizontalVelocity.sqrMagnitude <= 0.1f) {
                player.states.Change<IdlePlayerState>();
            }
        }
    }

    public override void OnContact(Player player, Collider other){
        
    }
}
