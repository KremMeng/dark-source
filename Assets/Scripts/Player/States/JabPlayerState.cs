using UnityEngine;
public class JabPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        player.IsFrozeVelocity(true);
        player.InputEnabled = false;
        
        // 注册Jab动画完成回调
        player.OnJabFinish(() => {
            // 播放完jab动画后切换到Idle状态
            player.states.Change<IdlePlayerState>();
        });
    }

    protected override void OnExit(Player player){
        player.IsFrozeVelocity(false);
    } 

    protected override void OnStep(Player player){
        //后撤距离
        player.horizontalVelocity += player.transform.forward * -0.015f;
        // 不需要在OnStep中每帧检测，已经通过OnEnter中的回调处理
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
