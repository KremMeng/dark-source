using UnityEngine;
/// <summary>
/// 非战斗状态时向上跳、用于跑图攀爬等
/// </summary>
public class JumpUpPlayerState : PlayerState{
    protected override void OnEnter(Player player){
        player.InputEnabled = false;
        player.verticalVelocity = new Vector3(0, player.stat.current.maxJumpHeight, 0);//0.5-0.8倍身高
        player.jumpCounter++;
        player.playerEvents.OnJump?.Invoke();
        player.OnJumpUpFinish(() => {
            // 播放完jab动画后切换到Idle状态
            player.states.Change<JumpLandPlayerState>();
        });
    }
    
    protected override void OnStep(Player player){
        player.Gravity();
    }
    
    protected override void OnExit(Player player){
        
    }


    public override void OnContact(Player player, Collider other){
        
    }
}
