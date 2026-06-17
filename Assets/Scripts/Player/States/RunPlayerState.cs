using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class RunPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.IsFrozeVelocity(false);
        player.InputEnabled = true;
        player.maxSpeedMulti = 2.0f;
    }

    protected override void OnExit(Player player){
        player.maxSpeedMulti = 1.0f;
    }

    protected override void OnStep(Player player){
        //Debug.Log(player.horizontalVelocity.magnitude);
        // var anim = player.GetComponentInChildren<Animator>();
        // Debug.Log(anim.GetNextAnimatorStateInfo(0).normalizedTime);
        player.Gravity();
        player.SnapToGround();
        player.Roll();
        var inputDirection = player.inputs.GetMovementCSDirction();
        if (inputDirection.sqrMagnitude > 0) {
            player.Accelerate(inputDirection);  
            player.FaceDirectionSmooth(inputDirection);
        }
        if (player.inputs.RunOnReleased()) {
            if (inputDirection.sqrMagnitude != 0) {
                player.states.Change<WalkPlayerState>();
            }else if (inputDirection.sqrMagnitude <= 0) {
                player.Decelerate();
                player.states.Change<BreakPlayerState>();
            }
        }
    }

    public override void OnContact(Player player, Collider other){
        
    }
}
