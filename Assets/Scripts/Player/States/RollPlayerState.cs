using UnityEngine;
public class RollPlayerState : PlayerState {
    protected override void OnEnter(Player player){
        // // 【杀手锏】：强制重置 Animator 中 Roll 状态的播放进度为 0
        // // 这能解决因为 Transition 延迟或 normalizedTime 没归零导致的“间隔变长”问题
        // var animator = player.GetComponent<PlayerAnimator>();
        // if (animator != null && animator.anim != null) {
        //     animator.anim.Play(stateName: "roll", layer: 0, normalizedTime: 0f);
        // }

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
        var anim = player.GetComponent<PlayerAnimator>();
        Debug.Log("walk的播放进度： "+ anim.anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
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
