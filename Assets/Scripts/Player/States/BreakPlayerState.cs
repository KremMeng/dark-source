using UnityEngine;
[AddComponentMenu("Asset/Player/States/Break Player State")]
public class BreakPlayerState : PlayerState{
    protected override void OnEnter(Player player){
        //播放完切idle,速度强制切0
        player.ChangeOnAnimFinish(() => player.states.Change<IdlePlayerState>());
        var inputDir = player.inputs.GetMovementCameraDirction();
        if (inputDir.sqrMagnitude <= 0 && player.inputs.RunOnReleased()) {
            player.horizontalVelocity =
                Vector3.Lerp(player.horizontalVelocity, Vector3.zero, player.stat.current.brakeLerp);
        }
    }

    protected override void OnExit(Player player){
        
    }

    protected override void OnStep(Player player){
        player.Gravity();
        player.Decelerate();
        player.Fall();
    }   

    public override void OnContact(Player player, Collider other){
        
    }
}
