using UnityEngine;
public class JumpLandPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.InputEnabled = false;
        player.OnJumpLandFinish(() => {
            // 播放完jab动画后切换到Idle状态
            player.states.Change<IdlePlayerState>();
        });           
    }

    protected override void OnExit(Player player){
        
    }

    protected override void OnStep(Player player){
        player.Gravity();
    }

    public override void OnContact(Player player, Collider other){
        
    }
}
