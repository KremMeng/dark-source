using UnityEngine;
public class JumpPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.verticalVelocity = new Vector3(0, 4.0f, 0);
        //player.IsFrozeVelocity(true);
        player.InputEnabled = false;
        
        // 注册Jump动画完成回调
        player.OnJumpFinish(() => {
            // 播放完jump动画后切换到Roll状态
            player.states.Change<RollPlayerState>();
        });
    }

    protected override void OnExit(Player player){
        //player.states.Change<RollPlayerState>();
        player.IsFrozeVelocity(false);
        
    } 

    protected override void OnStep(Player player){
        // 不需要在OnStep中每帧检测，已经通过OnEnter中的回调处理
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
