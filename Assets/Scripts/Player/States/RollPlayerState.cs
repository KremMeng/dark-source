using UnityEngine;
public class RollPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        Debug.Log($"[RollState] OnEnter - 速度: {player.horizontalVelocity.magnitude:F2}");

        //进入状态时给一个向移动方向冲量
        player.horizontalVelocity = player.transform.forward * player.horizontalVelocity.magnitude;
        player.IsFrozeVelocity(true);
        player.InputEnabled = false;
    }


    protected override void OnExit(Player player){
        player.IsFrozeVelocity(false);
        player.InputEnabled = true; // 确保退出时恢复输入
    } 

    protected override void OnStep(Player player){
        
        //根据上个状态索引，播放完roll的动画转到上个状态
        if (player.states.lastIndex == 1) {
            player.ChangeOnAnimFinish(() => player.states.Change<WalkPlayerState>());
        }else if (player.states.lastIndex == 7) {
            player.ChangeOnAnimFinish(() => player.states.Change<RunPlayerState>());
        }
        
    }

    public override void OnContact(Player player, Collider other){
        
    } 
}
