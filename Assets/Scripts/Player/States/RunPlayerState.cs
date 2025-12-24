using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class RunPlayerState : PlayerState {
    protected override void OnEnter(Player player){
    }

    protected override void OnExit(Player player){
    }

    protected override void OnStep(Player player){
        player.Gravity();
        var inputDirection = player.inputs.GetMovementCameraDirction();
        player.FaceDirectionSmooth(inputDirection);
        //player.Accelerate(inputDirection);
        if (player.inputs.RunOnReleased()) {
            player.states.Change<BreakPlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other){
        
    }
}
